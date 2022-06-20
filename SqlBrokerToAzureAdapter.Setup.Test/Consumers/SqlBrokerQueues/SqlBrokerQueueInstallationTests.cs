using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Setup.Test.Consumers.SqlBrokerQueues
{
    public class SqlBrokerQueueInstallationTests
    {
        private readonly Fixture _fixture;

        public SqlBrokerQueueInstallationTests(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(testOutputHelper.BuildLoggerFor<SqlBrokerQueueInstallation>());
        }

        [Fact]
        public async Task InstallAsync_ShouldExecuteQueueSqlScript()
        {
            //Arrange
            _fixture.SetupGenerateQueue();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                _fixture.RepositoryMock.Verify(
                    x => x.ExecuteSqlScriptAsync(It.Is<string>(value =>
                        value.IsSameOrEqualTo(_fixture.QueueSqlScript))),
                    Times.Once());
            }
        }

        [Fact]
        public async Task InstallAsync_ShouldExecuteSqlScriptForEachInstallTableService()
        {
            //Arrange
            _fixture.SetupGenerateTableService();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                foreach (var tableServicesSqlScript in _fixture.TableServicesSqlScripts)
                {
                    _fixture.RepositoryMock.Verify(
                        x => x.ExecuteSqlScriptAsync(It.Is<string>(value =>
                            value.IsSameOrEqualTo(tableServicesSqlScript))),
                        Times.Once());
                }
            }
        }

        [Fact]
        public async Task InstallAsync_ShouldExecuteSqlScriptForEachTableInsertedTrigger()
        {
            //Arrange
            _fixture.SetupGenerateTableInsertedTriggers();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                foreach (var tableInsertedTriggerSqlScript in _fixture.TableInsertedTriggerSqlScripts)
                {
                    _fixture.RepositoryMock.Verify(
                        x => x.ExecuteSqlScriptAsync(It.Is<string>(value =>
                            value.IsSameOrEqualTo(tableInsertedTriggerSqlScript))), Times.Once());
                }
            }
        }

        [Fact]
        public async Task InstallAsync_ShouldExecuteSqlScriptForEachTableDeletedTrigger()
        {
            //Arrange
            _fixture.SetupGenerateTableDeletedTriggers();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                foreach (var tableDeletedTriggerSqlScript in _fixture.TableDeletedTriggerSqlScripts)
                {
                    _fixture.RepositoryMock.Verify(
                        x => x.ExecuteSqlScriptAsync(It.Is<string>(value =>
                            value.IsSameOrEqualTo(tableDeletedTriggerSqlScript))), Times.Once());
                }
            }
        }

        [Fact]
        public async Task InstallAsync_ShouldExecuteSqlScriptForEachTableUpdatedTrigger()
        {
            //Arrange
            _fixture.SetupGenerateTableUpdatedTriggers();
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                foreach (var tableUpdatedTriggerSqlScript in _fixture.TableUpdatedTriggerSqlScripts)
                {
                    _fixture.RepositoryMock.Verify(
                        x => x.ExecuteSqlScriptAsync(It.Is<string>(value =>
                            value.IsSameOrEqualTo(tableUpdatedTriggerSqlScript))), Times.Once());
                }
            }
        }

        [Fact]
        public async Task InstallAsync_NoException_ShouldCommit()
        {
            //Arrange
            var testObject = _fixture.CreateTestObject();

            //Act
            await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                _fixture.RepositoryMock.Verify(x => x.BeginTransactionAsync(), Times.Once());
                _fixture.RepositoryMock.Verify(x => x.CommitTransactionAsync(), Times.Once());
            }
        }

        [Fact]
        public void InstallAsync_Exception_ShouldRethrowAndRollback()
        {
            //Arrange
            _fixture.SetupExceptionGenerateQueue();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.InstallAsync(CancellationToken.None);

            //Assert
            using (new AssertionScope())
            {
                act.Should().Throw<Exception>();
                _fixture.RepositoryMock.Verify(x => x.BeginTransactionAsync(), Times.Once());
                _fixture.RepositoryMock.Verify(x => x.RollbackTransactionAsync(), Times.Once());
            }
        }

        private sealed class Fixture
        {
            private readonly ILogger<SqlBrokerQueueInstallation> _logger;
            private readonly Mock<ISqlBrokerQueueSqlScriptGenerator> _generatorMock;

            private readonly AutoFixture.Fixture _dataGenerationFixture;

            public Fixture(ILogger<SqlBrokerQueueInstallation> logger)
            {
                _logger = logger;
                _generatorMock = new Mock<ISqlBrokerQueueSqlScriptGenerator>();
                RepositoryMock = new Mock<ISqlBrokerQueueSetupRepository>();
                _dataGenerationFixture = new AutoFixture.Fixture();
            }

            public Mock<ISqlBrokerQueueSetupRepository> RepositoryMock { get; }

            public string QueueSqlScript { get; private set; }
            public IEnumerable<string> TableServicesSqlScripts { get; private set; }
            public IEnumerable<string> TableInsertedTriggerSqlScripts { get; private set; }
            public IEnumerable<string> TableDeletedTriggerSqlScripts { get; private set; }
            public IEnumerable<string> TableUpdatedTriggerSqlScripts { get; private set; }

            public SqlBrokerQueueInstallation CreateTestObject()
            {
                return new SqlBrokerQueueInstallation(
                    _logger,
                    _generatorMock.Object,
                    RepositoryMock.Object
                );
            }

            public void SetupGenerateTableService()
            {
                TableServicesSqlScripts = _dataGenerationFixture.CreateMany<string>(1);

                _generatorMock.Setup(x => x.GenerateSetupTableServicesSqlScripts())
                    .ReturnsAsync(TableServicesSqlScripts);
            }

            public void SetupGenerateQueue()
            {
                QueueSqlScript = _dataGenerationFixture.Create<string>();

                _generatorMock.Setup(x => x.GenerateSetupQueueSqlScript())
                    .ReturnsAsync(QueueSqlScript);
            }

            public void SetupGenerateTableInsertedTriggers()
            {
                TableInsertedTriggerSqlScripts = _dataGenerationFixture.CreateMany<string>(1);

                _generatorMock.Setup(x => x.GenerateSetupTableInsertedTriggerSqlScripts())
                    .ReturnsAsync(TableInsertedTriggerSqlScripts);
            }

            public void SetupGenerateTableDeletedTriggers()
            {
                TableDeletedTriggerSqlScripts = _dataGenerationFixture.CreateMany<string>(1);

                _generatorMock.Setup(x => x.GenerateSetupTableDeletedTriggerSqlScripts())
                    .ReturnsAsync(TableDeletedTriggerSqlScripts);
            }

            public void SetupGenerateTableUpdatedTriggers()
            {
                TableUpdatedTriggerSqlScripts = _dataGenerationFixture.CreateMany<string>(1);

                _generatorMock.Setup(x => x.GenerateSetupTableUpdatedTriggerSqlScripts())
                    .ReturnsAsync(TableUpdatedTriggerSqlScripts);
            }

            public void SetupExceptionGenerateQueue()
            {
                _generatorMock.Setup(x => x.GenerateSetupQueueSqlScript())
                    .Throws(new Exception("MyFakeException"));
            }
        }
    }
}