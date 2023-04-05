using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;
using SqlBrokerToAzureAdapter.Test.TestModelBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Test.Consumers.SqlBrokerQueues
{
    public class SqlBrokerQueueConsumerTests
    {
        private readonly Fixture _fixture;

        public SqlBrokerQueueConsumerTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(testOutputHelper.BuildLoggerFor<SqlBrokerQueueConsumer>());
        }

        [Fact]
        public async Task RunAsync_WhereHandleThrowsException_ShouldRollbackTransaction()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupHandleThrowsException();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupSkipCurrentBrokerMessage();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyRollbackTransaction(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WhereHandleThrowsExceptionAndConfiguredSkipping_ShouldSkipCurrentBrokerMessage()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupHandleThrowsException();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupSkipCurrentBrokerMessage();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifySkipCurrentBrokerMessage(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WhereHandleThrowsExceptionAndNoConfiguredSkipping_ShouldNotSkipCurrentBrokerMessage()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupHandleThrowsException();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupDisableSkipCurrentBrokerMessageOnException();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifySkipCurrentBrokerMessage(Times.Never());
        }

        [Fact]
        public async Task RunAsync_WithUnknownMessageType_ShouldRollbackTransaction()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupWithUnknownMessageType();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupSkipCurrentBrokerMessage();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyRollbackTransaction(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WithUnknownMessageType_ShouldSkipCurrentBrokerMessage()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupWithUnknownMessageType();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupSkipCurrentBrokerMessage();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifySkipCurrentBrokerMessage(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WithNullMessageAndNoConfiguredSkipping_ShouldNotResetSkippedBrokerMessages()
        {
            //Arrange
            _fixture.SetupWithNullMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupRollbackTransaction();
            _fixture.SetupDisableSkipCurrentBrokerMessageOnException();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyResetSkippedBrokerMessages(Times.Never());
        }

        [Fact]
        public async Task RunAsync_WithMessage_ShouldHandleMessage()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupCommitTransaction();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyHandleMessage(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WithMessage_ShouldCommit()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupCommitTransaction();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyCommitTransaction(Times.Once());
        }

        [Fact]
        public async Task RunAsync_WithMessage_ShouldCloseSqlConnection()
        {
            //Arrange
            _fixture.SetupWithMessage();
            _fixture.SetupReceiveNextMessage();
            _fixture.SetupBrokerMessageTypeName();
            _fixture.SetupCommitTransaction();
            _fixture.SetupCloseSqlConnection();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.RunAsync(_fixture.CancellationToken);

            //Assert
            _fixture.VerifyCloseSqlConnection(Times.Once());
        }

        private class Fixture
        {
            private readonly Mock<ISqlBrokerMessageHandler> _brokerMessageHandlerMock;
            private readonly Mock<ISqlBrokerQueueConfiguration> _sqlBrokerQueueConfigurationMock;
            private readonly Mock<ISqlBrokerQueueRepository> _repositoryMock;
            private readonly ILogger<SqlBrokerQueueConsumer> _logger;
            private readonly CancellationTokenSource _cancellationTokenSource;

            public Fixture(ILogger<SqlBrokerQueueConsumer> logger)
            {
                _brokerMessageHandlerMock = new Mock<ISqlBrokerMessageHandler>();
                _sqlBrokerQueueConfigurationMock = new Mock<ISqlBrokerQueueConfiguration>();
                _repositoryMock = new Mock<ISqlBrokerQueueRepository>(MockBehavior.Strict);
                _logger = logger;
                _cancellationTokenSource = new CancellationTokenSource();
            }

            private string MessageTypeName { get; } = "TestMessageTypeName";
            private BrokerMessage BrokerMessage { get; set; }

            public CancellationToken CancellationToken
            {
                get
                {
                    return _cancellationTokenSource.Token;
                }
            }

            public SqlBrokerQueueConsumer CreateTestObject()
            {
                return new SqlBrokerQueueConsumer(
                    _logger,
                    _sqlBrokerQueueConfigurationMock.Object,
                    new SqlBrokerMessageHandlerCollection{_brokerMessageHandlerMock.Object},
                    _repositoryMock.Object
                );
            }

            public void SetupHandleThrowsException()
            {
                _brokerMessageHandlerMock
                    .Setup(x => x.HandleAsync(It.Is<BrokerMessage>(value => value == BrokerMessage)))
                    .ThrowsAsync(new InvalidOperationException());
            }

            public void SetupWithUnknownMessageType()
            {
                _brokerMessageHandlerMock.SetupGet(x => x.BrokerMessageTypeName)
                    .Returns("JustAnotherMessageType");
            }

            public void SetupWithNullMessage()
            {
                BrokerMessage = null;
            }

            public void SetupWithMessage()
            {
                BrokerMessage = new BrokerMessageBuilder().WithMessageTypeName(MessageTypeName).Create();
            }

            public void SetupBrokerMessageTypeName()
            {
                _brokerMessageHandlerMock.SetupGet(x => x.BrokerMessageTypeName)
                    .Returns(MessageTypeName);
            }

            public void SetupSkipCurrentBrokerMessage()
            {
                SetupEnableSkipCurrentBrokerMessageOnException();
                _repositoryMock.Setup(x => x.SkipCurrentBrokerMessageAsync()).Returns(Task.CompletedTask);
            }

            private void SetupEnableSkipCurrentBrokerMessageOnException()
            {
                _sqlBrokerQueueConfigurationMock.SetupGet(x => x.SkipCurrentBrokerMessageOnException).Returns(true);
            }

            public void SetupDisableSkipCurrentBrokerMessageOnException()
            {
                _sqlBrokerQueueConfigurationMock.SetupGet(x => x.SkipCurrentBrokerMessageOnException).Returns(false);
            }

            public void SetupResetSkippedBrokerMessages()
            {
                SetupEnableSkipCurrentBrokerMessageOnException();
                _repositoryMock.Setup(x => x.ResetSkippedBrokerMessagesAsync()).Returns(Task.CompletedTask);
            }

            public void SetupRollbackTransaction()
            {
                _repositoryMock.Setup(x => x.RollbackTransactionAsync()).Returns(Task.CompletedTask);
            }

            public void SetupCommitTransaction()
            {
                _repositoryMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);
            }

            public void SetupCloseSqlConnection()
            {
                _repositoryMock.Setup(x => x.CloseSqlConnectionAsync())
                    .Callback(() => _cancellationTokenSource.Cancel())
                    .Returns(Task.CompletedTask);
            }

            public void SetupReceiveNextMessage()
            {
                _repositoryMock.Setup(x => x.OpenSqlConnectionAsync()).Returns(Task.CompletedTask);
                _repositoryMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);

                _repositoryMock.Setup(x => x.ReceiveNextMessageAsync())
                    .ReturnsAsync(BrokerMessage);
            }

            public void VerifySkipCurrentBrokerMessage(Times times)
            {
                _repositoryMock.Verify(x => x.SkipCurrentBrokerMessageAsync(), times);
            }

            public void VerifyResetSkippedBrokerMessages(Times times)
            {
                _repositoryMock.Verify(x => x.ResetSkippedBrokerMessagesAsync(), times);
            }

            public void VerifyHandleMessage(Times times)
            {
                _brokerMessageHandlerMock.Verify(x => x.HandleAsync(It.Is<BrokerMessage>(value => value == BrokerMessage)), times);
            }

            public void VerifyCommitTransaction(Times times)
            {
                _repositoryMock.Verify(x => x.CommitTransactionAsync(), times);
            }

            public void VerifyCloseSqlConnection(Times times)
            {
                _repositoryMock.Verify(x => x.CloseSqlConnectionAsync(), times);
            }

            public void VerifyRollbackTransaction(Times times)
            {
                _repositoryMock.Verify(x => x.RollbackTransactionAsync(), times);
            }
        }
    }
}