using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    internal class SqlBrokerQueueInstallation : ISqlBrokerQueueInstallation
    {
        private readonly ILogger<SqlBrokerQueueInstallation> _logger;
        private readonly ISqlBrokerQueueSqlScriptGenerator _scriptGenerator;
        private readonly ISqlBrokerQueueSetupRepository _repository;

        public SqlBrokerQueueInstallation(
            ILogger<SqlBrokerQueueInstallation> logger,
            ISqlBrokerQueueSqlScriptGenerator scriptGenerator,
            ISqlBrokerQueueSetupRepository repository)
        {
            _logger = logger;
            _scriptGenerator = scriptGenerator;
            _repository = repository;
        }

        public async Task InstallAsync(CancellationToken cancellationToken)
        {
            await _repository.OpenSqlConnectionAsync();
            try
            {
                await _repository.BeginTransactionAsync();

                await InstallQueueAsync();
                cancellationToken.ThrowIfCancellationRequested();

                await InstallTableServicesAsync(cancellationToken);
                await InstallTableInsertedTriggersAsync(cancellationToken);
                await InstallTableUpdatedTriggersAsync(cancellationToken);
                await InstallTableDeletedTriggersAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                await _repository.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occured... rollback transaction");
                await _repository.RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _repository.CloseSqlConnectionAsync();
            }
        }

        private async Task InstallTableDeletedTriggersAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Installing delete triggers...");
            var tableDeletedTriggerSqlScripts = await _scriptGenerator.GenerateSetupTableDeletedTriggerSqlScripts();
            foreach (var tableDeletedTriggerSqlScript in tableDeletedTriggerSqlScripts)
            {
                await _repository.ExecuteSqlScriptAsync(tableDeletedTriggerSqlScript);
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation("Delete triggers successfully installed");
        }

        private async Task InstallTableUpdatedTriggersAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Installing update triggers...");
            var tableUpdatedTriggerSqlScripts = await _scriptGenerator.GenerateSetupTableUpdatedTriggerSqlScripts();
            foreach (var tableUpdatedTriggerSqlScript in tableUpdatedTriggerSqlScripts)
            {
                await _repository.ExecuteSqlScriptAsync(tableUpdatedTriggerSqlScript);
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation("Update triggers successfully installed");
        }

        private async Task InstallTableInsertedTriggersAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Installing insert triggers...");
            var tableInsertedTriggerSqlScripts = await _scriptGenerator.GenerateSetupTableInsertedTriggerSqlScripts();
            foreach (var tableInsertedTriggerSqlScript in tableInsertedTriggerSqlScripts)
            {
                await _repository.ExecuteSqlScriptAsync(tableInsertedTriggerSqlScript);
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation("Insert triggers successfully installed");
        }

        private async Task InstallTableServicesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Installing table services...");
            var tableServicesSqlScripts = await _scriptGenerator.GenerateSetupTableServicesSqlScripts();
            foreach (var tableServicesSqlScript in tableServicesSqlScripts)
            {
                await _repository.ExecuteSqlScriptAsync(tableServicesSqlScript);
                cancellationToken.ThrowIfCancellationRequested();
            }

            _logger.LogInformation("Table Installing table subscriptions successfully installed");
        }

        private async Task InstallQueueAsync()
        {
            _logger.LogInformation("Installing queue...");
            var setupQueueSqlScript = await _scriptGenerator.GenerateSetupQueueSqlScript();
            await _repository.ExecuteSqlScriptAsync(setupQueueSqlScript);
            _logger.LogInformation("Queue successfully installed");
        }
    }
}