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
            APIResponse<AccountHolder> apiresponse = new APIResponse<AccountHolder>();
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(b => b.Id == accountholder.BranchId);
                if (branch != null)
                {
                    accountholder.Type = UserType.AccountHolder;
                    BankAppDbctx.AccountHolders.Add(accountholder);
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = accountholder.Id.ToString();
                }
                else
                {
                    apiresponse.Message = "Branch not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while creating account";
            }
            
            return apiresponse;
        }

        public APIResponse<AccountHolder> UpdateAccountHolder(AccountHolder accountholder)
        {
            APIResponse<AccountHolder> apiresponse = new APIResponse<AccountHolder>();
            try
            {
                AccountHolder oldaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountholder.Id);
                if(oldaccountholder != null)
                {
                    oldaccountholder.Name = string.IsNullOrEmpty(accountholder.Name) ? oldaccountholder.Name : accountholder.Name;
                    oldaccountholder.PhoneNumber = accountholder.PhoneNumber == default(long) ? oldaccountholder.PhoneNumber : accountholder.PhoneNumber;
                    oldaccountholder.Address = string.IsNullOrEmpty(accountholder.Address) ? oldaccountholder.Address : accountholder.Address;
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = "Successfully updated";
                }
                else
                {
                    apiresponse.Message = "AccountHolder not found";
                }
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured while updating account";
            }
            return apiresponse;
        }

        public APIResponse<AccountHolder> DeleteAccountHolderAccount(int userid)
        {
            APIResponse<AccountHolder> apiresponse = new APIResponse<AccountHolder>();
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
                if (accountholder != null)
                {
                    BankAppDbctx.AccountHolders.Remove(accountholder);
                    BankAppDbctx.SaveChanges();
                    apiresponse.Message = "Successfully deleted";
                    apiresponse.IsSuccess = true;
                }
                else
                {
                    apiresponse.Message = "Account Holder not found";
                }
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured while deleting.Try again";
            }
            
            return apiresponse;
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

        public bool IsExistAccountHolder(int accountid)
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

        public APIResponse<AccountHolder> GetAccountHoldersByBank(int bankid)
        {
            APIResponse<AccountHolder> apiresponse = new APIResponse<AccountHolder>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    apiresponse.ListData = BankAppDbctx.AccountHolders.Where(b => b.BankId == bankid).ToList();
                    apiresponse.IsSuccess = true;
                }
                else
                {
                    apiresponse.Message = "Bank not found";
                }
                
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
        }

        public APIResponse<AccountHolder> GetBalance(int accountid)
        {
            APIResponse<AccountHolder> apiresponse = new APIResponse<AccountHolder>();
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountid);
                if (accountholder != null)
                {
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = accountholder.Balance.ToString();
                }
                else
                {
                    apiresponse.Message = "Account Holder not found";
                }
                
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
        }

        public APIResponse<Transaction> GetTransactions(int accountholderid)
        {
            APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountholderid);
                if (accountholder != null)
                {
                    List<Transaction> transactions = BankAppDbctx.Transactions.Where(t => t.SrcAccId == accountholderid).ToList();
                    apiresponse.IsSuccess = true;
                    apiresponse.ListData = transactions;
                }
                else
                {
                    apiresponse.Message = "Successfully updated";
                }
                
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
        }
    }
}
