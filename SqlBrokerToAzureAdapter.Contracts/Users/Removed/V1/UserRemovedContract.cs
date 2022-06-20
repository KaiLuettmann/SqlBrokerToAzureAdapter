namespace SqlBrokerToAzureAdapter.Users.Removed.V1
{
    public class UserRemovedContract
    {
        public UserRemovedContract(int id)
        {
            Id = id;
        }
        public int Id { get; }
    }
}