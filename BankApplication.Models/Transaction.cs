using System;

namespace BankApplication.Models
{
    public class Transaction
    {
        public string Id { set; get; }
        public string SenderAccountId { set; get; }
        public string RecieverAccountId { set; get; }
        public double Amount { set; get; }
        public DateTime Time { set; get; }
        public TransactionType Type { set; get; }

        public Transaction(string senderId, string recieverId, double amt, string bankId, TransactionType type)
        {
            Id = "TXN" + senderId + recieverId + bankId + DateTime.Now.ToString("yyyyMMddHHmmss");
            Type = type;
            SenderAccountId = senderId;
            RecieverAccountId = recieverId;
            Amount = amt;
            Time = DateTime.Now;
        }

    }
}

