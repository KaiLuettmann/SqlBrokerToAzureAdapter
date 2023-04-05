namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserNameChangedContract
    {
        public int Id { get; set;}

        public UserNameInfoContract NameInfo { get; set;}
    }
}