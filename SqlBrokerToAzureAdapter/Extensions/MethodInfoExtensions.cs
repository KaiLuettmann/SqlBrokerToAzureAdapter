using System.Reflection;
using System.Threading.Tasks;

namespace SqlBrokerToAzureAdapter.Extensions
{
    internal static class MethodInfoExtensions
    {
        internal static async Task InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task) @this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
        }
    }
}
