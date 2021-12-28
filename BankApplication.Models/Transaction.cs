
namespace BankApplication.Models
{
    public class Transaction
    {
        public int Id { set; get; }
        public string TransactionId { get; set; }
        public int SrcAccId { set; get; }
        public int DestAccId { set; get; }
        public double Amount { set; get; }
        public int CreatedBy { get; set; }
        public string CreatedOn { set; get; }
        public TransactionType Type { set; get; }
    }
}

