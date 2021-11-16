using System;

namespace BankApplication.Models
{
    public class Transaction
    {
        public string Id { set; get; }
        public string SrcAccId { set; get; }
        public string DestAccId { set; get; }
        public double Amount { set; get; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { set; get; }
        public TransactionType Type { set; get; }
    }
}

