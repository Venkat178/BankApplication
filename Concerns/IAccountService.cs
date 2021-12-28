using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountService
    {
        public Employee AdminLogin(int employeeid, string password);
        public Employee EmployeeLogin(int employeeid, string password);
        public AccountHolder Login(int userid, string password);
        public string ResetPassword(int userid);
    }
}
