namespace SqlBrokerToAzureAdapter.Users.Added.V1
{
    public class UserNameInfoContract
    {
        public UserNameInfoContract(string firstname, string lastname, string nickname)
        {
            Firstname = firstname;
            Lastname = lastname;
            Nickname = nickname;
        }

        public string Firstname { get; }

        public string Lastname { get; }

        public string Nickname { get; }
    }
}