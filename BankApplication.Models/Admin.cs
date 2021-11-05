using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Models
{
    public class Admin
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string BankId { get; set; }
        public string BranchName { get; set; }
        public GenderType Gender { get; set; }
        public string PhoneNumber { get; set; }
        public EnumHolderType Type { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
    }
}
