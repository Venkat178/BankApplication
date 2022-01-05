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
        private IAccountHolderService accountholderservice;

        public BankService(IAccountHolderService accountholderservice)
        {
            this.accountholderservice = accountholderservice;
            BankAppDbctx = new BankApplicationDbContext();
        }

        public APIResponse<Bank> CreateBank(Branch branch,Employee admin)
        {
            APIResponse<Bank> status = new APIResponse<Bank>();
            try
            {
                branch.IsMainBranch = true;
                //branch.Bank = admin.Bank;

                BankAppDbctx.Banks.Add(admin.Bank);
                BankAppDbctx.SaveChanges();

                admin.BranchId = branch.BankId = admin.Bank.Id;
                admin.EmployeeId = admin.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");

                BankAppDbctx.Branches.Add(branch);
                BankAppDbctx.SaveChanges();

                BankAppDbctx.Employees.Add(admin);
                BankAppDbctx.SaveChanges();
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.IsSuccess = false;
                status.Message = "Unable to create a bank...!";
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
                    BankAppDbctx.CurrencyCodes.Add(new CurrencyCode() { Code = currencycode.Code, ExchangeRate = currencycode.ExchangeRate, IsDefault = false });
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<CurrencyCode>() { IsSuccess = true, Message = "Successfully added" };
                }
                return new APIResponse<CurrencyCode>() { IsSuccess = false, Message = "Bank not found" };
            }
            catch(Exception)
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
            catch(Exception)
            {
                return new APIResponse<int>() { IsSuccess = false, Message = "Error occured" };
            }
        }

        public APIResponse<Branch> AddBranch(Branch branch)
        {
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == branch.BankId);
                if (bank != null)
                {
                    BankAppDbctx.Branches.Add(branch);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Branch>() { IsSuccess = true, Message = "Successfully Added Branch" };
                }
            }
            catch (Exception)
            {
                return new APIResponse<Branch>() { IsSuccess = false, Message = "Error occured while adding branch.Try again" };
            }
            return new APIResponse<Branch>() { IsSuccess = false, Message = "Bank not found" };
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
                //AccountHolderService accountholderservice = new AccountHolderService();
                if (accountholderservice.IsExitAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    Transaction transaction = Utilities.GetTransaction(user.Id,default(int),accountid,amt,accountholder.BankId,TransactionType.Debit);
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
            catch(Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while depositing" };
            }
        }

        public APIResponse<Transaction> Withdraw(User user, int accountid, double amt)
        {
            try
            {
                AccountHolderService accountholderservice = new AccountHolderService();
                if (accountholderservice.IsExitAccountHolder(accountid))
                {
                    AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                    if (accountholder.Balance >= amt)
                    {
                        Transaction transaction = Utilities.GetTransaction(user.Id, accountid, default(int), amt, accountholder.BankId, TransactionType.Credit);
                        accountholder.Balance -= amt;
                        BankAppDbctx.AccountHolders.Update(accountholder);
                        BankAppDbctx.Transactions.Add(transaction);
                        BankAppDbctx.SaveChanges();

                        return new APIResponse<Transaction> { IsSuccess = true, Message = "Withdraw Successfull" };
                    }
                }
                return new APIResponse<Transaction> { IsSuccess = false, Message = "AccountHolder not found" };
            }
            catch(Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while withdraw" };
            }
        }

        public APIResponse<Transaction> Transfer(User user,Transaction transaction1,Charges charge)
        {
            try
            {
                AccountHolderService accountholderservice = new AccountHolderService();
                if (accountholderservice.IsExitAccountHolder(transaction1.SrcAccId) && accountholderservice.IsExitAccountHolder(transaction1.DestAccId))
                {
                    AccountHolder srcaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == transaction1.SrcAccId);
                    AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == transaction1.DestAccId);
                    Transaction transaction = Utilities.GetTransaction(user.Id, transaction1.SrcAccId, transaction1.DestAccId, transaction1.Amount, srcaccountholder.BankId, TransactionType.Transfer);
                    if (srcaccountholder.BankId == destaccountholder.BankId)
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (srcaccountholder.Balance >= transaction1.Amount)
                            {
                                srcaccountholder.Balance = srcaccountholder.Balance - transaction1.Amount;
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
                            if (srcaccountholder.Balance >= transaction1.Amount + (0.05 * transaction1.Amount))
                            {
                                srcaccountholder.Balance = srcaccountholder.Balance - (transaction1.Amount + (0.05 * transaction1.Amount));
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
                    else
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (srcaccountholder.Balance >= transaction1.Amount + (0.02 * transaction1.Amount))
                            {
                                srcaccountholder.Balance = srcaccountholder.Balance - (transaction1.Amount + (0.02 * transaction1.Amount));
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
                    destaccountholder.Balance += transaction1.Amount;
                    BankAppDbctx.AccountHolders.Update(destaccountholder);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Transaction> { IsSuccess = true, Message = "Transfered Successfull" };
                }
                else
                {
                    return new APIResponse<Transaction> { IsSuccess = false, Message = "Users not found" };
                }
            }
            catch(Exception)
            {
                return new APIResponse<Transaction> { IsSuccess = false, Message = "Error occured while transfering" };
            }
        }

        public APIResponse<Branch> ViewAllBranches(int bankid)
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
            catch(Exception)
            {
                return new APIResponse<Branch> { Message = "Error occured", IsSuccess = false };
            }
        }
    }
}


