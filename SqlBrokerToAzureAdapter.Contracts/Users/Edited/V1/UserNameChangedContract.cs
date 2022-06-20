namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserNameChangedContract
    {
        public UserNameChangedContract(int id,
            UserNameInfoContract nameInfo)
        {
            Id = id;
            NameInfo = nameInfo;
        }
        public int Id { get; }

        public UserNameInfoContract NameInfo { get; }
    }
}