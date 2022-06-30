using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users.Removed.V1
{
    public class UserRemovedTransformation : IRemoveEventTransformation<UserContract>
    {
        public Event Transform(UserContract value)
        {
            return new Event(
                value.Id.ToString(),
            new UserRemovedContract(
                value.Id
                )
            );
        }
    }
}