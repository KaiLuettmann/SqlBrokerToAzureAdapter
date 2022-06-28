using System.Collections.Generic;
using System.Linq;
using SqlBrokerToAzureAdapter.MessageContracts;
using SqlBrokerToAzureAdapter.Producers.Common.Models;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserContactInfoChangedTransformation : IEditEventTransformation<UserContract>
    {
        public bool IsResponsibleFor(IEnumerable<Difference> differences)
        {
            var responsibleMember = new[]
            {
                nameof(UserContract.EMail),
                nameof(UserContract.Phone)
            };
            return differences.Any(difference => responsibleMember.Contains(difference.MemberPath));
        }

        public Event Transform(UpdatedPair<UserContract> value)
        {
            return new Event(
                value.NewValue.Id.ToString(),
                new UserContactInfoChangedContract(
                    value.NewValue.Id,
                    new UserContactInfoContract(value.NewValue.EMail, value.NewValue.Phone)
                    )
                );
        }
    }
}