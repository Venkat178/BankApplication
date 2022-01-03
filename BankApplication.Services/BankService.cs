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

        public BankService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public APIResponse SetUpBank(Branch branch, Bank bank, Employee admin)
        {
            APIResponse status = new APIResponse();
            try
            {
                branch.IsMainBranch = true;
                branch.Bank = bank;

                admin.BranchId = bank.Id;
                admin.EmployeeId = admin.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                admin.Role = "Admin";


                BankAppDbctx.Banks.Add(bank);
                BankAppDbctx.SaveChanges();

                branch.Bank = bank;
                branch.BankId = bank.Id;

                BankAppDbctx.Branches.Add(branch);
                BankAppDbctx.SaveChanges();

                admin.Bank = bank;
                admin.BankId = bank.Id;
                admin.BranchId = branch.Id;

                BankAppDbctx.Employees.Add(admin);
                BankAppDbctx.SaveChanges();
                status.IsSuccess = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                status.IsSuccess = false;
                status.Message = "Unable to create a bank...!";
            }

            return status;
        }

        public APIResponse AddCurrency(string currencyCode, double exchangeRate, int bankid)
        {
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(_=>_.Id == bankid);
            if(bank != null)
            {
                BankAppDbctx.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate, IsDefault = false });
                BankAppDbctx.SaveChanges();
                return new APIResponse() { IsSuccess = true, Message = "Successfully added" };
            }
            return new APIResponse() { IsSuccess = false, Message = "Bank not found" };
        }

        public APIResponse UpdateCharges(Bank bank1)
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
                return new APIResponse() { IsSuccess = true, Message = "Successfully updated" };
            }
            return new APIResponse() { IsSuccess = false, Message = "Bank not found" };
        }

        public APIResponse AddBranch(Branch branch,int bankid)
        {
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(_=>_.Id == bankid);
            if(bank != null)
            {
                try
                {
                    BankAppDbctx.Branches.Add(branch);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully Added Branch" };
                }
                catch (Exception)
                {
                    return new APIResponse() { IsSuccess = false, Message = "Error occured while adding branch.Try again" };
                }
            }
            return new APIResponse() { IsSuccess = false, Message = "Error occured while adding branch.Try again" };
        }

        public APIResponse DeleteBranch(int branchid)
        {
            Branch branch = BankAppDbctx.Branches.FirstOrDefault(_ => _.Id == branchid);
            try
            {
                if (branch != null)
                {
                    BankAppDbctx.Branches.Remove(branch);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully deleted" };
                }
                else
                {
                    Console.WriteLine("Branch not found");
                    return new APIResponse() { IsSuccess = false, Message = "Branch ot found" };
                }
            }
            catch (Exception)
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
            }

        }

        public APIResponse Deposit(User user, int accountid, double amt)
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            if (accountholderservice.IsExitAccountHolder(accountid))
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                Transaction transaction = new Transaction()
                {
                    SrcAccId = accountid,
                    DestAccId = accountid,
                    Amount = amt,
                    CreatedBy = user.Id,
                    CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TransactionId = "TXN" + user.Id + user.Id + user.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Credit
                };
                accountholder.Balance += amt;
                BankAppDbctx.AccountHolders.Update(accountholder);
                BankAppDbctx.Transactions.Add(transaction);
                BankAppDbctx.SaveChanges();
                return new APIResponse { IsSuccess = true, Message = "Deposited Successfully" };
            }
            else
            {
                return new APIResponse { IsSuccess = false, Message = "AccountHolder not found" };
            }
        }

        public APIResponse Withdraw(User user, int accountid, double amt)
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            if (accountholderservice.IsExitAccountHolder(accountid))
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == accountid);
                if (accountholder.Balance >= amt)
                {
                    Transaction transaction = new Transaction()
                    {
                        SrcAccId = accountid,
                        DestAccId = accountid,
                        Amount = amt,
                        CreatedBy = user.Id,
                        CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                        TransactionId = "TXN" + user.Id + user.Id + user.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                        Type = TransactionType.Credit
                    };
                    accountholder.Balance -= amt;
                    BankAppDbctx.AccountHolders.Update(accountholder);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse { IsSuccess = true, Message = "Withdraw Successfull" };
                }
            }
            return new APIResponse { IsSuccess = false, Message = "AccountHolder not found" };
        }

        public APIResponse Transfer(User user, int srcid, int destid, double amt, Charges charge)
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            if (accountholderservice.IsExitAccountHolder(srcid) && accountholderservice.IsExitAccountHolder(destid))
            {
                AccountHolder srcaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == srcid);
                AccountHolder destaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(a => a.Id == destid);
                Transaction transaction = new Transaction()
                {
                    SrcAccId = srcid,
                    DestAccId = destid,
                    Amount = amt,
                    CreatedBy = user.Id,
                    CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TransactionId = "TXN" + user.Id + user.Id + user.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Transfer
                };
                if (srcaccountholder.BankId == destaccountholder.BankId)
                {
                    if(charge == Charges.RTGS)
                    {
                        if(srcaccountholder.Balance >= amt)
                        {
                            srcaccountholder.Balance = srcaccountholder.Balance - amt;
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            return new APIResponse { Message = "Transfered successfull", IsSuccess = true };
                        }
                        else
                        {
                            return new APIResponse { Message = "No sufficient Balance", IsSuccess = false };
                        }
                    }
                    else
                    {
                        if(srcaccountholder.Balance >= amt + (0.05 * amt))
                        {
                            srcaccountholder.Balance = srcaccountholder.Balance - (amt + (0.05 * amt));
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            return new APIResponse { Message = "Transfered successfull", IsSuccess = true };
                        }
                        else
                        {
                            return new APIResponse { Message = "No sufficient Balance", IsSuccess = false };
                        }
                    }
                }
                else
                {
                    if(charge == Charges.RTGS)
                    {
                        if(srcaccountholder.Balance >= amt + (0.02 * amt))
                        {
                            srcaccountholder.Balance = srcaccountholder.Balance - (amt + (0.02 * amt));
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            return new APIResponse { Message = "Transfered successfull", IsSuccess = true };
                        }
                        else
                        {
                            return new APIResponse { Message = "No sufficient Balance", IsSuccess = false };
                        }
                    }
                    else
                    {
                        if(srcaccountholder.Balance >= amt + (0.06 * amt))
                        {
                            srcaccountholder.Balance = srcaccountholder.Balance -= (amt + (0.06 * amt));
                            BankAppDbctx.AccountHolders.Update(srcaccountholder);
                            BankAppDbctx.Transactions.Add(transaction);
                            BankAppDbctx.SaveChanges();
                            return new APIResponse { Message = "Transfered successfull", IsSuccess = true };
                        }
                        else
                        {
                            return new APIResponse { Message = "No sufficient Balance", IsSuccess = false };
                        }
                    }
                }
                destaccountholder.Balance += amt;
                BankAppDbctx.AccountHolders.Update(destaccountholder);
                BankAppDbctx.Transactions.Add(transaction);
                BankAppDbctx.SaveChanges();
                return new APIResponse { IsSuccess = true, Message = "Transfered Successfull" };
            }
            else
            {
                return new APIResponse { IsSuccess = false, Message = "Users not found" };
            }

        }

        public APIResponse ViewAllBranches(int bankid)
        {
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
            if (bank != null)
            {
                return new APIResponse { BranchList = BankAppDbctx.Branches.Where(_ => _.BankId == bankid).ToList(), IsSuccess = true };
            }
            return new APIResponse { Message = "Bank not found", IsSuccess = false };
        }
    }
}


