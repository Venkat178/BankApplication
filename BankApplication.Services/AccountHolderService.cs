using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountHolderService
    {
        public void UpdateAccountHolder(BankAccount bankaccount,string userid)
        {
            BankAccount oldbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            oldbankaccount.Name = bankaccount.Name != null ? oldbankaccount.Name : bankaccount.Name;
            oldbankaccount.PhoneNumber = bankaccount.PhoneNumber != null ? oldbankaccount.PhoneNumber : bankaccount.PhoneNumber;
            oldbankaccount.Address = bankaccount.Address !=null ? oldbankaccount.Address : bankaccount.Address;
        }

        public bool DeleteAccountHolderAccount(string userid)
        {
            bool Isdeleted = false;
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                BankDatabase.BankAccounts.Remove(bankaccount);
                Isdeleted = true;
            }
            return Isdeleted;
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

        public string Transfer(BankAccount bankaccount, BankAccount recevierbankaccount, double amt, Charges charge)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (bankaccount != null && recevierbankaccount != null)
            {
                recevierbankaccount.Balance += amt;
                transaction.SrcAccId = bankaccount.Id;
                transaction.DestAccId = recevierbankaccount.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = bankaccount.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + bankaccount.Id + recevierbankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;
                txnid = transaction.Id;
                if (bankaccount.BankId == recevierbankaccount.BankId)
                {
                    bankaccount.Balance = charge == Charges.RTGS ? (bankaccount.Balance >= amt ? bankaccount.Balance - amt : 0) : (bankaccount.Balance >= amt + (0.05 * amt) ? bankaccount.Balance - (amt + (0.05 * amt)) : 0);
                    bankaccount.Transactions.Add(transaction);
                    recevierbankaccount.Transactions.Add(transaction);
                }
                else
                {
                    bankaccount.Balance = charge == Charges.RTGS ? (bankaccount.Balance >= amt + (0.02 * amt) ? bankaccount.Balance - (amt + (0.02 * amt)) : 0) : (bankaccount.Balance >= amt + (0.06 * amt) ? bankaccount.Balance -= (amt + (0.06 * amt)) : 0);
                    bankaccount.Transactions.Add(transaction);
                    recevierbankaccount.Transactions.Add(transaction);
                }
            }
            return txnid;
        }

        public void ViewTransactions(BankAccount bankaccount)
        {
            foreach(var i in bankaccount.Transactions)
            {
                Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
            }
        }

        public void revertTransaction(Transaction transaction)
        {
            BankAccount senderbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.SrcAccId);
            BankAccount receiverbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.DestAccId);
            senderbankaccount.Balance += transaction.Amount;
            receiverbankaccount.Balance -= transaction.Amount;
            senderbankaccount.Transactions.Add(transaction);
            receiverbankaccount.Transactions.Add(transaction);
        }

        public void AddCurrency(string currencyCode, double exchangeRate,Bank bank)
        {
            bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate });
        }

        public void UpdateCharges(Bank bank,string Bankid)
        {
            Bank oldbank = BankDatabase.Banks.Find(bank => bank.Id == Bankid);
            bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank != 0 ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
            bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank != 0 ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
            bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank != 0 ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
            bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank != 0 ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
        }
    }
}
