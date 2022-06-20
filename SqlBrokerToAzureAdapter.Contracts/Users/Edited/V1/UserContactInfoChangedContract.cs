namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserContactInfoChangedContract
    {
        public UserContactInfoChangedContract(int id,
            UserContactInfoContract contactInfoContract)
        {
            Id = id;
            ContactInfoContract = contactInfoContract;
        }

        public UserContactInfoContract ContactInfoContract { get; }

        public int Id { get; }
    }
}