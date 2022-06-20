using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;
using SqlBrokerToAzureAdapter.Testkit.Consumers;
using SqlBrokerToAzureAdapter.Testkit.Producers;
using Xunit;

namespace SqlBrokerToAzureAdapter.IntegrationTests.Users
{
    public class ProgramTests
    {
        private readonly Fixture _fixture;
        private const string BrokerMessagesPath = "Users/BrokerMessages/";
        private const string EventsPath = "Users/Events/";

        public ProgramTests()
        {
            _fixture = new Fixture();
        }

        [Theory]
        [InlineData("User.Inserted", "User.Inserted.json", "UserAddedEvent.json")]
        [InlineData("User.Deleted", "User.Deleted.json", "UserDeletedEvent.json")]
        [InlineData("User.Updated", "User.Updated.Name.json", "UserNameChangedEvent.json")]
        [InlineData("User.Updated", "User.Updated.Phone.json", "UserContactInfoChangedEvent.json")]
        public async Task Run_WithBrokerMessage_TopicShouldContainEvent(string messageTypeName, string messageBodyJsonFile, string expectedTopicContentJsonFile)
        {
            //Arrange
            _fixture.SetupSqlBrokerReturnsSequence(new BrokerMessage(
                Guid.NewGuid(),
                DateTime.Now,
                messageTypeName,
                await File.ReadAllTextAsync($"{BrokerMessagesPath}{messageBodyJsonFile}")
                )
            );

            //Act
            await _fixture.RunSqlBrokerQueueConsumerAsync();

            //Assert
            _fixture.AssertTopicContains(Path.GetFullPath($"{EventsPath}{expectedTopicContentJsonFile}"));
        }

        private class Fixture
        {
            private readonly SqlBrokerConsumerFixture _sqlBrokerConsumerFixture;
            private readonly AzureTopicProducerFixture _azureTopicProducerFixture;
            private readonly IServiceCollection _serviceCollection;
            private readonly string _topicPublishStoragePath;
            private ISqlBrokerQueueConsumer _sqlBrokerQueueConsumer;
            private readonly CancellationTokenSource _cancellationTokenSource;

            public Fixture()
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT","Development");
                _sqlBrokerConsumerFixture = new SqlBrokerConsumerFixture();
                _azureTopicProducerFixture = new AzureTopicProducerFixture();
                _serviceCollection = new Program().ConfigureServices();
                _topicPublishStoragePath = Path.GetFullPath("TopicPublishStorage.json");
                _cancellationTokenSource = new CancellationTokenSource();

                SetupTopicPublishesToFile();
            }

            private ISqlBrokerQueueConsumer CreateTestObject()
            {
                var serviceProvider = _serviceCollection.BuildServiceProvider();
                return Program.Configure(serviceProvider);
            }

            public void SetupSqlBrokerReturnsSequence(params BrokerMessage[] brokerMessages)
            {
                _sqlBrokerConsumerFixture.SetupSqlBrokerReturnsSequence(_serviceCollection, () => _cancellationTokenSource.Cancel(), brokerMessages);
            }

            private void SetupTopicPublishesToFile()
            {
                _azureTopicProducerFixture.SetupTopicPublishesToFile(_serviceCollection, _topicPublishStoragePath);
            }

            public void AssertTopicContains(string jsonFullPath)
            {
                var expected = File.ReadAllText(jsonFullPath);
                var actual = File.ReadAllText(_topicPublishStoragePath);

                actual.Should().Contain(expected);
            }

            public async Task RunSqlBrokerQueueConsumerAsync()
            {
                _sqlBrokerQueueConsumer = CreateTestObject();
                await _sqlBrokerQueueConsumer.RunAsync(_cancellationTokenSource.Token);
            }
        }
    }
}