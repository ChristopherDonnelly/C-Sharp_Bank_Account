using System.Collections.Generic;

namespace Bank_Accounts.Models
{
    public class AccountBundle
    {
        public User user { get; set; }
        public Transaction transaction { get; set; }
    }
}