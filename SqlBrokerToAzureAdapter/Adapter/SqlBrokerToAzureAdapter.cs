using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Adapter.Exceptions;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Transformations.Models;

namespace SqlBrokerToAzureAdapter.Adapter
{
    internal sealed class SqlBrokerToAzureAdapter<TDatabaseContract> : ISqlBrokerMessageReceiver<TDatabaseContract>
    {
        private readonly IAddEventTransformations<TDatabaseContract> _addEventTransformations;
        private readonly IObjectComparer<TDatabaseContract> _comparer;
        private readonly IEditEventTransformations<TDatabaseContract> _editEventTransformations;
        private readonly ILogger<SqlBrokerToAzureAdapter<TDatabaseContract>> _logger;
        private readonly ISqlBrokerToAzureAdapterConfiguration _configuration;
        private readonly ITopicProducer _producer;
        private readonly IRemoveEventTransformations<TDatabaseContract> _removeEventTransformations;

        public SqlBrokerToAzureAdapter(
            ILogger<SqlBrokerToAzureAdapter<TDatabaseContract>> logger,
            ISqlBrokerToAzureAdapterConfiguration configuration,
            IAddEventTransformations<TDatabaseContract> addEventTransformations,
            IEditEventTransformations<TDatabaseContract> editEventTransformations,
            IRemoveEventTransformations<TDatabaseContract> removeEventTransformations,
            ITopicProducer producer,
            IObjectComparer<TDatabaseContract> comparer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _addEventTransformations = addEventTransformations ?? throw new ArgumentNullException(nameof(addEventTransformations));
            _editEventTransformations = editEventTransformations ?? throw new ArgumentNullException(nameof(editEventTransformations));
            _removeEventTransformations = removeEventTransformations ?? throw new ArgumentNullException(nameof(removeEventTransformations));
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
        }

        public async Task ReceiveInsertedAsync(Metadata metadata, IEnumerable<TDatabaseContract> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!_addEventTransformations.Any() && _configuration.ThrowIfNoAddEventTransformationIsPresent)
            {
                throw new MissingAddEventTransformationException();
            }

            var valueList = values.ToList();
            foreach (var addEventTransformation in _addEventTransformations)
            {
                if (valueList.Count == 0)
                {
                    continue;
                }
                var events = TransformEditedValues(valueList, addEventTransformation);

                await _producer.PublishAsync(metadata, events);
            }
        }

        private Events TransformEditedValues(IEnumerable<TDatabaseContract> valueList, IAddEventTransformation<TDatabaseContract> addEventTransformation)
        {
            IList<Event> events = new List<Event>();
            foreach (var @event in valueList.Select(addEventTransformation.Transform))
            {
                _logger.LogInformation("Insert values transformed to Azure-Event");

                events.Add(@event);
            }

            return new Events(events);
        }

        public async Task ReceiveUpdatedAsync(Metadata metadata, IEnumerable<UpdatedPair<TDatabaseContract>> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!_editEventTransformations.Any() && _configuration.ThrowIfNoEditEventTransformationIsPresent)
            {
                throw new MissingEditEventTransformationException();
            }

            var comparedValues = _comparer.Compare(values).Where(x => !x.IsEqual).ToList();
            foreach (var editEventTransformation in _editEventTransformations)
            {
                var events = TransformEditedValues(editEventTransformation, comparedValues);

                if (!events.Any())
                    continue;

                _logger.LogInformation("Edited values transformed to Azure-Event");

                await _producer.PublishAsync(metadata, events);
            }
        }

        private Events TransformEditedValues(IEditEventTransformation<TDatabaseContract> editEventTransformation, List<ComparedUpdatedPair<TDatabaseContract>> comparedValues)
        {
            var events = new List<Event>();
            foreach (var comparedValue in comparedValues)
            {
                var differenceList = comparedValue.Differences.ToList();

                if (!editEventTransformation.IsResponsibleFor(differenceList))
                {
                    _logger.LogDebug(
                        $"EditTransformation of type '{editEventTransformation.GetType().Name}' is not responsible for detected changes.");
                    continue;
                }

                _logger.LogDebug(
                    $"EditTransformation of type '{editEventTransformation.GetType().Name}' is responsible for detected changes.");

                var @event = editEventTransformation.Transform(comparedValue.UpdatedPair);
                events.Add(@event);
            }

            return new Events(events);
        }

        public async Task ReceiveDeletedAsync(Metadata metadata, IEnumerable<TDatabaseContract> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (!_removeEventTransformations.Any() && _configuration.ThrowIfNoRemoveEventTransformationIsPresent)
            {
                throw new MissingRemoveEventTransformationException();
            }

            var valueList = values.ToList();
            foreach (var removeEventTransformation in _removeEventTransformations)
            {
                if (valueList.Count == 0)
                {
                    continue;
                }
                var events = TransformRemovedValues(valueList, removeEventTransformation);
                await _producer.PublishAsync(metadata, events);
            }
        }

        private Events TransformRemovedValues(IEnumerable<TDatabaseContract> valueList, IRemoveEventTransformation<TDatabaseContract> removeEventTransformation)
        {
            var events = new List<Event>();
            foreach (var @event in valueList.Select(removeEventTransformation.Transform))
            {
                _logger.LogInformation("Deleted values transformed to Azure-Event");

                events.Add(@event);
            }

            return new Events(events);
        }
    }
}