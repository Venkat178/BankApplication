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
            if (BankAppDbctx.Employees.Any(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Admin))
            {
                employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Admin);
                return employee;
            }
            return employee;
        }

        public Employee EmployeeLogin(int employeeid, string password)
        {
            Employee employee = null;
            if (BankAppDbctx.Employees.Any(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Employee))
            {
                employee = BankAppDbctx.Employees.FirstOrDefault(emp => emp.Id == employeeid && emp.Password == password && emp.Type == UserType.Employee);
                return employee;
            }
            return employee;
        }

        public AccountHolder AccountHolderLogin(int userid, string password)
        {
            AccountHolder accountholder = null;
            if (BankAppDbctx.AccountHolders.Any(emp => emp.Id == userid && emp.Password == password))
            {
                accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(emp => emp.Id == userid && emp.Password == password);
                return accountholder;
            }
            return accountholder;
        }

        public APIResponse ResetPassword(int userid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
            if (accountholder != null)
            {
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                accountholder.Password = password;
                BankAppDbctx.SaveChanges();
                return new APIResponse { IsSuccess =true ,Message = accountholder.Password};
            }
            else
            {
                return new APIResponse { IsSuccess = false, Message = "Accountholder not found" };
            }
            
        }
    }
}
