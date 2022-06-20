using System.Threading;
using System.Threading.Tasks;

namespace SqlBrokerToAzureAdapter.Setup.Consumers.SqlBrokerQueues
{
    /// <summary>
    /// The installation of the sql broker queue
    /// </summary>
    public interface ISqlBrokerQueueInstallation
    {
        /// <summary>
        /// Installs the sql broker queue for the configured tables
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InstallAsync(CancellationToken cancellationToken);
    }
}