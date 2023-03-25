using System.IO;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using SqlBrokerToAzureAdapter.Testkit.Consumers;
using SqlBrokerToAzureAdapter.Transformations.Tests.Extensions.FluentAssertion;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;
using Xunit;

namespace SqlBrokerToAzureAdapter.Transformations.Tests.Users.SqlBrokerMessageContracts
{
    public class UserContractTests
    {
        private readonly MessageBodyDeserializerFixture<UserContract> _fixture;

        public UserContractTests()
        {
            _fixture = new MessageBodyDeserializerFixture<UserContract>();
        }

        [Fact]
        public void Deserialize_ShouldHaveNoEmptyProperties()
        {
            //Arrange
            var sqlBrokerMessageBodyJson = File.ReadAllText("Users/SqlBrokerMessageContracts/UserContract.complete.json");

            //Act
            var sqlBrokerMessageContracts = _fixture.Deserialize(sqlBrokerMessageBodyJson).ToList();

            //Assert
            using (new AssertionScope())
            {
                sqlBrokerMessageContracts.Should().HaveCount(1);
                var sqlBrokerMessageContract = sqlBrokerMessageContracts.Single();

                sqlBrokerMessageContract.Should().NotHavePropertyWithEmptyValue();
            }
        }

        [Fact]
        public void Deserialize_ShouldHaveNoPropertyWithDefaultValue()
        {
            //Arrange
            var sqlBrokerMessageBodyJson = File.ReadAllText("Users/SqlBrokerMessageContracts/UserContract.complete.json");

            //Act
            var sqlBrokerMessageContracts = _fixture.Deserialize(sqlBrokerMessageBodyJson).ToList();

            //Assert
            using (new AssertionScope())
            {
                sqlBrokerMessageContracts.Should().HaveCount(1);
                var sqlBrokerMessageContract = sqlBrokerMessageContracts.Single();

                sqlBrokerMessageContract.Should().NotHavePropertyWithDefaultValue();
            }
        }

        [Fact]
        public void Deserialize_ShouldHaveNoNullProperties()
        {
            //Arrange
            var sqlBrokerMessageBodyJson = File.ReadAllText("Users/SqlBrokerMessageContracts/UserContract.complete.json");

            //Act
            var sqlBrokerMessageContracts = _fixture.Deserialize(sqlBrokerMessageBodyJson).ToList();

            //Assert
            using (new AssertionScope())
            {
                sqlBrokerMessageContracts.Should().HaveCount(1);
                var sqlBrokerMessageContract = sqlBrokerMessageContracts.Single();
                sqlBrokerMessageContract.Should().NotHavePropertyWithNullValue();
            }
        }
    }
}