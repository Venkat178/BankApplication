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
                BankAppDbctx.Banks.Add(bank);
                BankAppDbctx.SaveChanges();

                status.Data = bank;
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.IsSuccess = false;
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
                status.IsSuccess = false;
                status.Message = "Unable to create a branch...!";
            }

            return status;
        }

        public APIResponse<CurrencyCode> AddCurrency(CurrencyCode currencycode)
        {
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == currencycode.BankId);
                if (bank != null)
                {
                    currencycode.IsDefault = false;
                    BankAppDbctx.CurrencyCodes.Add(currencycode);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<CurrencyCode>() { IsSuccess = true, Message = "Successfully added" };
                }

                return new APIResponse<CurrencyCode>() { IsSuccess = false, Message = "Bank not found" };
            }
            catch (Exception ex)
            {
                return new APIResponse<CurrencyCode>() { IsSuccess = false, Message = "Error occured whille adding" };
            }
        }

        public APIResponse<int> UpdateCharges(Bank bank1)
        {
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

                    return new APIResponse<int>() { IsSuccess = true, Message = "Successfully updated" };
                }

                return new APIResponse<int>() { IsSuccess = false, Message = "Bank not found" };
            }
            catch (Exception)
            {
                return new APIResponse<int>() { IsSuccess = false, Message = "Error occured" };
            }
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
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(_ => _.Id == branchid);
                if (branch != null)
                {
                    BankAppDbctx.Branches.Remove(branch);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Branch>() { IsSuccess = true, Message = "Successfully deleted" };
                }
                else
                {
                    return new APIResponse<Branch>() { IsSuccess = false, Message = "Branch not found" };
                }
            }
            catch (Exception)
            {
                return new APIResponse<Branch>() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
            }
        }

        public APIResponse<Transaction> Deposit(User user, int accountid, double amt)
        {
            try
            {
                if (accountholderservice.IsExistAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    Transaction transaction = Utilities.GetTransaction(user.Id, default(int), accountid, amt, accountholder.BankId, TransactionType.Credit);
                    accountholder.Balance += amt;
                    BankAppDbctx.AccountHolders.Update(accountholder);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Transaction> { IsSuccess = true, Message = "Deposited Successfully" };
                }
                else
                {
                    return new APIResponse<Transaction> { IsSuccess = false, Message = "AccountHolder not found" };
                }
            }
            catch (Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while depositing" };
            }
        }

        public APIResponse<Transaction> Withdraw(User user, int accountid, double amt)
        {
            try
            {
                if (accountholderservice.IsExistAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    if (accountholder.Balance >= amt)
                    {
                        Transaction transaction = Utilities.GetTransaction(user.Id, accountid, default(int), amt, accountholder.BankId, TransactionType.Debit);
                        accountholder.Balance -= amt;
                        BankAppDbctx.AccountHolders.Update(accountholder);
                        BankAppDbctx.Transactions.Add(transaction);
                        BankAppDbctx.SaveChanges();

                        return new APIResponse<Transaction> { IsSuccess = true, Message = "Withdraw Successfull" };
                    }
                }

                return new APIResponse<Transaction> { IsSuccess = false, Message = "AccountHolder not found" };
            }
            catch (Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while withdraw" };
            }
        }

        public APIResponse<Transaction> Transfer(int userid, int srcId, int destId, float amount, Charges charge)
        {
            try
            {
                AccountHolder srcaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == srcId);
                AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == destId);
                if (srcaccountholder != null && destaccountholder != null)
                {
                    Transaction transaction = Utilities.GetTransaction(userid, srcId, destId, amount, srcaccountholder.BankId, TransactionType.Transfer);
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

                            return new APIResponse<Transaction> { Message = "Transfered successfull", IsSuccess = true };
                        }
                        else
                        {
                            return new APIResponse<Transaction> { Message = "No sufficient Balance", IsSuccess = false };
                        }
                    }
                    else
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (srcaccountholder.Balance >= transaction1.Amount + (0.02 * transaction1.Amount))
                            {

                                srcaccountholder.Balance = srcaccountholder.Balance - (transaction1.Amount + (0.02 * transaction1.Amount));
                                destaccountholder.Balance += transaction1.Amount;
                                BankAppDbctx.AccountHolders.Update(destaccountholder);
                                BankAppDbctx.AccountHolders.Update(srcaccountholder);
                                BankAppDbctx.Transactions.Add(transaction);
                                BankAppDbctx.SaveChanges();

                                return new APIResponse<Transaction> { Message = "Transfered successfull", IsSuccess = true };
                            }
                            else
                            {
                                return new APIResponse<Transaction> { Message = "No sufficient Balance", IsSuccess = false };
                            }
                        }
                        else
                        {
                            if (srcaccountholder.Balance >= transaction1.Amount + (0.06 * transaction1.Amount))
                            {

                                srcaccountholder.Balance = srcaccountholder.Balance -= (transaction1.Amount + (0.06 * transaction1.Amount));
                                destaccountholder.Balance += transaction1.Amount;
                                BankAppDbctx.AccountHolders.Update(destaccountholder);
                                BankAppDbctx.AccountHolders.Update(srcaccountholder);
                                BankAppDbctx.Transactions.Add(transaction);
                                BankAppDbctx.SaveChanges();

                                return new APIResponse<Transaction> { Message = "Transfered successfull", IsSuccess = true };
                            }
                            else
                            {
                                return new APIResponse<Transaction> { Message = "No sufficient Balance", IsSuccess = false };
                            }
                        }
                    }
                }
                else
                {
                    return new APIResponse<Transaction> { IsSuccess = false, Message = "Users not found" };
                }
            }
            catch (Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while transfering" };
            }
        }

        public APIResponse<Transaction> RevertTransaction(User user, int transid)
        {
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

        public APIResponse<Branch> GetBranchesBybank(int bankid)
        {
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    return new APIResponse<Branch> { list = BankAppDbctx.Branches.Where(_ => _.BankId == bankid).ToList(), IsSuccess = true };
                }

                return new APIResponse<Branch> { Message = "Bank not found", IsSuccess = false };
            }
            catch (Exception)
            {
                return new APIResponse<Branch> { Message = "Error occured", IsSuccess = false };
            }
        }
    }
}


