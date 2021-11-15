using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public class BankAccount
    {
        public string BankId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string BranchName { get; set; }
        public string Id { get; set; }
        public double Balance { get; set; }
        public GenderType Gender { set; get; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public UserType Type { get; set; }

        public List<Transaction> Transactions = new List<Transaction>();
    }
}


