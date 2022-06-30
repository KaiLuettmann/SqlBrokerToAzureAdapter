using System;
using FluentAssertions;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.Producers.Common.Exceptions;
using SqlBrokerToAzureAdapter.Test.TestModelBuilders;
using Xunit;

namespace SqlBrokerToAzureAdapter.Test.Adapter.Models
{
    public class EventsTest
    {
        private readonly Fixture _fixture;

        public EventsTest()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Ctor_TwoEventsWithDifferentPayloadType_ShouldThrow()
        {
            // Arrange
            _fixture.SetupEventWithFakePayloadType();
            _fixture.SetupEventWithAnotherFakePayloadType();

            // Act
            Action act = () => _ = new Events(
                new[]
                {
                    _fixture.EventWithFakePayloadType,
                    _fixture.EventWithAnotherFakePayloadType
                });

            // Assert
            act.Should().ThrowExactly<UnexpectedPayloadTypeException>();
        }

        private sealed class Fixture
        {
            public Event EventWithFakePayloadType { get; private set; }

            public Event EventWithAnotherFakePayloadType { get; private set; }

            public void SetupEventWithFakePayloadType()
            {
                EventWithFakePayloadType = new EventBuilder().WithPayload(new FakePayload()).Create();
            }

            public void SetupEventWithAnotherFakePayloadType()
            {
                EventWithAnotherFakePayloadType = new EventBuilder().WithPayload(new AnotherFakePayload()).Create();
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