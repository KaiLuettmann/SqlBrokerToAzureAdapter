using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DockerComposeFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues;
using Xunit;
using Xunit.Abstractions;

namespace SqlBrokerToAzureAdapter.Setup.IntegrationTest.Consumers.SqlBrokerQueues
{
    public class SqlBrokerQueueInstallationTests : IClassFixture<DockerFixture>
    {
        private readonly Fixture _fixture;

        public SqlBrokerQueueInstallationTests(ITestOutputHelper testOutputHelper, DockerFixture dockerFixture)
        {
            _fixture = new Fixture(testOutputHelper, dockerFixture);
        }

        [Fact]
        public void Install_Once_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupSqlServer();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () => await testObject.InstallAsync(CancellationToken.None);

            //Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Install_Twice_ShouldNotThrow()
        {
            //Arrange
            _fixture.SetupSqlServer();
            var testObject = _fixture.CreateTestObject();

            //Act
            Func<Task> act = async () =>
            {
                await testObject.InstallAsync(CancellationToken.None);
                await testObject.InstallAsync(CancellationToken.None);
            };

            //Assert
            act.Should().NotThrow();
        }

        private class Fixture
        {
            private readonly ITestOutputHelper _testOutputHelper;
            private readonly DockerFixture _dockerFixture;

            public Fixture(ITestOutputHelper testOutputHelper, DockerFixture dockerFixture)
            {
                _testOutputHelper = testOutputHelper;
                _dockerFixture = dockerFixture;
            }

            private IConfigurationRoot GetConfiguration()
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("Consumers/SqlBrokerQueues/SqlBrokerQueueInstallationTests.settings.json", true, true);
                return builder.Build();
            }

            private ILoggerFactory CreateLoggerFactory()
            {
                return LoggerFactory.Create(builder => builder.AddXunit(_testOutputHelper));
            }

            public ISqlBrokerQueueInstallation CreateTestObject()
            {
                var loggerFactory = CreateLoggerFactory();
                var serviceCollection = new ServiceCollection()
                    .AddLogging()
                    .AddSingleton(x => loggerFactory)
                    .AddSqlBrokerQueueInstallation(GetConfiguration());

                var provider = serviceCollection.BuildServiceProvider();
                return provider.GetRequiredService<ISqlBrokerQueueInstallation>();
            }

            public void SetupSqlServer()
            {
                _dockerFixture.InitOnce(() => new DockerFixtureOptions
                {
                    DockerComposeFiles = new[] { "Consumers/SqlBrokerQueues/Docker/docker-compose.yml" },
                    DockerComposeUpArgs = "--force-recreate",
                    DockerComposeDownArgs = "--rmi local",
                    CustomUpTest = output => output.Any(l => l.Contains("setup.sql completed"))
                });
            }
        }
    }
}