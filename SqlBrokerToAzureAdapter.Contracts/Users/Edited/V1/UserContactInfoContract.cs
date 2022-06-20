namespace SqlBrokerToAzureAdapter.Users.Edited.V1
{
    public class UserContactInfoContract
    {
        public UserContactInfoContract(string eMail, string phone)
        {
            EMail = eMail;
            Phone = phone;
        }

        public string Phone { get; }

        public string EMail { get; }
    }
}