namespace SqlBrokerToAzureAdapter.Users.Added.V1
{
    public class UserAddedContract
    {
        public int Id { get; set;}

        public UserNameInfoContract NameInfo { get; set;}
    }
}