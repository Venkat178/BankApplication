using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankApplication.Models
{
    public class CurrencyCode
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; }

        public double ExchangeRate { get; set; }

        public bool IsDefault { get; set; }

        public string BankId { get; set; }

        public Bank Bank { get; set; }
    }
}
