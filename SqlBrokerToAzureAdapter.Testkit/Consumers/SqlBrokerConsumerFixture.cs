using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues;
using SqlBrokerToAzureAdapter.Consumers.SqlBrokerQueues.Models;

namespace SqlBrokerToAzureAdapter.Testkit.Consumers
{
    public class SqlBrokerConsumerFixture
    {
        private readonly Mock<ISqlBrokerQueueRepository> _sqlBrokerQueueRepositoryMock;
        private int _transactionCounter = 0;

        public SqlBrokerConsumerFixture()
        {
            _sqlBrokerQueueRepositoryMock = new Mock<ISqlBrokerQueueRepository>();
        }

        public void SetupSqlBrokerReturnsSequence(IServiceCollection collection, Action onFinish,
            IEnumerable<BrokerMessage> brokerMessages)
        {
            var brokerMessageList = brokerMessages.ToList();
            var receiveNextMessageSetup = _sqlBrokerQueueRepositoryMock.SetupSequence(x => x.ReceiveNextMessageAsync());
            foreach (var brokerMessage in brokerMessageList)
            {
                receiveNextMessageSetup.Returns(Task.FromResult(brokerMessage));
            }

            _sqlBrokerQueueRepositoryMock.Setup(x => x.CommitTransactionAsync()).Callback(() =>
            {
                _transactionCounter++;
                if (brokerMessageList.Count == _transactionCounter)
                {
                    onFinish.Invoke();
                }
            });

            _sqlBrokerQueueRepositoryMock.Setup(x => x.RollbackTransactionAsync()).Callback(() =>
            {
                _transactionCounter++;
                if (brokerMessageList.Count == _transactionCounter)
                {
                    onFinish.Invoke();
                }
            });

            collection.AddScoped(x => _sqlBrokerQueueRepositoryMock.Object);
        }
    }
}