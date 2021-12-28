
namespace BankApplication.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public GenderType Gender { set; get; }
        public long PhoneNumber { get; set; }
        public string Address { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
        public Bank Bank { get; set; }
    }
}