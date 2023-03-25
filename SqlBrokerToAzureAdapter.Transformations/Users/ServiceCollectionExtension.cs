using Microsoft.Extensions.DependencyInjection;
using SqlBrokerToAzureAdapter.Transformations;
using SqlBrokerToAzureAdapter.Users.Added.V1;
using SqlBrokerToAzureAdapter.Users.Edited.V1;
using SqlBrokerToAzureAdapter.Users.Removed.V1;
using SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts;

namespace SqlBrokerToAzureAdapter.Users
{
    public static class ServiceCollectionExtension
    {
        public static void AddUsers(this IServiceCollection collection)
        {
            collection.AddScoped<IAddEventTransformations<UserContract>>(_ => new AddEventTransformations<UserContract>
            {
                new UserAddedTransformation()
            });
            collection.AddScoped<IRemoveEventTransformations<UserContract>>(_ => new RemoveEventTransformations<UserContract>
            {
                new UserRemovedTransformation()
            });
            collection.AddScoped<IEditEventTransformations<UserContract>>(_ => new EditEventTransformations<UserContract>
            {
                new UserNameChangedTransformation(),
                new UserContactInfoChangedTransformation()
            });
            collection.AddSqlBrokerToAzureAdapter<UserContract>();
        }
    }
}