using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    internal class SqlBrokerQueueSetupRepository : ISqlBrokerQueueSetupRepository
    {
        private readonly ILogger<SqlBrokerQueueSetupRepository> _logger;
        private readonly ISqlServerInstallationConnectionConfiguration _sqlServerConnectionConnectionConfiguration;
        private SqlConnection _sqlConnection;
        private SqlTransaction _transaction;

        public SqlBrokerQueueSetupRepository(
            ILogger<SqlBrokerQueueSetupRepository> logger,
            ISqlServerInstallationConnectionConfiguration sqlServerConnectionConnectionConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sqlServerConnectionConnectionConfiguration = sqlServerConnectionConnectionConfiguration ?? throw new ArgumentNullException(nameof(sqlServerConnectionConnectionConfiguration));
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null) await RollbackTransactionAsync();

            await _sqlConnection.DisposeAsync();
            _sqlConnection = null;
        }

        public async Task CloseSqlConnectionAsync()
        {
            await _sqlConnection.CloseAsync();
            await _sqlConnection.DisposeAsync();
            _sqlConnection = null;
        }

        public async Task ExecuteSqlScriptAsync(string sqlScript)
        {
            await using var command = new SqlCommand(sqlScript, _sqlConnection, _transaction);

            _logger.LogTrace(command.CommandText);
            await command.ExecuteNonQueryAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction before was not commited or rollbacked");

            _transaction = (SqlTransaction) await _sqlConnection.BeginTransactionAsync(IsolationLevel.Serializable);
        }

        public async Task OpenSqlConnectionAsync()
        {
            _sqlConnection = new SqlConnection(_sqlServerConnectionConnectionConfiguration.ConnectionString);
            await _sqlConnection.OpenAsync();
        }
    }
}