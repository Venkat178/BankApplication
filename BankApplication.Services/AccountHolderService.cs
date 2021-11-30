using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountHolderService
    {
        public bool UpdateAccountHolder(AccountHolder accountholder)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == accountholder.Id));
                AccountHolder oldaccountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == accountholder.Id) : null;
                oldaccountholder.Name = accountholder.Name != null ? oldaccountholder.Name : accountholder.Name;
                oldaccountholder.PhoneNumber = accountholder.PhoneNumber != default(int) ? oldaccountholder.PhoneNumber : accountholder.PhoneNumber;
                oldaccountholder.Address = accountholder.Address != null ? oldaccountholder.Address : accountholder.Address;
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            return false;
        }

        public bool DeleteAccountHolderAccount(string userid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
                AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
                if (accountholder != null)
                {
                    bank.AccountHolders.Remove(accountholder);
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return false;
        }
        public void ViewTransactions(AccountHolder AccountHolder)
        {
            foreach(var i in AccountHolder.Transactions)
            {
                Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
            }
        }

        public bool revertTransaction(Transaction transaction)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Transactions.Any(trans => trans.Id == transaction.Id)));
                AccountHolder senderaccountholder = bank.AccountHolders.Find(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                AccountHolder receiveraccountholder = bank.AccountHolders.Find(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                senderaccountholder.Balance += transaction.Amount;
                receiveraccountholder.Balance -= transaction.Amount;
                senderaccountholder.Transactions.Add(transaction);
                receiveraccountholder.Transactions.Add(transaction);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }

        public bool AddCurrency(string currencyCode, double exchangeRate,Bank bank)
        {
            try
            {
                bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate });
            }
            catch(Exception ex)
            {
                return false;
            }
            return false;
        }

        public bool UpdateCharges(Bank bank)
        {
            try
            {
                Bank oldbank = BankDatabase.Banks.Find(bank => bank.Id == bank.Id);
                bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank != default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank != default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank != default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank != default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }
    }
}
