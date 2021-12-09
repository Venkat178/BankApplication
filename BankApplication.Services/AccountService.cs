using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountService
    {
        public BankApplicationDbContext BankAppDbctx;

        public AccountService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public Employee AdminLogin(string employeeid, string password)
        {
            Employee user = null;
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(b => b.Employees.Any(employee=> employee.Id == employeeid && employee.Type == UserType.Admin));
            Employee admin = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
            if (admin != null)
            {
                user = admin;
            }
            return user;
        }

        public Employee EmployeeLogin(string employeeid, string password)
        {
            Employee user = null;
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(b => b.Employees.Any(employee => employee.Id == employeeid));
            Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
            if (employee != null)
            {
                user = employee;
            }
            return user;
        }

        public AccountHolder Login(string userid, string password)
        {
            AccountHolder user = null;
            Bank bank2 = BankAppDbctx.Banks.FirstOrDefault(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank2 != null ? bank2.AccountHolders.Find(account => account.Id == userid) : null;
            if (accountholder != null)
            {
                user = accountholder;
            }
            return user;
        }

        public string ResetPassword(string userid)
        {
            Bank bank2 = BankAppDbctx.Banks.FirstOrDefault(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank2 != null ? bank2.AccountHolders.Find(account => account.Id == userid) : null;
            string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
            while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }
            accountholder.Password = password;
            return accountholder.Password;
        }
    }
}
