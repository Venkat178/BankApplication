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
        
        public APIResponse<AccountHolder> CreateAccountHolder(AccountHolder accountholder)
        {
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(b => b.Id == accountholder.BranchId);
                if (branch != null)
                {
                    if (BankAppDbctx.AccountHolders.Any(p => p.Name == accountholder.Name) == true)
                    {
                        return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Account already exists!" };
                    }

                    accountholder.Type = UserType.AccountHolder;
                    BankAppDbctx.AccountHolders.Add(accountholder);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<AccountHolder>() { IsSuccess = true, Message = accountholder.Id.ToString() };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Error occured while creating account" };
            }
            return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Branch not found" };           
        }

        public APIResponse<AccountHolder> UpdateAccountHolder(AccountHolder accountholder)
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

                    return new APIResponse<AccountHolder>() { IsSuccess = true, Message = "Successfully updated" };
                }
                
            }
            catch(Exception)
            {
                return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Error occured while updating account" };
            }
            return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Changes not done!" };
        }

        public APIResponse<AccountHolder> DeleteAccountHolderAccount(int userid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
                if (accountholder != null)
                {
                    BankAppDbctx.AccountHolders.Remove(accountholder);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<AccountHolder>() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch(Exception)
            {
                return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Error occured while deleting.Try again" };
            }
            return new APIResponse<AccountHolder>() { IsSuccess = false, Message = "Account Holder not found" };
        }

        public APIResponse<Transaction> RevertTransaction(User user,int transid)
        {
            try
            {
                Transaction transaction = BankAppDbctx.Transactions.FirstOrDefault(t => t.Id == transid);
                if (transaction != null)
                {
                    if(transaction.Type == TransactionType.Transfer)
                    {
                        AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                        AccountHolder senderaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                        senderaccountholder.Balance += transaction.Amount;
                        destaccountholder.Balance -= transaction.Amount;
                        BankAppDbctx.AccountHolders.Update(senderaccountholder);
                        BankAppDbctx.AccountHolders.Update(destaccountholder);
                        //BankAppDbctx.Transactions.Add(transaction);
                        BankAppDbctx.SaveChanges();

                        return new APIResponse<Transaction>() { IsSuccess = true, Message = "Succefully transfered" };
                    }
                    else
                    {
                        return new APIResponse<Transaction>() { IsSuccess = false, Message = "Deposit , credit not to be reverted" };
                    }
                }
            }
            catch (Exception)
            {
                return new APIResponse<Transaction>() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
            }
            return new APIResponse<Transaction>() { IsSuccess = false, Message = "Transaction not found" };          
        }

        public AccountHolder GetAccountHolder(int accountid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountid);

                return accountholder;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public bool IsExitAccountHolder(int accountid)
        {
            try
            {
                return BankAppDbctx.AccountHolders.Any(p => p.Id == accountid);
            }
            catch
            {
                return false;
            }
        }

        public APIResponse<AccountHolder> ViewAllAccountHolders(int bankid)
        {
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    return new APIResponse<AccountHolder> { list = BankAppDbctx.AccountHolders.Where(b => b.BankId == bankid).ToList(), IsSuccess = true };
                }
                return new APIResponse<AccountHolder> { Message = "Bank not found", IsSuccess = false };
            }
            catch(Exception)
            {
                return new APIResponse<AccountHolder> { Message = "Error occured", IsSuccess = false };
            }
        }

        public APIResponse<AccountHolder> ViewBalance(int accountid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountid);
                if (accountholder != null)
                {
                    return new APIResponse<AccountHolder> { Message = accountholder.Balance.ToString(), IsSuccess = true };
                }
                return new APIResponse<AccountHolder> { Message = "Account Holder not found", IsSuccess = false };
            }
            catch(Exception)
            {
                return new APIResponse<AccountHolder> { Message = "Error occured", IsSuccess = false };
            }
        }

        public APIResponse<Transaction> ViewTransactions(int accountholderid)
        {
            try
            {
                APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountholderid);
                if (accountholder != null)
                {
                    List<Transaction> transactions = BankAppDbctx.Transactions.Where(t => t.SrcAccId == accountholderid).ToList();

                    return new APIResponse<Transaction> { list = transactions, IsSuccess = true };
                }
                return new APIResponse<Transaction> { Message = "Account Holder not found", IsSuccess = false };
            }
            catch(Exception)
            {
                return new APIResponse<Transaction> { Message = "Erro occured", IsSuccess = false };
            }
        }
    }
}
