using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplication.Models
{
    public class Employee : User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string EmployeeId { get; set; }
        public string Role { get; set; }
        public float Salary { get; set; }
    }
}
