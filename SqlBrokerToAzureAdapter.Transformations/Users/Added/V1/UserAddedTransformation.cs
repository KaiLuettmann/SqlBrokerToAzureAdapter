using SqlBrokerToAzureAdapter.Adapter.Models;
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
                new UserAddedContract {
                    Id = value.Id,
                    NameInfo = new UserNameInfoContract {
                        Firstname = value.Firstname,
                        Lastname = value.Lastname,
                        Nickname = value.NickName
                    }
                });
        }
    }
}