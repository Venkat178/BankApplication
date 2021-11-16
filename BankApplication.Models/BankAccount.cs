using System.Collections.Generic;

namespace BankApplication.Models
{
    public class BankAccount : User
    {
        public double Balance { get; set; }

        public List<Transaction> Transactions = new List<Transaction>();
    }
}


