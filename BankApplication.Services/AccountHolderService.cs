using System;
using BankApplication.Models;
using BankApplication.Concerns;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountHolderService : IAccountHolderService
    {
        public BankApplicationDbContext BankAppDbctx;

        public AccountHolderService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public Status UpdateAccountHolder(AccountHolder accountholder)
        {
            try
            {
                AccountHolder oldaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountholder.Id);
                if(oldaccountholder != null)
                {
                    oldaccountholder.Name = accountholder.Name == string.Empty ? oldaccountholder.Name : accountholder.Name;
                    oldaccountholder.PhoneNumber = accountholder.PhoneNumber == default(long) ? oldaccountholder.PhoneNumber : accountholder.PhoneNumber;
                    oldaccountholder.Address = accountholder.Address == string.Empty ? oldaccountholder.Address : accountholder.Address;
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully updated" };
                }
                
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Changes not done!" };
            }
            return new Status() { IsSuccess = false, Message = "Changes not done!" };
        }

        public Status DeleteAccountHolderAccount(int userid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
                if (accountholder != null)
                {
                    BankAppDbctx.AccountHolders.Remove(accountholder);
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while deleting.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while deleting.Try again" };
        }

        public Status revertTransaction(Transaction transaction)
        {
            try
            {
                AccountHolder senderaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                AccountHolder receiveraccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                senderaccountholder.Balance += transaction.Amount;
                receiveraccountholder.Balance -= transaction.Amount;
                BankAppDbctx.Transactions.Add(transaction);
                //receiveraccountholder.Transactions.Add(transaction);
                BankAppDbctx.SaveChanges();
                return new Status() { IsSuccess = true, Message = "Succefully transfered" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
            }
        }

        public Status AddCurrency(string currencyCode, double exchangeRate,Bank bank)
        {
            try
            {
                BankAppDbctx.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate ,IsDefault = false});
                BankAppDbctx.SaveChanges();
                return new Status() { IsSuccess = true, Message = "Successfully added" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while adding currency.Try again" };
            }
        }

        public Status UpdateCharges(Bank bank)
        {
            try
            {
                Bank oldbank = BankAppDbctx.Banks.FirstOrDefault(bank => bank.Id == bank.Id);
                bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank == default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank == default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank == default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank == default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                BankAppDbctx.SaveChanges();
                return new Status() { IsSuccess = true, Message = "Successfully updated" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while updating charges.Try again" };
            }
        }
    }
}
