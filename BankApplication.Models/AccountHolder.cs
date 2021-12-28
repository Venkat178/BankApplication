using System.Collections.Generic;

namespace BankApplication.Models
{
    public class AccountHolder : User
    {
        public AccountHolder()
        {
            this.Transactions = new List<Transaction>();
        }
        public double Balance { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}


