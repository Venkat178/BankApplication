using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    public class BankService
    {
        public Status SetUpBank(Branch branch,Bank bank, Employee admin)
        {
            Status status = new Status();
            try
            {

                bank.Id = bank.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "INR", ExchangeRate = 1 });

                branch.Id = branch.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                branch.IsMainBranch = true;

                admin.Id = bank.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                admin.Role = "Admin";
                bank.Admins.Add(admin);

                BankDatabase.Banks.Add(bank);
                bank.Branches.Add(branch);
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.IsSuccess = false;
                status.Message = "Unable to create a bank...!";
            }

            return status;
        }

        public string Register(Bank bank, BankAccount bankaccount)
        {
            if (BankDatabase.BankAccounts.Count != 0 && BankDatabase.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                throw new Exception("Account already exists!");
            }
            bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            bankaccount.Type = UserType.AccountHolder;
            bank.BankAccounts.Add(bankaccount);
            BankDatabase.BankAccounts.Add(bankaccount);
            return bankaccount.Id;
        }

        public string EmployeeRegister(Employee employee,Bank bank)
        {
            employee.Type = UserType.Employee;
            employee.Id = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            bank.Employees.Add(employee);
            return employee.Id;
        }
        public string Deposit(BankAccount bankaccount, double amt)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            bankaccount.Balance += amt;
            transaction.SrcAccId = bankaccount.Id;
            transaction.DestAccId = bankaccount.Id;
            transaction.Amount = amt;
            transaction.CreatedBy = bankaccount.Id;
            transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
            transaction.Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
            transaction.Type = TransactionType.Credit;
            bankaccount.Transactions.Add(transaction);
            txnid = transaction.Id;
            return txnid;
        }

        public string Withdraw(BankAccount bankaccount, double amt)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (bankaccount.Balance >= amt)
            {
                bankaccount.Balance -= amt;
                transaction.SrcAccId = bankaccount.Id;
                transaction.DestAccId = bankaccount.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = bankaccount.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Debit;
                bankaccount.Transactions.Add(transaction);
                txnid = transaction.Id;
            }
            else
            {
                Console.WriteLine("You do not have sufficient money");
            }
            return txnid;
        }


        public string Transfer(BankAccount bankaccount, string destuserId, double amt)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == destuserId);
            if(recevierbankaccount!=null)
            {
                if (bankaccount.Balance >= amt)
                {
                    bankaccount.Balance -= amt;
                    recevierbankaccount.Balance += amt;
                    transaction.SrcAccId = bankaccount.Id;
                    transaction.DestAccId = destuserId;
                    transaction.Amount = amt;
                    transaction.CreatedBy = bankaccount.Id;
                    transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                    transaction.Id = "TXN" + bankaccount.Id + destuserId + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                    transaction.Type = TransactionType.Transfer;
                    bankaccount.Transactions.Add(transaction);
                    recevierbankaccount.Transactions.Add(transaction);
                    txnid = transaction.Id;
                }
                else
                {
                    Console.WriteLine("You do not have sufficient money");
                }
            }
            return txnid;
        }

        public double ViewBalance(BankAccount bankaccount)
        {
            return bankaccount.Balance;
        }

        public void ViewAllBankBranches(string Bankid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == Bankid);
            foreach(var i in bank.Branches)
            {
                Console.WriteLine(i.Id + "  -  " + i.Name);
            }
        }

        public void ViewAllAccounts()
        {
            foreach(var i in BankDatabase.BankAccounts)
            {
                Console.WriteLine(i.Id + "   -   " + i.Name+"   -   "+i.PhoneNumber+"   -   "+i.Address);
            }
        }

        public void ViewAllEmployees(string bankid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            foreach(var i in bank.Employees)
            {
                Console.WriteLine(i.Id + "   -   " + i.Name + "   -   " + i.PhoneNumber + "   -   " + i.Address);
            }
        }
    }
}


