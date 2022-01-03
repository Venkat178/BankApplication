using System;
using System.Linq;
using System.Collections.Generic;
using BankApplication.Models;
using BankApplication.Concerns;


namespace BankApplication.Services
{
    public class AccountHolderService : IAccountHolderService
    {
        public BankApplicationDbContext BankAppDbctx;

        public AccountHolderService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }
        
        public APIResponse CreateAccountHolder(AccountHolder accountholder,int branchid)
        {
            Branch branch = BankAppDbctx.Branches.FirstOrDefault(b => b.Id == branchid);
            if (branch != null)
            {
                try
                {
                    if (BankAppDbctx.AccountHolders.Any(p => p.Name == accountholder.Name) == true)
                    {
                        return new APIResponse() { IsSuccess = false, Message = "Account already exists!" };
                    }
                    accountholder.Type = UserType.AccountHolder;
                    BankAppDbctx.AccountHolders.Add(accountholder);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Account Created successfully...!" };
                }
                catch(Exception)
                {
                    return new APIResponse() { IsSuccess = false, Message = "Error occured while creating account" };
                }
            }
            return new APIResponse() { IsSuccess = false, Message = "Branch not found" };
        }

        public APIResponse UpdateAccountHolder(AccountHolder accountholder)
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
                    return new APIResponse() { IsSuccess = true, Message = "Successfully updated" };
                }
                
            }
            catch(Exception)
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while updating account" };
            }
            return new APIResponse() { IsSuccess = false, Message = "Changes not done!" };
        }

        public APIResponse DeleteAccountHolderAccount(int userid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
                if (accountholder != null)
                {
                    BankAppDbctx.AccountHolders.Remove(accountholder);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch(Exception)
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while deleting.Try again" };
            }
            return new APIResponse() { IsSuccess = false, Message = "Error occured while deleting.Try again" };
        }

        public APIResponse RevertTransaction(User user,int transid)
        {
            Transaction transaction = BankAppDbctx.Transactions.FirstOrDefault(t => t.Id == transid);
            if(transaction != null)
            {
                try
                {
                    AccountHolder senderaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                    AccountHolder receiveraccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                    senderaccountholder.Balance += transaction.Amount;
                    receiveraccountholder.Balance -= transaction.Amount;
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Succefully transfered" };
                }
                catch (Exception)
                {
                    return new APIResponse() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
                }
            }
            return new APIResponse() { IsSuccess = false, Message = "Transaction not found" };
        }

        public AccountHolder GetAccountHolder(int accountid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountid);
            return accountholder;
        }

        public bool IsExitAccountHolder(int accountid)
        {
            return BankAppDbctx.AccountHolders.Any(p => p.Id == accountid);
        }

        public APIResponse ViewAllAccountHolders(int bankid)
        {
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
            if (bank != null)
            {
                return new APIResponse { AccountHolderList = BankAppDbctx.AccountHolders.Where(b => b.BankId == bankid).ToList(), IsSuccess = true };
            }
            return new APIResponse { Message = "Bank not found", IsSuccess = false };
        }

        public APIResponse ViewBalance(int accountid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountid);
            if(accountholder != null)
            {
                return new APIResponse { Message = accountholder.Balance.ToString() , IsSuccess = true};
            }
            return new APIResponse { Message = "Account Holder not found", IsSuccess = false };
        }

        public APIResponse ViewTransactions(int accountholderid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountholderid);
            if(accountholder != null)
            {
                List<Transaction> transactions = BankAppDbctx.Transactions.Where(t => t.SrcAccId == accountholderid ).ToList();
                //List<Transaction> transactions1 = BankAppDbctx.Transactions.Where(t => t.DestAccId == accountholderid).ToList();
                return new APIResponse { TransactionList = transactions ,IsSuccess = true};
            }
            return new APIResponse { Message = "Account Holder not found", IsSuccess = false };
        }
    }
}
