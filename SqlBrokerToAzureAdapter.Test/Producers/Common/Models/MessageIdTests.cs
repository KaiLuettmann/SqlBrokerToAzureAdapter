using System;
using FluentAssertions;
using SqlBrokerToAzureAdapter.Producers.Common.Models;
using Xunit;

namespace SqlBrokerToAzureAdapter.Test.Producers.Common.Models
{
    public class MessageIdTests
    {
        private readonly Fixture _fixture;

        public MessageIdTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void Ctor_Twice_ShouldReturnSameResult()
        {
            // Arrange
            _fixture.SetupCorrelationId();
            _fixture.SetupEntityId();
            _fixture.SetupPayloadType();

            // Act
            var message1 = new MessageId(_fixture.CorrelationId, _fixture.EntityId, _fixture.PayloadType);
            var message2 = new MessageId(_fixture.CorrelationId, _fixture.EntityId, _fixture.PayloadType);

            // Assert
            message1.Should().BeEquivalentTo(message2);
        }

        [Fact]
        public void Ctor_PayloadTypeIsNull_ShouldThrow()
        {
            // Arrange
            _fixture.SetupCorrelationId();
            _fixture.SetupEntityId();

            // Act
            Action act = () => new MessageId(_fixture.CorrelationId, _fixture.EntityId, null);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*payloadType*");
        }

        [Fact]
        public void Ctor_CorrelationIdIsNull_ShouldThrow()
        {
            // Arrange
            _fixture.SetupEntityId();
            _fixture.SetupPayloadType();

            // Act
            Action act = () => new MessageId(null, _fixture.EntityId, _fixture.PayloadType);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*correlationId*");
        }

        [Fact]
        public void Ctor_EntityIdIsNull_ShouldThrow()
        {
            // Arrange
            _fixture.SetupCorrelationId();
            _fixture.SetupPayloadType();

            // Act
            Action act = () => new MessageId(_fixture.CorrelationId, null, _fixture.PayloadType);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("*entityId*");
        }

        private sealed class Fixture
        {
            public Guid CorrelationId { get; private set; }
            public string EntityId { get; private set; }
            public Type PayloadType { get; set; }

            public void SetupCorrelationId()
            {
                CorrelationId = Guid.NewGuid();
            }

            public void SetupEntityId()
            {
                EntityId = "EntityId";
            }

            public void SetupPayloadType()
            {
                PayloadType = typeof(int);
            }
        }
    }
}