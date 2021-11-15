using System;
using System.Collections.Generic;
using System.Linq;
using BankApplication.Models;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    public class AccountService
    {
        public void UpdateEmployeeName(string userid, string EmployeeName)
        {
            Employee employee= BankDatabase.Employees.Find(employee => employee.Id == userid);
            if (employee!=null)
            {
                employee.Name = EmployeeName;
            }
        }

        public void UpdateEmployeePhoneNumber(string userid, string phonenumber)
        {
            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == userid);
            if (employee != null)
            {
                employee.PhoneNumber = phonenumber;
            }
        }

        public void UpdateEmployeeGender(string userid, GenderType gender)
        {
            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == userid);
            if (employee != null)
            {
                employee.Gender = gender;
            }
        }

        public void UpdateEmployeeAddress(string userid, string address)
        {
            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == userid);
            if (employee != null)
            {
                employee.Address = address;
            }
        }

        public void DeleteEmployeeAccount(string userid)
        {
            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == userid);
            if (employee != null)
            {
                BankDatabase.Employees.Remove(employee);
            }
        }

        public void UpdateBankName(string Bankid,string name)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == Bankid);
            if(bank!=null)
            {
                bank.BankName = name;
            }
        }

        public void UpdateBankBranchName(string Bankid, string name)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == Bankid);
            if (bank != null)
            {
                bank.BranchName = name;
            }
        }

        public void UpdateAccountHolderName(string userid, string HolderName)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                bankaccount.Name =HolderName;
            }
        }

        public void UpdateAccountHolderPhoneNumber(string userid, string phonenumber)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                bankaccount.PhoneNumber = phonenumber;
            }
        }

        public void UpdateAccountHolderGender(string userid, GenderType gender)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                bankaccount.Gender = gender;
            }
        }

        public void UpdateAccountHolderAddress(string userid, string address)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                bankaccount.Address = address;
            }

        }
        
        public void DeleteAccountHolderAccount(string userid)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                BankDatabase.BankAccounts.Remove(bankaccount);
            }
        }

        public void revertTransaction(string transId)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(b => b.Transactions.Any(transaction => transaction.Id == transId));
            Transaction transaction = bankaccount != null ? bankaccount.Transactions.Find(transaction => transaction.Id == transId) : null;
            if (transaction != null)
            {
                BankAccount senderbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.SenderAccountId);
                BankAccount receiverbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.RecieverAccountId);
                senderbankaccount.Balance += transaction.Amount;
                receiverbankaccount.Balance -= transaction.Amount;
                Transaction trans = new Transaction(transaction.SenderAccountId, transaction.RecieverAccountId, transaction.Amount, senderbankaccount.BankId, TransactionType.Debited);
                Transaction trans1 = new Transaction(transaction.SenderAccountId, transaction.RecieverAccountId, transaction.Amount, senderbankaccount.BankId, TransactionType.Credited);
                senderbankaccount.Transactions.Add(trans);
                receiverbankaccount.Transactions.Add(trans1);

            }
        }
    }
}
