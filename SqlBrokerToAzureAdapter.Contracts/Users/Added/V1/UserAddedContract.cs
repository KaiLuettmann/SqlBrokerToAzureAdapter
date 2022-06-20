namespace SqlBrokerToAzureAdapter.Users.Added.V1
{
    public class UserAddedContract
    {
        public UserAddedContract(int id,
            UserNameInfoContract nameInfo)
        {
            Id = id;
            NameInfo = nameInfo;
        }
        public int Id { get; }

        public UserNameInfoContract NameInfo { get; }
    }
}