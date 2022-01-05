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

        public Employee AdminLogin(int employeeid, string password)
        {
            Employee employee = null;
            try
            {
                
                if (BankAppDbctx.Employees.Any(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Admin))
                {
                    employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Admin);

                    return employee;
                }
                return employee;
            }
            catch(Exception)
            {
                return employee;
            }
        }

        public Employee EmployeeLogin(int employeeid, string password)
        {
            Employee employee = null;
            try
            {
                if (BankAppDbctx.Employees.Any(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Employee))
                {
                    employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Employee);

                    return employee;
                }
                return employee;
            }
            catch(Exception)
            {
                return employee;
            }
        }

        public AccountHolder AccountHolderLogin(int userid, string password)
        {
            AccountHolder accountholder = null;
            try
            {
                if (BankAppDbctx.AccountHolders.Any(emp => emp.Id == userid && emp.Password == password))
                {
                    accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(emp => emp.Id == userid && emp.Password == password);

                    return accountholder;
                }
                return accountholder;
            }
            catch(Exception)
            {
                return accountholder;
            }
        }

        public APIResponse<string> ResetPassword(AccountHolder accountholder1)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountholder1.Id);
                if (accountholder != null)
                {
                    accountholder.Password = accountholder1.Password;
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<string> { IsSuccess = true, Message = accountholder.Password };
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
