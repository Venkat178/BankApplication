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
            Employee user = null;
            Employee admin = BankAppDbctx.Employees.FirstOrDefault(employee => employee.Id == employeeid);
            if (admin != null)
            {
                user = admin;
            }
            return user;
        }

        public Employee EmployeeLogin(int employeeid, string password)
        {
            Employee user = null;
            Employee employee = BankAppDbctx.Employees.FirstOrDefault(employee => employee.Id == employeeid);
            if (employee != null)
            {
                user = employee;
            }
            return user;
        }

        public AccountHolder Login(int userid, string password)
        {
            AccountHolder user = null;
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
            if (accountholder != null)
            {
                user = accountholder;
            }
            return user;
        }

        public string ResetPassword(int userid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
            string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
            while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }
            accountholder.Password = password;
            BankAppDbctx.SaveChanges();
            return accountholder.Password;
        }
    }
}
