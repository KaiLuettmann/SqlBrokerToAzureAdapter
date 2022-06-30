using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using SqlBrokerToAzureAdapter.Adapter;
using SqlBrokerToAzureAdapter.Adapter.Exceptions;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Test.TestModelBuilders;
using SqlBrokerToAzureAdapter.Test.TestUtilities.Moq;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Transformations.Models;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Test.Adapter
{
    public class SqlBrokerToAzureAdapterTests
    {
        private readonly Fixture _fixture;

        public SqlBrokerToAzureAdapterTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(testOutputHelper.BuildLoggerFor<SqlBrokerToAzureAdapter<FakeDataBaseContract>>());
        }

        [Fact]
        public void ReceiveInsertedAsync_WithNullValues_ShouldThrow()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveInsertedAsync(_fixture.Metadata, null);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>().WithMessage("*values*");
            }
        }

        [Fact]
        public void ReceiveInsertedAsync_WithoutAddTransformation_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupThrowIfNoEventTransformationIsPresent(false);
            _fixture.SetupWithoutAddTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow<MissingAddEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveInsertedAsync_WithoutAddTransformation_ShouldThrow()
        {
            //Arrange
            _fixture.SetupWithoutAddTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<MissingAddEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveInsertedAsync_WithoutValues_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupWithoutValues();
            _fixture.SetupWithAddTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow();
            }
        }

        [Fact]
        public async Task ReceiveInsertedAsync_WithoutValues_ShouldNotPublish()
        {
            //Arrange
            _fixture.SetupWithoutValues();
            _fixture.SetupWithAddTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(It.IsAny<Metadata>(), It.IsAny<Events>()),
                    Times.Never);
            }
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 2)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 3, 3)]
        public async Task ReceiveInsertedAsync_WithValuesAndAddTransformations_ShouldTransform(int valueCount,
            int transformationCount, int expectedTransformCallsPerTransformation)
        {
            //Arrange
            _fixture.SetupWithValues(valueCount);
            _fixture.SetupWithAddTransformations(transformationCount);
            _fixture.SetupPublishAdded();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                foreach (var addEventTransformationMock in _fixture.AddEventTransformationMocks)
                {
                    addEventTransformationMock.Verify(x => x.Transform(It.IsAny<FakeDataBaseContract>()),
                        Times.Exactly(expectedTransformCallsPerTransformation));
                }
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 1, 2, 1)]
        [InlineData(3, 1, 3, 1)]
        [InlineData(1, 2, 1, 2)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(1, 3, 1, 3)]
        [InlineData(2, 3, 2, 3)]
        [InlineData(3, 3, 3, 3)]
        public async Task ReceiveInsertedAsync_WithValuesAndAddTransformations_ShouldPublish(int valueCount,
            int transformationCount, int expectedEventCount, int expectedPublishCalls)
        {
            //Arrange
            _fixture.SetupWithValues(valueCount);
            _fixture.SetupWithAddTransformations(transformationCount);
            _fixture.SetupPublishAdded();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveInsertedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(
                        It.Is<Metadata>(value => value == _fixture.Metadata),
                        It.Is<Events>(value => value.Count() == expectedEventCount)),
                    Times.Exactly(expectedPublishCalls));
            }
        }

        [Fact]
        public void ReceiveDeletedAsync_WithNullValues_ShouldThrow()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveDeletedAsync(_fixture.Metadata, null);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>().WithMessage("*values*");
            }
        }

        [Fact]
        public void ReceiveDeletedAsync_WithoutRemoveTransformation_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupThrowIfNoEventTransformationIsPresent(false);
            _fixture.SetupWithoutRemoveTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow<MissingRemoveEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveDeletedAsync_WithoutRemoveTransformation_ShouldThrow()
        {
            //Arrange
            _fixture.SetupWithoutRemoveTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<MissingRemoveEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveDeletedAsync_WithoutValues_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupWithoutValues();
            _fixture.SetupWithRemoveTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow();
            }
        }

        [Fact]
        public async Task ReceiveDeletedAsync_WithoutValues_ShouldNotPublish()
        {
            //Arrange
            _fixture.SetupWithoutValues();
            _fixture.SetupWithRemoveTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(It.IsAny<Metadata>(), It.IsAny<Events>()),
                    Times.Never);
            }
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 2)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 3, 3)]
        public async Task ReceiveDeletedAsync_WithValuesAndRemoveTransformations_ShouldTransform(int valueCount,
            int transformationCount, int expectedTransformCallsPerTransformation)
        {
            //Arrange
            _fixture.SetupWithValues(valueCount);
            _fixture.SetupWithRemoveTransformations(transformationCount);
            _fixture.SetupPublishRemoved();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                foreach (var removeEventTransformationMock in _fixture.RemoveEventTransformationMocks)
                {
                    removeEventTransformationMock.Verify(x => x.Transform(It.IsAny<FakeDataBaseContract>()),
                        Times.Exactly(expectedTransformCallsPerTransformation));
                }
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 1, 2, 1)]
        [InlineData(3, 1, 3, 1)]
        [InlineData(1, 2, 1, 2)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(1, 3, 1, 3)]
        [InlineData(2, 3, 2, 3)]
        [InlineData(3, 3, 3, 3)]
        public async Task ReceiveDeletedAsync_WithValuesAndRemoveTransformation_ShouldPublish(int valueCount,
            int transformationCount, int expectedEventCount, int expectedPublishCalls)
        {
            //Arrange
            _fixture.SetupWithValues(valueCount);
            _fixture.SetupWithRemoveTransformations(transformationCount);
            _fixture.SetupPublishRemoved();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveDeletedAsync(_fixture.Metadata, _fixture.Values);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(
                        It.Is<Metadata>(value => value == _fixture.Metadata),
                        It.Is<Events>(value => value.Count() == expectedEventCount)),
                    Times.Exactly(expectedPublishCalls));
            }
        }


        [Fact]
        public void ReceiveUpdatedAsync_WithNullValues_ShouldThrow()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.ReceiveUpdatedAsync(_fixture.Metadata, null);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>().WithMessage("*values*");
            }
        }

        [Fact]
        public void ReceiveUpdatedAsync_WithoutEditTransformation_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupThrowIfNoEventTransformationIsPresent(false);
            _fixture.SetupWithoutEditTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () =>
                await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow<MissingRemoveEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveUpdatedAsync_WithoutEditTransformation_ShouldThrow()
        {
            //Arrange
            _fixture.SetupWithoutEditTransformation();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () =>
                await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<MissingEditEventTransformationException>();
            }
        }

        [Fact]
        public void ReceiveUpdatedAsync_WithoutUpdatedValues_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupWithoutUpdatedValues();
            _fixture.SetupWithDifferences();
            _fixture.SetupWithEditTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () =>
                await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                act.Should().NotThrow();
            }
        }

        [Fact]
        public async Task ReceiveUpdatedAsync_WithoutUpdatedValues_ShouldNotPublish()
        {
            //Arrange
            _fixture.SetupWithoutUpdatedValues();
            _fixture.SetupWithEditTransformations();
            _fixture.SetupWithDifferences();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(It.IsAny<Metadata>(), It.IsAny<Events>()),
                    Times.Never);
            }
        }


        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 2)]
        [InlineData(2, 3, 2)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 2, 3)]
        [InlineData(3, 3, 3)]
        public async Task ReceiveUpdatedAsync_WithUpdatedValuesAndEditTransformations_ShouldTransform(int valueCount,
            int transformationCount, int expectedTransformCallsPerTransformation)
        {
            //Arrange
            _fixture.SetupWithUpdatedValues(valueCount);
            _fixture.SetupWithDifferences();
            _fixture.SetupWithEditTransformations(transformationCount);
            _fixture.SetupPublishEdited();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                foreach (var editEventTransformationMock in _fixture.EditEventTransformationMocks)
                {
                    editEventTransformationMock.Verify(x => x.Transform(It.IsAny<UpdatedPair<FakeDataBaseContract>>()),
                        Times.Exactly(expectedTransformCallsPerTransformation));
                }
            }
        }

        [Fact]
        public async Task ReceiveUpdatedAsync_WithUpdatedValuesAndWithoutDifferences_ShouldNotTransform()
        {
            //Arrange
            _fixture.SetupWithUpdatedValues();
            _fixture.SetupWithoutDifferences();
            _fixture.SetupWithEditTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                foreach (var editEventTransformationMock in _fixture.EditEventTransformationMocks)
                {
                    editEventTransformationMock.Verify(x => x.Transform(It.IsAny<UpdatedPair<FakeDataBaseContract>>()),
                        Times.Never);
                }
            }
        }

        [Fact]
        public async Task ReceiveUpdatedAsync_WithUpdatedValuesAndWithoutDifferences_ShouldNotPublish()
        {
            //Arrange
            _fixture.SetupWithUpdatedValues();
            _fixture.SetupWithoutDifferences();
            _fixture.SetupWithEditTransformations();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(
                    It.Is<Metadata>(value => value == _fixture.Metadata),
                    It.IsAny<Events>()), Times.Never);
            }
        }

        [Theory]
        [InlineData(1, 1, 1, 1)]
        [InlineData(2, 1, 2, 1)]
        [InlineData(3, 1, 3, 1)]
        [InlineData(1, 2, 1, 2)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(1, 3, 1, 3)]
        [InlineData(2, 3, 2, 3)]
        [InlineData(3, 3, 3, 3)]
        public async Task ReceiveUpdatedAsync_WithUpdatedValuesAndEditTransformation_ShouldPublish(int valueCount,
            int transformationCount, int expectedEventCount, int expectedPublishCalls)
        {
            //Arrange
            _fixture.SetupWithUpdatedValues(valueCount);
            _fixture.SetupWithDifferences();
            _fixture.SetupWithEditTransformations(transformationCount);
            _fixture.SetupPublishEdited();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.ReceiveUpdatedAsync(_fixture.Metadata, _fixture.UpdatedValues);

            //Assert
            using (new AssertionScope())
            {
                _fixture.ProducerMock.Verify(x => x.PublishAsync(
                        It.Is<Metadata>(value => value == _fixture.Metadata),
                        It.Is<Events>(value => value.Count() == expectedEventCount)),
                    Times.Exactly(expectedPublishCalls));
            }
        }

        private class Fixture
        {
            private readonly Mock<IObjectComparer<FakeDataBaseContract>> _comparerMock;
            private readonly ILogger<SqlBrokerToAzureAdapter<FakeDataBaseContract>> _logger;
            private readonly Mock<ISqlBrokerToAzureAdapterConfiguration> _configurationMock;

            private readonly RemoveEventTransformations<FakeDataBaseContract> _removeEventTransformations =
                new RemoveEventTransformations<FakeDataBaseContract>();

            private readonly AddEventTransformations<FakeDataBaseContract> _addEventTransformations =
                new AddEventTransformations<FakeDataBaseContract>();

            private readonly EditEventTransformations<FakeDataBaseContract> _editEventTransformations =
                new EditEventTransformations<FakeDataBaseContract>();

            private readonly MockRepository _mockRepository;
            private readonly EventBuilder _eventBuilder;
            private readonly ComparedUpdatedPairBuilder<FakeDataBaseContract> _comparedUpdatedPairBuilder;

            public Fixture(ILogger<SqlBrokerToAzureAdapter<FakeDataBaseContract>> logger)
            {
                _logger = logger;

                _mockRepository = new MockRepository(MockBehavior.Strict);
                _comparerMock = _mockRepository.Create<IObjectComparer<FakeDataBaseContract>>();
                _configurationMock = _mockRepository.Create<ISqlBrokerToAzureAdapterConfiguration>();
                ProducerMock = _mockRepository.Create<ITopicProducer>();
                _eventBuilder = new EventBuilder();
                _comparedUpdatedPairBuilder = new ComparedUpdatedPairBuilder<FakeDataBaseContract>();

                SetupThrowIfNoEventTransformationIsPresent(true);
            }

            internal Mock<ITopicProducer> ProducerMock { get; }

            internal IEnumerable<Mock<IAddEventTransformation<FakeDataBaseContract>>> AddEventTransformationMocks
            {
                get;
                private set;
            }

            internal IEnumerable<Mock<IRemoveEventTransformation<FakeDataBaseContract>>> RemoveEventTransformationMocks
            {
                get;
                private set;
            }

            internal IEnumerable<Mock<IEditEventTransformation<FakeDataBaseContract>>> EditEventTransformationMocks
            {
                get;
                private set;
            }

            public Metadata Metadata { get; } = new Metadata(Guid.NewGuid(), DateTime.Now);

            public IEnumerable<FakeDataBaseContract> Values { get; private set; } = new List<FakeDataBaseContract>
                {new FakeDataBaseContract()};

            public IEnumerable<UpdatedPair<FakeDataBaseContract>> UpdatedValues { get; private set; } =
                new List<UpdatedPair<FakeDataBaseContract>> {new UpdatedPair<FakeDataBaseContract>()};

            public SqlBrokerToAzureAdapter<FakeDataBaseContract> CreateTestObject()
            {
                return new SqlBrokerToAzureAdapter<FakeDataBaseContract>(
                    _logger,
                    _configurationMock.Object,
                    _addEventTransformations,
                    _editEventTransformations,
                    _removeEventTransformations,
                    ProducerMock.Object,
                    _comparerMock.Object
                );
            }

            public void SetupThrowIfNoEventTransformationIsPresent(bool shouldThrow)
            {
                _configurationMock.SetupGet(x => x.ThrowIfNoAddEventTransformationIsPresent).Returns(shouldThrow);
                _configurationMock.SetupGet(x => x.ThrowIfNoEditEventTransformationIsPresent).Returns(shouldThrow);
                _configurationMock.SetupGet(x => x.ThrowIfNoRemoveEventTransformationIsPresent).Returns(shouldThrow);
            }

            public void SetupWithoutAddTransformation()
            {
                SetupWithAddTransformations(0);
            }

            public void SetupWithAddTransformations(int count = 1)
            {
                AddEventTransformationMocks = _mockRepository
                    .CreateMany<IAddEventTransformation<FakeDataBaseContract>>(count).ToList();
                _addEventTransformations.AddRange(AddEventTransformationMocks.Select(x => x.Object));
            }

            public void SetupWithoutRemoveTransformation()
            {
                SetupWithRemoveTransformations(0);
            }

            public void SetupWithRemoveTransformations(int count = 1)
            {
                RemoveEventTransformationMocks = _mockRepository
                    .CreateMany<IRemoveEventTransformation<FakeDataBaseContract>>(count).ToList();
                _removeEventTransformations.AddRange(RemoveEventTransformationMocks.Select(x => x.Object));
            }

            public void SetupWithoutEditTransformation()
            {
                SetupWithEditTransformations(0);
            }

            public void SetupWithEditTransformations(int count = 1)
            {
                EditEventTransformationMocks = _mockRepository
                    .CreateMany<IEditEventTransformation<FakeDataBaseContract>>(count).ToList();
                _editEventTransformations.AddRange(EditEventTransformationMocks.Select(x => x.Object));
            }

            public void SetupWithoutValues()
            {
                Values = new List<FakeDataBaseContract>();
            }

            public void SetupWithValues(int count = 1)
            {
                Values = new AutoFixture.Fixture().CreateMany<FakeDataBaseContract>(count);
            }

            public void SetupWithoutUpdatedValues()
            {
                UpdatedValues = new List<UpdatedPair<FakeDataBaseContract>>();
            }

            public void SetupWithUpdatedValues(int count = 1)
            {
                UpdatedValues = new AutoFixture.Fixture().CreateMany<UpdatedPair<FakeDataBaseContract>>(count);
            }

            public void SetupPublishAdded()
            {
                foreach (var addEventTransformationMock in AddEventTransformationMocks)
                {
                    var @event = new Event(Guid.NewGuid().ToString(), new object());
                    addEventTransformationMock.Setup(x => x.Transform(It.IsAny<FakeDataBaseContract>()))
                        .Returns(@event);

                    ProducerMock.Setup(x => x.PublishAsync(
                            It.Is<Metadata>(value => value == Metadata),
                            It.Is<Events>(value => value.Contains(@event))))
                        .Returns(Task.CompletedTask);
                }
            }

            public void SetupPublishRemoved()
            {
                foreach (var removeEventTransformationMock in RemoveEventTransformationMocks)
                {
                    var @event = _eventBuilder.Create();
                    removeEventTransformationMock.Setup(x => x.Transform(It.IsAny<FakeDataBaseContract>()))
                        .Returns(@event);

                    ProducerMock.Setup(x => x.PublishAsync(
                            It.Is<Metadata>(value => value == Metadata),
                            It.Is<Events>(value => value.Contains(@event))))
                        .Returns(Task.CompletedTask);
                }
            }

            public void SetupPublishEdited()
            {
                foreach (var editEventTransformationMock in EditEventTransformationMocks)
                {
                    var @event = _eventBuilder.Create();
                    editEventTransformationMock.Setup(x => x.IsResponsibleFor(It.IsAny<IEnumerable<Difference>>()))
                        .Returns(true);
                    editEventTransformationMock.Setup(x => x.Transform(It.IsAny<UpdatedPair<FakeDataBaseContract>>()))
                        .Returns(@event);

                    ProducerMock.Setup(x => x.PublishAsync(
                            It.Is<Metadata>(value => value == Metadata),
                            It.Is<Events>(value => value.Contains(@event))))
                        .Returns(Task.CompletedTask);
                }
            }

            public void SetupWithDifferences()
            {
                SetupUpdatedValuesHaveDifferences(value => _comparedUpdatedPairBuilder.WithUpdatedPair(value).Create());
            }

            public void SetupWithoutDifferences()
            {
                SetupUpdatedValuesHaveDifferences(value =>
                    _comparedUpdatedPairBuilder.WithUpdatedPair(value).WithoutDifferences().Create());
            }

            private void SetupUpdatedValuesHaveDifferences(
                Func<UpdatedPair<FakeDataBaseContract>, ComparedUpdatedPair<FakeDataBaseContract>>
                    comparedUpdatedPairFactory)
            {
                _comparerMock.Setup(x =>
                        x.Compare(It.Is<IEnumerable<UpdatedPair<FakeDataBaseContract>>>(
                            value => value.Equals(UpdatedValues))))
                    .Returns<IEnumerable<UpdatedPair<FakeDataBaseContract>>>(values =>
                        values.Select(comparedUpdatedPairFactory));
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public class FakeDataBaseContract
        {
        }
    }
}