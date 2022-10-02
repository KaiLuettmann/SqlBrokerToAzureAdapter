using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    internal class SqlBrokerQueueSqlScriptGenerator : ISqlBrokerQueueSqlScriptGenerator
    {
        internal const string PlaceholderWrapper = "__";
        static readonly string TemplatePath = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/Consumers/SqlBrokerQueues/SqlTemplates/");
        private readonly ILogger<SqlBrokerQueueSqlScriptGenerator> _logger;
        private readonly ISqlBrokerQueueGenerationConfiguration _configuration;

        public SqlBrokerQueueSqlScriptGenerator(
            ILogger<SqlBrokerQueueSqlScriptGenerator> logger,
            ISqlBrokerQueueGenerationConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> GenerateSetupQueueSqlScript()
        {
            var template = await File.ReadAllTextAsync($"{TemplatePath}SetupQueueTemplate.sql");
            return ReplaceQueuePlaceholders(template);
        }

        private string ReplaceQueuePlaceholders(string template)
        {
            _logger.LogInformation($"Start generating sql script for creating queues '{_configuration.SenderQueueName}' and '{_configuration.ReceiverQueueName}'.");
            var result = template
                .Replace(GetReplacingValue(nameof(_configuration.DatabaseName)), _configuration.DatabaseName)
                .Replace(GetReplacingValue(nameof(_configuration.SchemaName)), _configuration.SchemaName)
                .Replace(GetReplacingValue(nameof(_configuration.ReceiverQueueName)), _configuration.ReceiverQueueName)
                .Replace(GetReplacingValue(nameof(_configuration.SenderQueueName)), _configuration.SenderQueueName);
            _logger.LogInformation($"Sql script for creating a queues '{_configuration.SenderQueueName}', '{_configuration.ReceiverQueueName}' generated successfully.");
            return result;
        }

        public async Task<IEnumerable<string>> GenerateSetupTableServicesSqlScripts()
        {
            var template = await File.ReadAllTextAsync($"{TemplatePath}SetupTableServicesTemplate.sql");
            var results = ReplacePlaceholdersForEachTableSubscription(template)
                .ToList();

            foreach (var result in results)
            {
                _logger.LogTrace(result);
            }

            return results;
        }

        public async Task<IEnumerable<string>> GenerateSetupTableDeletedTriggerSqlScripts()
        {
            var template = await File.ReadAllTextAsync($"{TemplatePath}SetupTableDeletedTriggerTemplate.sql");
            return ReplacePlaceholdersForEachTableSubscription(template);
        }

        public async Task<IEnumerable<string>> GenerateSetupTableInsertedTriggerSqlScripts()
        {
            var template = await File.ReadAllTextAsync($"{TemplatePath}SetupTableInsertedTriggerTemplate.sql");
            return ReplacePlaceholdersForEachTableSubscription(template);
        }

        public async Task<IEnumerable<string>> GenerateSetupTableUpdatedTriggerSqlScripts()
        {
            var template = await File.ReadAllTextAsync($"{TemplatePath}SetupTableUpdatedTriggerTemplate.sql");
            return ReplacePlaceholdersForEachTableSubscription(template);
        }

        private IEnumerable<string> ReplacePlaceholdersForEachTableSubscription(string template)
        {
            foreach (var tableSubscription in _configuration.TableSubscriptions)
            {
                _logger.LogInformation($"Generating sql script for subscription to table {tableSubscription.TableName}...");
                var result = template
                    .Replace(GetReplacingValue(nameof(_configuration.DatabaseName)), _configuration.DatabaseName)
                    .Replace(GetReplacingValue(nameof(_configuration.SchemaName)), _configuration.SchemaName)
                    .Replace(GetReplacingValue(nameof(_configuration.ReceiverQueueName)), _configuration.ReceiverQueueName)
                    .Replace(GetReplacingValue(nameof(_configuration.SenderQueueName)), _configuration.SenderQueueName)
                    .Replace(GetReplacingValue(nameof(tableSubscription.TableName)), tableSubscription.TableName)
                    .Replace(GetReplacingValue(nameof(tableSubscription.PrimaryKeyColumnName)), tableSubscription.PrimaryKeyColumnName)
                    .Replace(GetReplacingValue(nameof(tableSubscription.DeleteBrokerMessageTypeName)), tableSubscription.DeleteBrokerMessageTypeName)
                    .Replace(GetReplacingValue(nameof(tableSubscription.InsertBrokerMessageTypeName)), tableSubscription.InsertBrokerMessageTypeName)
                    .Replace(GetReplacingValue(nameof(tableSubscription.UpdateBrokerMessageTypeName)), tableSubscription.UpdateBrokerMessageTypeName);
                _logger.LogInformation($"Sql script for subscription to table {tableSubscription.TableName} generated successfully.");
                yield return result;
            }
        }

        private static string GetReplacingValue(string propertyName)
        {
            return $"{PlaceholderWrapper}{propertyName}{PlaceholderWrapper}";
        }
    }
}