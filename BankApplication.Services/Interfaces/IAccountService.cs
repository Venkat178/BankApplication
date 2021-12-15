using BankApplication.Models;

namespace BankApplication.Services.Interfaces
{
    public interface IAccountService
    {
        public Employee AdminLogin(string employeeid, string password);
        public Employee EmployeeLogin(string employeeid, string password);
        public AccountHolder Login(string userid, string password);
        public string ResetPassword(string userid);
    }
}
