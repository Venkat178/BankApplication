using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountService
    {
        public Employee AdminLogin(int employeeid, string password);
        public Employee EmployeeLogin(int employeeid, string password);
        public AccountHolder AccountHolderLogin(int userid, string password);
        public APIResponse<string> ResetPassword(AccountHolder accountholder);
    }
}
