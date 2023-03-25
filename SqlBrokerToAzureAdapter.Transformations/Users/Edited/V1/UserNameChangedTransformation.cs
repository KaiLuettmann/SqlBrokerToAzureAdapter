using System.Collections.Generic;
using System.Linq;
using SqlBrokerToAzureAdapter.Adapter.Models;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserNameChangedTransformation : IEditEventTransformation<UserContract>
    {
        public bool IsResponsibleFor(IEnumerable<Difference> differences)
        {
            var responsibleMember = new[]
            {
                nameof(UserContract.Lastname),
                nameof(UserContract.NickName),
                nameof(UserContract.Firstname)
            };
            return differences.Any(difference => responsibleMember.Contains(difference.MemberPath));
        }

        public Event Transform(UpdatedPair<UserContract> value)
        {
            return new Event(
                value.NewValue.Id.ToString(),
                new UserNameChangedContract{
                    Id = value.NewValue.Id,
                    NameInfo = new UserNameInfoContract{
                        Firstname = value.NewValue.Firstname,
                        Lastname = value.NewValue.Lastname,
                        Nickname = value.NewValue.NickName
                    }
                }
            );
        }
    }
}