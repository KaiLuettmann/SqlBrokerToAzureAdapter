using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.Logging;
using Moq;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Test.TestModelBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Test.Consumers.SqlBrokerQueues
{
    public class SqlBrokerMessageHandlerTests
    {
        private readonly Fixture<FakeDataBaseContract> _fixture;

        public SqlBrokerMessageHandlerTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture<FakeDataBaseContract>(testOutputHelper.BuildLoggerFor<SqlBrokerMessageHandler<FakeDataBaseContract>>());
        }

        [Fact]
        public async Task Handle_WithInserted_ShouldReceiveInserted()
        {
            //Arrange
            _fixture.SetupHandler(SqlBrokerMessageType.Inserted);
            _fixture.SetupInsertedValues();
            _fixture.SetupDeserializeInsert();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.HandleAsync(_fixture.BrokerMessage);

            //Assert
            _fixture.VerifyReceiveInserted(Times.Once());
        }

        [Fact]
        public async Task Handle_WithUpdated_ShouldReceiveUpdated()
        {
            //Arrange
            _fixture.SetupHandler(SqlBrokerMessageType.Updated);
            _fixture.SetupUpdatedValues();
            _fixture.SetupDeserializeUpdate();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.HandleAsync(_fixture.BrokerMessage);

            //Assert
            _fixture.VerifyReceiveUpdated(Times.Once());
        }

        [Fact]
        public async Task Handle_WithDeleted_ShouldReceiveDeleted()
        {
            //Arrange
            _fixture.SetupHandler(SqlBrokerMessageType.Deleted);
            _fixture.SetupDeletedValues();
            _fixture.SetupDeserializeDelete();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.HandleAsync(_fixture.BrokerMessage);

            //Assert
            _fixture.VerifyReceiveDeleted(Times.Once());
        }

        [Theory]
        [InlineData(SqlBrokerMessageType.Inserted)]
        [InlineData(SqlBrokerMessageType.Updated)]
        [InlineData(SqlBrokerMessageType.Deleted)]
        public void Handle_ReceiverThrowsException_ShouldThrow(SqlBrokerMessageType sqlBrokerMessageType)
        {
            //Arrange
            _fixture.SetupHandler(sqlBrokerMessageType);
            _fixture.SetupValues();
            _fixture.SetupDeserialize();
            _fixture.SetupReceiverThrowsException();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.HandleAsync(_fixture.BrokerMessage);

            //Assert
            act.Should().Throw<Exception>();
        }

        private class Fixture<T>
        {
            private readonly ILogger<SqlBrokerMessageHandler<T>> _logger;
            private readonly Mock<IMessageBodyDeserializer<T>> _messageBodyDeserializerMock;
            private readonly Mock<ISqlBrokerMessageReceiver<T>> _receiverMock;
            private SqlBrokerMessageType _sqlBrokerMessageType;
            private string _brokerMessageTypeName;

            public Fixture(ILogger<SqlBrokerMessageHandler<T>> logger)
            {
                _logger = logger;
                _messageBodyDeserializerMock = new Mock<IMessageBodyDeserializer<T>>(MockBehavior.Strict);
                _receiverMock = new Mock<ISqlBrokerMessageReceiver<T>>();
            }

            public BrokerMessage BrokerMessage { get; private set; }

            private Metadata Metadata => new Metadata(BrokerMessage.ConversationHandle, BrokerMessage.MessageEnqueueTime);

            IEnumerable<T> DeserializedInsertedValues { get;  set; }

            IEnumerable<T> DeserializedDeletedValues { get;  set; }

            IEnumerable<UpdatedPair<T>> DeserializedUpdatedValues { get; set; }

            public SqlBrokerMessageHandler<T> CreateTestObject()
            {
                return new SqlBrokerMessageHandler<T>(
                    _logger,
                    _receiverMock.Object,
                    _messageBodyDeserializerMock.Object,
                    _brokerMessageTypeName,
                    _sqlBrokerMessageType
                );
            }

            private void SetupBrokerMessageTypeName()
            {
                _brokerMessageTypeName = $"{nameof(FakeDataBaseContract)}_{_sqlBrokerMessageType}";
            }

            public void SetupHandler(SqlBrokerMessageType sqlBrokerMessageType)
            {
                _sqlBrokerMessageType = sqlBrokerMessageType;
                SetupBrokerMessageTypeName();
            }

            public void SetupDeserializeInsert()
            {
                _messageBodyDeserializerMock
                    .Setup(x => x.ToInsertContract(It.Is<string>(value => value == BrokerMessage.MessageBody)))
                    .Returns(DeserializedInsertedValues);
            }

            public void SetupDeserializeDelete()
            {
                _messageBodyDeserializerMock
                    .Setup(x => x.ToDeletedContract(It.Is<string>(value => value == BrokerMessage.MessageBody)))
                    .Returns(DeserializedDeletedValues);
            }

            public void SetupDeserializeUpdate()
            {
                _messageBodyDeserializerMock
                    .Setup(x => x.ToUpdateContract(It.Is<string>(value => value == BrokerMessage.MessageBody)))
                    .Returns(DeserializedUpdatedValues);
            }

            public void SetupInsertedValues(int count = 1)
            {
                SetupBrokerMessage();
                DeserializedInsertedValues = new Fixture().CreateMany<T>(count);
            }

            private void SetupBrokerMessage()
            {
                BrokerMessage = new BrokerMessageBuilder().WithMessageTypeName(_brokerMessageTypeName).Create();
            }

            public void SetupDeletedValues(int count = 1)
            {
                SetupBrokerMessage();
                DeserializedDeletedValues = new Fixture().CreateMany<T>(count);
            }

            public void SetupUpdatedValues(int count = 1)
            {
                SetupBrokerMessage();
                DeserializedUpdatedValues = new Fixture().CreateMany<UpdatedPair<T>>(count);
            }

            public void VerifyReceiveInserted(Times times)
            {
                _receiverMock.Verify(x => x.ReceiveInsertedAsync(
                    It.Is<Metadata>(value =>
                        value.Timestamp == Metadata.Timestamp
                        && value.CorrelationId == Metadata.CorrelationId),
                    It.Is<IEnumerable<T>>(value => value.IsSameOrEqualTo(DeserializedInsertedValues))), times);
            }

            public void VerifyReceiveDeleted(Times times)
            {
                _receiverMock.Verify(x => x.ReceiveDeletedAsync(
                    It.Is<Metadata>(value =>
                        value.Timestamp == Metadata.Timestamp
                        && value.CorrelationId == Metadata.CorrelationId),
                    It.Is<IEnumerable<T>>(value => value.IsSameOrEqualTo(DeserializedDeletedValues))), times);
            }

            public void VerifyReceiveUpdated(Times times)
            {
                _receiverMock.Verify(x => x.ReceiveUpdatedAsync(
                    It.Is<Metadata>(value =>
                        value.Timestamp == Metadata.Timestamp
                        && value.CorrelationId == Metadata.CorrelationId),
                    It.Is<IEnumerable<UpdatedPair<T>>>(value => value.IsSameOrEqualTo(DeserializedUpdatedValues))), times);
            }

            public void SetupValues(int count = 1)
            {
                SetupInsertedValues(count);
                SetupUpdatedValues(count);
                SetupDeletedValues(count);
            }

            public void SetupDeserialize()
            {
                SetupDeserializeInsert();
                SetupDeserializeUpdate();
                SetupDeserializeDelete();
            }

            public void SetupReceiverThrowsException()
            {
                _receiverMock.Setup(x => x.ReceiveDeletedAsync(It.IsAny<Metadata>(), It.IsAny<IEnumerable<T>>()))
                    .ThrowsAsync(new Exception("MyTestException"));

                _receiverMock.Setup(x => x.ReceiveInsertedAsync(It.IsAny<Metadata>(), It.IsAny<IEnumerable<T>>()))
                    .ThrowsAsync(new Exception("MyTestException"));

                _receiverMock.Setup(x => x.ReceiveUpdatedAsync(It.IsAny<Metadata>(), It.IsAny<IEnumerable<UpdatedPair<T>>>()))
                    .ThrowsAsync(new Exception("MyTestException"));
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public class FakeDataBaseContract
        {
        }
    }
}