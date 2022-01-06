using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountService
    {
        public User Login(int userid, string password);
        public APIResponse<string> ResetPassword(int accountid,string password);
    }
}
