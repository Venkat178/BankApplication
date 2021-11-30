using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountService
    {
        public Employee AdminLogin(string employeeid, string password)
        {
            Employee user = null;
            Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee=> employee.Id == employeeid && employee.Type == UserType.Admin));
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
            Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid));
            Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
            if (employee != null)
            {
                user = employee;
            }
            return user;
        }

        public BankAccount Login(string userid, string password)
        {
            BankAccount user = null;
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid && bankaccount.Password == password);
            if (bankaccount != null)
            {
                user = bankaccount;
            }
            return user;
        }

        public string ResetPassword(string userid)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
            while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }
            bankaccount.Password = password;
            return bankaccount.Password;
        }
    }
}
