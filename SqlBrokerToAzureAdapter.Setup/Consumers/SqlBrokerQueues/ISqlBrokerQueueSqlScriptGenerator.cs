using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// Generates sql scripts for the installation
    /// </summary>
    public interface ISqlBrokerQueueSqlScriptGenerator
    {
        /// <summary>
        /// Generates the sql script for creating the queue.
        /// </summary>
        /// <returns>The sql script.</returns>
        Task<string> GenerateSetupQueueSqlScript();

        /// <summary>
        /// Generates the sql script for creating the services of all configured tables.
        /// </summary>
        /// <returns>The sql script.</returns>
        Task<IEnumerable<string>> GenerateSetupTableServicesSqlScripts();

        /// <summary>
        /// Generates the sql script for creating the delete trigger of all configured tables.
        /// </summary>
        /// <returns>The sql script.</returns>
        Task<IEnumerable<string>> GenerateSetupTableDeletedTriggerSqlScripts();

        /// <summary>
        /// Generates the sql script for creating the insert trigger of all configured tables.
        /// </summary>
        /// <returns>The sql script.</returns>
        Task<IEnumerable<string>> GenerateSetupTableInsertedTriggerSqlScripts();

        /// <summary>
        /// Generates the sql script for creating the update trigger of all configured tables.
        /// </summary>
        /// <returns>The sql script.</returns>
        Task<IEnumerable<string>> GenerateSetupTableUpdatedTriggerSqlScripts();
    }
}