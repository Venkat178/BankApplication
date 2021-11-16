namespace BankApplication.Models
{
    public class Employee : User
    {
        public string EmployeeId { get; set; }

        public string Role { get; set; }

        public int Salary { get; set; }
    }
}
