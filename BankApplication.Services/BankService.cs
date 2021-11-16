using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    public class BankService
    {
        public Status SetUpBank(Bank bank)
        {
            Status status = new Status();
            bank.Id = bank.BankName + DateTime.Now.ToString("yyyyMMddHHmmss");
            bank.CurrencyCode = "INR";
            BankDatabase.Banks.Add(bank);
            status.IsSuccess = true;
            return status;
        }


        public string Register(Bank bank,BankAccount bankaccount)
        {
            if (BankDatabase.BankAccounts.Count != 0 && BankDatabase.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                throw new Exception("Account already exists!");
            }
            bankaccount.Type = UserType.AccountHolder;
            bank.BankAccounts.Add(bankaccount);
            BankDatabase.BankAccounts.Add(bankaccount); 
            return bankaccount.Id;
        }

        public Admin AdminLogin(string userid, string password)
        {
            Admin user = null;
            Admin admin = BankDatabase.Admins.Find(admin => admin.Id == userid && admin.Password == password);
            if (admin != null)
            {
                user = admin;
            }
            return user;
        }

        public BankAccount Login(string userid,string password)
        {
            BankAccount user = null;
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id ==userid && bankaccount.Password == password);
            if(bankaccount!=null)
            {
                user = bankaccount;
            }
            return user;
        }

        public Employee EmployeeLogin(string userid,string password)
        {
            Employee user = null;
            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == userid && employee.Password == password);
            if (employee != null)
            {
                user = employee;
            }
            return user;
        }


        public string Deposit(BankAccount bankaccount,double amt)
        {
            string txnid = "TXN";
            bankaccount.Balance += amt;
            Transaction trans = new Transaction(bankaccount.Id, bankaccount.Id, amt, bankaccount.BankId, TransactionType.Credited);
            bankaccount.Transactions.Add(trans);
            txnid = trans.Id;
            return txnid;
        }

        public string Withdraw (BankAccount bankaccount, double amt)
        {
            string txnid = "TXN";
            if (bankaccount.Balance >= amt)
            {
                bankaccount.Balance -= amt;
                Transaction trans = new Transaction(bankaccount.Id, bankaccount.Id, amt, bankaccount.BankId, TransactionType.Debited);
                bankaccount.Transactions.Add(trans);
                txnid = trans.Id;
            }
            else
            {
                throw new OutofMoneyException("You do not have sufficient money");
            }
            return txnid;
        }


        public string Transfer(BankAccount bankaccount, string recuserId, double amt)
        {
            string txnid = "TXN";
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankacount => bankaccount.Id == recuserId);
            if (bankaccount.Balance >= amt)
            {
                bankaccount.Balance -= amt;
                recevierbankaccount.Balance += amt;
                Transaction trans = new Transaction(bankaccount.Id, recuserId, amt, bankaccount.BankId, TransactionType.Debited);
                Transaction trans1 = new Transaction(bankaccount.Id, recuserId, amt, bankaccount.BankId, TransactionType.Credited);
                bankaccount.Transactions.Add(trans);
                recevierbankaccount.Transactions.Add(trans1);
                txnid = trans.Id;
            }
            else
            {
                throw new OutofMoneyException("You do not have sufficient money");
            }
            return txnid;
        }

        public double ViewBalance(BankAccount bankaccount)
        {
            return bankaccount.Balance;
        }

        public void ViewTransactions(BankAccount bankaccount)
        {
            foreach (var i in bankaccount.Transactions)
            {
                Console.WriteLine(i.SenderAccountId + " to " + i.RecieverAccountId + " of " + i.Amount);
            }
        }

        /*public void logout()
        {
            BankAccount b = null;
        }*/

        public void ViewAllBankBranches()
        {
            foreach(var i in BankDatabase.Banks)
            {
                Console.WriteLine(i.BranchName);
            }
        }

    }
}


