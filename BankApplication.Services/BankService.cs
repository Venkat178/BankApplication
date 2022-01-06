using System;
using BankApplication.Models;
using BankApplication.Concerns;
using System.Linq;
using System.Collections.Generic;

namespace BankApplication.Services
{
    public class BankService : IBankService
    {
        public BankApplicationDbContext BankAppDbctx;
        private readonly IAccountHolderService accountholderservice;

        public BankService(IAccountHolderService accountholderservice)
        {
            this.accountholderservice = accountholderservice;
            BankAppDbctx = new BankApplicationDbContext();
        }

        public APIResponse<Bank> CreateBank(Bank bank)
        {
            APIResponse<Bank> status = new APIResponse<Bank>();
            try
            {
                bank.CurrencyCodes = new List<CurrencyCode>() { new CurrencyCode() { Code = "INR", ExchangeRate = 1, IsDefault = true } };
                bank.IMPSChargesforSameBank = 5;
                bank.RTGSChargesforSameBank = 0;
                bank.IMPSChargesforDifferentBank = 6;
                bank.RTGSChargesforDifferentBank = 2;
                BankAppDbctx.Banks.Add(bank);
                BankAppDbctx.SaveChanges();

                status.Data = bank;
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.Message = "Unable to create a bank...!";
            }

            return status;
        }

        public APIResponse<Branch> CreateBranch(Branch branch)
        {
            APIResponse<Branch> status = new APIResponse<Branch>();
            try
            {
                
                BankAppDbctx.Branches.Add(branch);
                BankAppDbctx.SaveChanges();

                status.Data = branch;
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.Message = "Unable to create a branch...!";
            }

            return status;
        }

        public APIResponse<CurrencyCode> AddCurrency(CurrencyCode currencycode)
        {
            APIResponse<CurrencyCode> apiresponse = new APIResponse<CurrencyCode>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == currencycode.BankId);
                if (bank != null)
                {
                    currencycode.IsDefault = false;
                    BankAppDbctx.CurrencyCodes.Add(currencycode);
                    BankAppDbctx.SaveChanges();
                    apiresponse.Message = "Successfully added";
                    apiresponse.IsSuccess = true;
                }
                else
                {
                    apiresponse.Message = "Bank not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured whille adding";
            }
            return apiresponse;
        }

        public APIResponse<int> UpdateCharges(Bank bank1)
        {
            APIResponse<int> apiresponse = new APIResponse<int>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bank1.Id);
                if (bank != null)
                {
                    Bank oldbank = BankAppDbctx.Banks.FirstOrDefault(bank => bank.Id == bank.Id);
                    bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank == default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                    bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank == default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                    bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank == default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                    bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank == default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = "Successfully updated";
                }
                else
                {
                    apiresponse.Message = "Bank not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
        }

        public APIResponse<Branch> AddBranch(Branch branch)
        {
            APIResponse<Branch> response = new APIResponse<Branch>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == branch.BankId);
                if (bank != null)
                {
                    BankAppDbctx.Branches.Add(branch);
                    BankAppDbctx.SaveChanges();

                    response.IsSuccess = true;
                    response.Message = "Successfully Added Branch";
                }
            }
            catch (Exception)
            {
                response.Message = "Error occured while adding branch.Try again";
            }

            response.Message = "Bank not found";
            return response;
        }

        public APIResponse<Branch> DeleteBranch(int branchid)
        {
            APIResponse<Branch> apiresponse = new APIResponse<Branch>();
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(_ => _.Id == branchid);
                if (branch != null)
                {
                    BankAppDbctx.Branches.Remove(branch);
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = "Successfully deleted";
                }
                else
                {
                    apiresponse.Message = "Branch not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while deleting branch.Try again";
            }
            return apiresponse;
        }

        public APIResponse<Transaction> Deposit(User user, int accountid, double amt)
        {
            APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
            try
            {
                if (accountholderservice.IsExistAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    Transaction transaction = Utilities.GenarateTransaction(user.Id, default(int), accountid, amt, accountholder.BankId, TransactionType.Credit);
                    accountholder.Balance += amt;
                    BankAppDbctx.AccountHolders.Update(accountholder);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    apiresponse.Message = "Deposited Successfully";
                    apiresponse.IsSuccess = true;
                }
                else
                {
                    apiresponse.Message = "AccountHolder not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while depositing";
            }
            return apiresponse;
        }

        public APIResponse<Transaction> Withdraw(User user, int accountid, double amt)
        {
            APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
            try
            {
                if (accountholderservice.IsExistAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    if (accountholder.Balance >= amt)
                    {
                        Transaction transaction = Utilities.GenarateTransaction(user.Id, accountid, default(int), amt, accountholder.BankId, TransactionType.Debit);
                        accountholder.Balance -= amt;
                        BankAppDbctx.AccountHolders.Update(accountholder);
                        BankAppDbctx.Transactions.Add(transaction);
                        BankAppDbctx.SaveChanges();
                        apiresponse.IsSuccess = true;
                        apiresponse.Message = "Withdraw Successfull";
                    }
                    else
                    {
                        apiresponse.Message = "Money not sufficient";
                    }
                }
                else
                {
                    apiresponse.Message = "AccountHolder not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while withdraw";
            }
            return apiresponse;
        }

        public APIResponse<Transaction> Transfer(int userid, int srcId, int destId, double amount, Charges charge)
        {
            APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
            try
            {
                AccountHolder srcaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == srcId);
                AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == destId);
                if (srcaccountholder != null && destaccountholder != null)
                {
                    Transaction transaction = Utilities.GenarateTransaction(userid, srcId, destId, amount, srcaccountholder.BankId, TransactionType.Transfer);
                    if (srcaccountholder.BankId == destaccountholder.BankId)
                    {
                        var isRtgs = charge == Charges.RTGS;
                        if((isRtgs && srcaccountholder.Balance >= amount) || (!isRtgs && srcaccountholder.Balance >= amount + (0.05 * amount)))
                        {
                            srcaccountholder.Balance = isRtgs 
                                ? (srcaccountholder.Balance - amount) 
                                : (srcaccountholder.Balance - (amount + (0.05 * amount)));
                            destaccountholder.Balance += amount;
                            BankAppDbctx.AccountHolders.Update(destaccountholder);
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            apiresponse.IsSuccess = true;
                            apiresponse.Message = "Transfered successfull";
                        }
                        else
                        {
                            apiresponse.Message = "No sufficient Balance";
                        }
                    }
                    else
                    {
                        var isRtgs = charge == Charges.RTGS;
                        if ((isRtgs && srcaccountholder.Balance >= amount + (0.02 * amount) || (!isRtgs && srcaccountholder.Balance >= amount + (0.06 * amount))))
                        {
                            srcaccountholder.Balance = isRtgs
                                ? srcaccountholder.Balance - (amount + (0.02 * amount))
                                : (srcaccountholder.Balance - (amount + (0.06 * amount)));
                            destaccountholder.Balance += amount;
                            BankAppDbctx.AccountHolders.Update(destaccountholder);
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            apiresponse.IsSuccess = true;
                            apiresponse.Message = "Transfered successfull";
                        }
                        else
                        {
                            apiresponse.Message = "No sufficient Balance";
                        }
                    }
                }
                else
                {
                    apiresponse.Message = "Users not found";
                }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while transfering";
            }
            return apiresponse;
        }

        public APIResponse<Transaction> RevertTransaction(User user, int transid)
        {
            APIResponse<Transaction> apiresponse = new APIResponse<Transaction>();
            try
            {
                Transaction transaction = BankAppDbctx.Transactions.FirstOrDefault(t => t.Id == transid);
                if (transaction != null)
                {
                    if (transaction.Type == TransactionType.Transfer)
                    {
                        AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                        AccountHolder senderaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                        senderaccountholder.Balance += transaction.Amount;
                        destaccountholder.Balance -= transaction.Amount;
                        BankAppDbctx.AccountHolders.Update(senderaccountholder);
                        BankAppDbctx.AccountHolders.Update(destaccountholder);
                        BankAppDbctx.SaveChanges();
                        apiresponse.IsSuccess = true;
                        apiresponse.Message = "Succefully transfered";
                    }
                    else
                    {
                        apiresponse.Message = "Deposit , credit not to be reverted";
                    }
                }
                else
                {
                    apiresponse.Message = "Transaction not found" ;
            }
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured while transfering.Try again";
            }
            return apiresponse;
        }

        public APIResponse<Branch> GetBranchesBybank(int bankid)
        {
            APIResponse<Branch> apiresponse = new APIResponse<Branch>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    apiresponse.IsSuccess = true;
                    apiresponse.ListData = BankAppDbctx.Branches.Where(_ => _.BankId == bankid).ToList();
                }

                apiresponse.Message = "Bank not found";
            }
            catch (Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
        }
    }
}


