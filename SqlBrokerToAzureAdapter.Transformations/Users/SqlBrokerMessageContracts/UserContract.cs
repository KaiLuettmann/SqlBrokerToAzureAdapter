namespace SqlBrokerToAzureAdapter.Users.SqlBrokerMessageContracts
{
    public class UserContract
    {
        public int Id { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string NickName { get; set; }
    }
}