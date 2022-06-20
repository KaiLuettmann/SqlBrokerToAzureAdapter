using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Dsl;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Setup.Test.Consumers.SqlBrokerQueues
{
    public class SqlBrokerQueueSqlScriptGeneratorTests
    {
        private readonly Fixture _fixture;

        public SqlBrokerQueueSqlScriptGeneratorTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(testOutputHelper.BuildLoggerFor<SqlBrokerQueueSqlScriptGenerator>());
        }

        [Fact]
        public async Task GenerateSetupQueueSqlScript_ShouldNotContainAnyPlaceholder()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            var result = await testObject.GenerateSetupQueueSqlScript();

            //Assert
            using (new AssertionScope())
            {
                result.Should().NotContain(SqlBrokerQueueSqlScriptGenerator.PlaceholderWrapper);
            }
        }

        [Fact]
        public async Task GenerateSetupQueueSqlScript_ShouldContainDatabaseName()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            var result = await testObject.GenerateSetupQueueSqlScript();

            //Assert
            using (new AssertionScope())
            {
                result.Should().Contain(_fixture.Configuration.DatabaseName);
            }
        }

        [Fact]
        public async Task GenerateSetupQueueSqlScript_ShouldContainSchemaName()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            var result = await testObject.GenerateSetupQueueSqlScript();

            //Assert
            using (new AssertionScope())
            {
                result.Should().Contain(_fixture.Configuration.SchemaName);
            }
        }

        [Fact]
        public async Task GenerateSetupQueueSqlScript_ShouldContainReceiverQueueName()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            var result = await testObject.GenerateSetupQueueSqlScript();

            //Assert
            using (new AssertionScope())
            {
                result.Should().Contain(_fixture.Configuration.ReceiverQueueName);
            }
        }

        [Fact]
        public async Task GenerateSetupQueueSqlScript_ShouldContainSenderQueueName()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            var result = await testObject.GenerateSetupQueueSqlScript();

            //Assert
            using (new AssertionScope())
            {
                result.Should().Contain(_fixture.Configuration.SenderQueueName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldNotContainAnyPlaceholder()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().NotContain(SqlBrokerQueueSqlScriptGenerator.PlaceholderWrapper);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainDatabaseName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.DatabaseName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainSchemaName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.SchemaName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainReceiverQueueName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.ReceiverQueueName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainSenderQueueName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.SenderQueueName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainTableName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().TableName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainUpdateBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().UpdateBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainInsertBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().InsertBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableServicesSqlScripts_ShouldContainDeleteBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableServicesSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().DeleteBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableDeletedTriggerSqlScripts_ShouldContainSchemaName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableDeletedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.SchemaName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableDeletedTriggerSqlScripts_ShouldContainTableName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableDeletedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().TableName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableDeletedTriggerSqlScripts_ShouldContainDeleteBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableDeletedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().DeleteBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableDeletedTriggerSqlScripts_ShouldContainDatabaseName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableDeletedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.DatabaseName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableDeletedTriggerSqlScripts_ShouldNotContain()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableDeletedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().NotContain(_fixture.Configuration.TableSubscriptions.Single().PrimaryKeyColumnName);
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().InsertBrokerMessageTypeName);
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().UpdateBrokerMessageTypeName);
                result.Should().NotContain(_fixture.Configuration.SenderQueueName);
                result.Should().NotContain(_fixture.Configuration.ReceiverQueueName);
                result.Should().NotContain(SqlBrokerQueueSqlScriptGenerator.PlaceholderWrapper);
            }
        }

        [Fact]
        public async Task GenerateSetupTableInsertedTriggerSqlScripts_ShouldContainSchemaName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableInsertedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.SchemaName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableInsertedTriggerSqlScripts_ShouldContainTableName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableInsertedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().TableName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableInsertedTriggerSqlScripts_ShouldContainInsertBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableInsertedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().InsertBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableInsertedTriggerSqlScripts_ShouldContainDatabaseName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableInsertedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.DatabaseName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableInsertedTriggerSqlScripts_ShouldNotContain()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableInsertedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().NotContain(_fixture.Configuration.TableSubscriptions.Single().PrimaryKeyColumnName);
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().DeleteBrokerMessageTypeName);
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().UpdateBrokerMessageTypeName);
                result.Should().NotContain(_fixture.Configuration.SenderQueueName);
                result.Should().NotContain(_fixture.Configuration.ReceiverQueueName);
                result.Should().NotContain(SqlBrokerQueueSqlScriptGenerator.PlaceholderWrapper);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldContainSchemaName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.SchemaName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldContainTableName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().TableName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldContainUpdateBrokerMessageTypeName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            using (new AssertionScope())
            {
                //Assert
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().UpdateBrokerMessageTypeName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldContainPrimaryKeyColumnName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.TableSubscriptions.Single().PrimaryKeyColumnName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldContainDatabaseName()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should().Contain(_fixture.Configuration.DatabaseName);
            }
        }

        [Fact]
        public async Task GenerateSetupTableUpdatedTriggerSqlScripts_ShouldNotContain()
        {
            //Arrange
            _fixture.SetupWithOneTableSubscription();
            var testObject = _fixture.CreateTestObject();

            //Act
            var results = (await testObject.GenerateSetupTableUpdatedTriggerSqlScripts())
                .ToList();

            //Assert
            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                var result = results.Single();
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().DeleteBrokerMessageTypeName);
                result.Should()
                    .NotContain(_fixture.Configuration.TableSubscriptions.Single().InsertBrokerMessageTypeName);
                result.Should().NotContain(_fixture.Configuration.SenderQueueName);
                result.Should().NotContain(_fixture.Configuration.ReceiverQueueName);
                result.Should().NotContain(SqlBrokerQueueSqlScriptGenerator.PlaceholderWrapper);
            }
        }

        private class Fixture
        {
            private readonly ILogger<SqlBrokerQueueSqlScriptGenerator> _logger;
            private readonly ICustomizationComposer<SqlBrokerQueueGenerationConfiguration> _configurationBuilder;
            private readonly ICustomizationComposer<SqlBrokerTableSubscriptionGenerationConfiguration> _tableSubscriptionBuilder;

            public Fixture(ILogger<SqlBrokerQueueSqlScriptGenerator> logger)
            {
                _logger = logger;
                _configurationBuilder = new AutoFixture.Fixture().Build<SqlBrokerQueueGenerationConfiguration>();
                _tableSubscriptionBuilder =
                    new AutoFixture.Fixture().Build<SqlBrokerTableSubscriptionGenerationConfiguration>();
            }

            public ISqlBrokerQueueGenerationConfiguration Configuration { get; private set; }
            private SqlBrokerTableSubscriptionGenerationConfiguration[] TableSubscriptions { get; set; }

            public SqlBrokerQueueSqlScriptGenerator CreateTestObject()
            {
                Configuration = _configurationBuilder
                    .With(x => x.TableSubscriptions, TableSubscriptions)
                    .Create();
                return new SqlBrokerQueueSqlScriptGenerator(_logger, Configuration);
            }

            public void SetupWithOneTableSubscription()
            {
                TableSubscriptions = _tableSubscriptionBuilder.CreateMany(1).ToArray();
            }
        }
    }
}