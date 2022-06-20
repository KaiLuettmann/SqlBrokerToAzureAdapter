using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues
{
    internal sealed class SqlBrokerQueueRepository : ISqlBrokerQueueRepository
    {
        private readonly ILogger<SqlBrokerQueueRepository> _logger;
        private readonly ISqlBrokerQueueConfiguration _sqlBrokerQueueConfiguration;
        private int _skippedBrokerMessageJustifiedOfErrors;
        private SqlConnection _sqlConnection;
        private SqlTransaction _transaction;

        public SqlBrokerQueueRepository(
            ILogger<SqlBrokerQueueRepository> logger,
            ISqlBrokerQueueConfiguration sqlBrokerQueueConfiguration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sqlBrokerQueueConfiguration = sqlBrokerQueueConfiguration ?? throw new ArgumentNullException(nameof(sqlBrokerQueueConfiguration));
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null) await RollbackTransactionAsync();

            await _sqlConnection.DisposeAsync();
            _sqlConnection = null;
        }

        public Task SkipCurrentBrokerMessageAsync()
        {
            _skippedBrokerMessageJustifiedOfErrors++;

            return Task.CompletedTask;
        }

        public async Task CloseSqlConnectionAsync()
        {
            await _sqlConnection.CloseAsync();
            await _sqlConnection.DisposeAsync();
            _sqlConnection = null;
        }

        public Task ResetSkippedBrokerMessagesAsync()
        {
            if (_skippedBrokerMessageJustifiedOfErrors != 0)
                _logger.LogInformation("reset erroneous messages so that they are processed again.");
            _skippedBrokerMessageJustifiedOfErrors = 0;

            return Task.CompletedTask;
        }

        public async Task EndConversationAsync(Guid conversationHandle)
        {
            var sb = new StringBuilder();

            sb.Append("END CONVERSATION @conversation_handle ;");
            var sql = sb.ToString();

            await using var command = new SqlCommand(sql, _sqlConnection, _transaction);
            command.Parameters.Add("@conversation_handle", SqlDbType.UniqueIdentifier);
            command.Parameters["@conversation_handle"].Value = conversationHandle;

            _logger.LogTrace(command.CommandText);
            var effectedRows = await command.ExecuteNonQueryAsync();
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

            _transaction = (SqlTransaction) await _sqlConnection.BeginTransactionAsync();
        }

        public async Task OpenSqlConnectionAsync()
        {
            _sqlConnection = new SqlConnection(_sqlBrokerQueueConfiguration.ConnectionString);
            await _sqlConnection.OpenAsync();
        }

        public async Task<BrokerMessage> ReceiveNextMessageAsync()
        {
            var sb = new StringBuilder();

            sb.Append("WAITFOR (  ");
            sb.Append(
                "receive top (@top) conversation_handle, message_enqueue_time, message_type_name, CAST(message_body AS NVARCHAR(MAX)) message_body ");
            sb.Append(
                $"FROM [{_sqlBrokerQueueConfiguration.DatabaseName}].[{_sqlBrokerQueueConfiguration.SchemaName}].[{_sqlBrokerQueueConfiguration.ReceiverQueueName}] ");
            sb.Append($"),  TIMEOUT @longPollingTimeout");
            var sql = sb.ToString();

            var top = _skippedBrokerMessageJustifiedOfErrors + 1;
            await using var command = new SqlCommand(sql, _sqlConnection) {Transaction = _transaction};
            command.Parameters.Add("@top", SqlDbType.Int);
            command.Parameters["@top"].Value = top;

            command.Parameters.Add("@longPollingTimeout", SqlDbType.Int);
            command.Parameters["@longPollingTimeout"].Value = _sqlBrokerQueueConfiguration.LongPollingTimeout;

            _logger.LogTrace(command.CommandText);
            await using var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                var readRows = 0;
                while (reader.Read())
                {
                    if (readRows != _skippedBrokerMessageJustifiedOfErrors)
                    {
                        readRows++;
                        continue;
                    }

                    var brokerMessage = new BrokerMessage(
                        reader.GetGuid(0),
                        reader.GetDateTime(1),
                        reader.GetString(2),
                        reader.GetString(3)
                    );

                    _logger.LogDebug($"Message received: {brokerMessage}");

                    return brokerMessage;
                }
            }

            _logger.LogDebug(
                $"No message received in {_sqlBrokerQueueConfiguration.LongPollingTimeout} milliseconds configured with parameter '{nameof(SqlBrokerQueueConfiguration.LongPollingTimeout)}'.");

            await reader.CloseAsync();
            return null;
        }
    }
}