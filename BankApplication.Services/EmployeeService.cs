using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    public class EmployeeService
    {
        public void UpdateAccountHolderName(string userid, string HolderName)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                bankaccount.Name = HolderName;
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

        public string returnId(Bank bank, string HolderName)
        {
            string id = string.Empty;
            foreach (var i in bank.BankAccounts)
            {
                if (i.Name == HolderName)
                {
                    id = i.Id;
                }
            }
            return id;
        }
    }
}
