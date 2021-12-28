namespace BankApplication.Models
{
    public class CurrencyCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public double ExchangeRate { get; set; }

        public bool IsDefault { get; set; }

        public int BankId { get; set; }

        public Bank Bank { get; set; }
    }
}
