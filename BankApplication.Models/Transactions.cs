using System;

namespace BankApplication.Models
{
    public class Transactions
    {
        public string TransactionId { set; get; }
        public string SenderAccountId { set; get; }
        public string RecieverAccountId { set; get; }
        public double Amount { set; get; }
        public DateTime Time { set; get; }
        public TransactionType Type { set; get; }

        public Transactions(string senderId, string recieverId, double amt, string bankId, TransactionType type)
        {
            TransactionId = "TXN" + senderId + recieverId + bankId + DateTime.Now.ToString("yyyyMMddHHmmss");
            Type = type;
            SenderAccountId = senderId;
            RecieverAccountId = recieverId;
            Amount = amt;
            Time = DateTime.Now;
        }

    }
}

