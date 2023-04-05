using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Moq;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.AzureTopics;
using SqlBrokerToAzureAdapter.Producers.Common;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;
using SqlBrokerToAzureAdapter.Producers.Common.Models;
using SqlBrokerToAzureAdapter.Test.TestModelBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Test.Producers.AzureTopics
{
    public class AzureTopicProducerTests
    {
        private readonly Fixture _fixture;

        public AzureTopicProducerTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(testOutputHelper.BuildLoggerFor<AzureTopicProducer>());
        }

        [Theory]
        [InlineData(7, 3, 5, 3)]
        [InlineData(1000, 1000, 1, 1)]
        [InlineData(2000, 1000, 3, 2)]
        [InlineData(3000, 1000, 7, 4)]
        [InlineData(4000, 1000, 7, 4)]
        [InlineData(5000, 1000, 15, 8)]
        [InlineData(6000, 1000, 15, 8)]
        [InlineData(7000, 1000, 15, 8)]
        [InlineData(8000, 1000, 15, 8)]
        [InlineData(9000, 1000, 31, 16)]
        [InlineData(10000, 1000, 31, 16)]
        public async Task Publish_WithManyMessages_ShouldBatch(int messageCount, int maxMessageCountPerBatch, int expectedBatchTries, int expectedSuccessfullySend)
        {
            //Arrange
            _fixture.SetupEventsWithCount(messageCount);
            _fixture.SetupTopicClientCanSendMessageCountLowerThan(maxMessageCountPerBatch);
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.TopicClient.Verify(x => x.SendAsync(It.IsAny<IList<Message>>()),
                    Times.Exactly(expectedBatchTries));
                _fixture.OccuredMessageSizeExceededExceptions.Should().Be(expectedBatchTries - expectedSuccessfullySend);
            }
        }

        [Fact]
        public void Publish_WithTooBigPayload_ShouldThrow()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientFailsWithTooBigPayload();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<MessageSizeExceededException>();
            }
        }

        [Fact]
        public void Publish_WithNullMetadata_ShouldThrow()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.PublishAsync(null, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>().WithMessage("*metadata*");
            }
        }

        [Fact]
        public void Publish_WithNullEvents_ShouldThrow()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.PublishAsync(_fixture.Metadata, null);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<ArgumentNullException>().WithMessage("*events*");
            }
        }

        [Fact]
        public async Task Publish_Successfully_MessageIdShouldBeAsExpected()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                var @event = _fixture.Events.Single();
                sendMessage.MessageId.Should().BeEquivalentTo(new MessageId(_fixture.Metadata.CorrelationId,
                    @event.EntityId, @event.PayloadType).ToString());
            }
        }

        [Fact]
        public async Task Publish_Successfully_MessageShouldHaveCorrelationId()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                sendMessage.CorrelationId.Should().Be(_fixture.Metadata.CorrelationId.ToString());
            }
        }

        [Fact]
        public async Task Publish_Successfully_MessageShouldHaveContentTypeWithValueApplicationJson()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                sendMessage.ContentType.Should().Be(MediaTypeNames.Application.Json);
            }
        }

        [Fact]
        public async Task Publish_Successfully_MessageShouldHaveTimestampUserProperties()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                sendMessage.UserProperties.ContainsKey("Timestamp").Should().BeTrue();
                sendMessage.UserProperties["Timestamp"].Should().NotBeNull();
            }
        }

        [Fact]
        public void Publish_EntityIdIEqualToCorrelationId_ShouldThrow()
        {
            //Arrange
            _fixture.SetupEventsWhereEntityIdIEqualToCorrelationIdWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<InvalidEntityIdException>();
            }
        }

        [Fact]
        public async Task Publish_WithoutEvents_TopicClientShouldNotSend()
        {
            //Arrange
            _fixture.SetupEventsWithCount(0);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.VerifyTopicClientDoesNotSend();
            }
        }

        [Fact]
        public async Task Publish_Successfully_MessageShouldHaveLabelWithPayloadTypeFullname()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                sendMessage.Label.Should().Be(_fixture.Events.Single().PayloadType.FullName);
            }
        }

        [Fact]
        public async Task Publish_Successfully_PayloadShouldEquivalentToSourcePayload()
        {
            //Arrange
            _fixture.SetupEventsWithCount(1);
            _fixture.SetupTopicClientSuccessfully();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.PublishAsync(_fixture.Metadata, _fixture.Events);

            //Assert
            using (new AssertionScope())
            {
                _fixture.SendMessages.Should().HaveCount(1);
                var sendMessage = _fixture.SendMessages.Single();

                sendMessage.Body.Should().NotBeNull();
                var body = Encoding.UTF8.GetString(sendMessage.Body);
                var payload = JsonSerializer.Deserialize<FakePayload>(body);
                payload.Should().BeEquivalentTo(_fixture.SourcePayload);
            }
        }

        private class Fixture
        {
            private readonly Mock<IAzureTopicClientFactory> _topicClientFactory;
            private readonly ILogger<AzureTopicProducer> _logger;
            public FakePayload SourcePayload { get; } = new FakePayload {FakeProperty = "FakeProperty"};
            public string Topic { get; } = "FakeTopic";

            public List<Message> SendMessages { get; } = new List<Message>();
            public Mock<ITopicClient> TopicClient { get; }

            public Fixture(ILogger<AzureTopicProducer> logger)
            {
                _topicClientFactory = new Mock<IAzureTopicClientFactory>();
                TopicClient = new Mock<ITopicClient>();
                _logger = logger;

                _topicClientFactory.Setup(x => x.Get(It.IsAny<Type>())).Returns(TopicClient.Object);
            }

            public Metadata Metadata { get; private set; }
            public Events Events { get; private set; }

            public int OccuredMessageSizeExceededExceptions { get; private set; }

            public AzureTopicProducer CreateTestObject()
            {
                return new AzureTopicProducer(
                    _logger,
                    _topicClientFactory.Object
                );
            }

            public void SetupTopicClientCanSendMessageCountLowerThan(int messageCount)
            {
                TopicClient.Setup(x => x.SendAsync(It.Is<IList<Message>>(messages => messages.Count > messageCount)))
                    .Callback(() => OccuredMessageSizeExceededExceptions++)
                    .Throws(new MessageSizeExceededException(string.Empty));
            }

            public void SetupEventsWithCount(int count)
            {
                Metadata = new Metadata(Guid.NewGuid(), DateTime.UtcNow);
                Events = new Events(new EventBuilder()
                    .WithPayload(SourcePayload)
                    .CreateMany(count));
            }

            public void SetupEventsWhereEntityIdIEqualToCorrelationIdWithCount(int count)
            {
                Metadata = new Metadata(Guid.NewGuid(), DateTime.UtcNow);
                Events = new Events(new EventBuilder()
                    .WithEntityId(Metadata.CorrelationId.ToString())
                    .WithPayload(SourcePayload)
                    .CreateMany(count));
            }

            public void SetupTopicClientFailsWithTooBigPayload()
            {
                TopicClient.Setup(x => x.SendAsync(It.IsAny<IList<Message>>()))
                    .Throws(new MessageSizeExceededException(string.Empty));
            }

            public void SetupTopicClientSuccessfully()
            {
                TopicClient.Setup(x => x.SendAsync(It.IsAny<IList<Message>>()))
                    .Callback<IList<Message>>(messages => SendMessages.AddRange(messages));
            }

            public void VerifyTopicClientDoesNotSend()
            {
                TopicClient.Verify(x => x.SendAsync(It.IsAny<Message>()), Times.Never());
            }
        }

        private class FakePayload
        {
            public string FakeProperty { get; set; }
        }

        private class AnotherFakePayload
        {
        }
    }
}