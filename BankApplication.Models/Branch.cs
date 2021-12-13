using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplication.Models
{
    public class Branch
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string IFSCCode { get; set; }

        public string Address { get; set; }

        public bool IsMainBranch { get; set; }

        public string BankId { get; set; }

        public Bank Bank { get; set; }
    }
}
