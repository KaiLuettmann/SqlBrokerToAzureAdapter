using SqlBrokerToAzureAdapter.Producers.AzureTopics.Models;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users.Added.V1
{
    public class UserAddedTransformation : IAddEventTransformation<UserContract>
    {
        public Event Transform(UserContract value)
        {
            return new Event(
                value.Id.ToString(),
        new UserAddedContract(
                    value.Id,
                    new UserNameInfoContract(value.Firstname, value.Lastname, value.NickName))
            );
        }
    }
}