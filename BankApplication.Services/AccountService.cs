using System;
using BankApplication.Models;
using BankApplication.Concerns;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountService : IAccountService
    {
        public BankApplicationDbContext BankAppDbctx;

        public AccountService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public User Login(int userid, string password)
        {
            //Employee employee = null;
            try
            {
                if (BankAppDbctx.Employees.Any(emp => emp.Id == userid && emp.Password == password && emp.Type == UserType.Admin))
                {
                    Employee employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == userid && emp.Password == password && emp.Type == UserType.Admin);

                    return employee;
                }
                if (BankAppDbctx.Employees.Any(emp => emp.Id == userid && emp.Password == password && emp.Type == UserType.Employee))
                {
                    Employee employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == userid && emp.Password == password && emp.Type == UserType.Employee);

                    return employee;
                }
                if (BankAppDbctx.AccountHolders.Any(emp => emp.Id == userid && emp.Password == password))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(emp => emp.Id == userid && emp.Password == password);

                    return accountholder;
                }
                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public APIResponse<string> ResetPassword(int accountid,string password)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountid);
                if (accountholder != null)
                {
                    accountholder.Password = password;
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<string> { IsSuccess = true, Message = "Password reset successfully" };
                }
                else
                {
                    return new APIResponse<string> { IsSuccess = false, Message = "Accountholder not found" };
                }
            }
            catch(Exception)
            {
                return new APIResponse<string> { IsSuccess = false, Message = "Error occured" };
            }
        }
    }
}
