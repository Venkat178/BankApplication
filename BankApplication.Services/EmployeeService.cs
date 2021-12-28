using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Concerns;

namespace BankApplication.Services
{
    public class EmployeeService : IEmployeeService
    {
        public BankApplicationDbContext BankAppDbctx;

        public EmployeeService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public Status UpdateEmployee(Employee employee)
        {
            try
            {
                var oldemployee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employee.Id);
                if (oldemployee != null)
                {
                    oldemployee.Name = employee.Name == string.Empty ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber == default(long) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = employee.Address == string.Empty ? oldemployee.Address : employee.Address;
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully updated" };
                }
            }
            catch (Exception) 
            {
                return new Status() { IsSuccess = false, Message = "Error occured while updating employee.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while updating employee.Try again" };
        }

        public Status DeleteEmployee(int employeeid)
        {
            try
            {
                Employee employee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employeeid);
                if (employee != null)
                {
                    BankAppDbctx.Employees.Remove(employee);
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch (Exception) { }
            return new Status() { IsSuccess = false, Message = "Error occured while deleting employee.Try again" };
        }

        public Status UpdateAccountHolder(AccountHolder accountholder)
        {
            try
            {
                var oldaccountholder = BankAppDbctx.AccountHolders.FirstOrDefault(_ => _.Id == accountholder.Id);
                if(oldaccountholder != null)
                {
                    oldaccountholder.Name = accountholder.Name == string.Empty ? oldaccountholder.Name : accountholder.Name;
                    oldaccountholder.PhoneNumber = accountholder.PhoneNumber == default(long) ? oldaccountholder.PhoneNumber : accountholder.PhoneNumber;
                    oldaccountholder.Address = accountholder.Address == string.Empty ? oldaccountholder.Address : accountholder.Address;
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully updated" };
                }
                else
                {
                    Console.WriteLine("User not found");
                    return new Status() { IsSuccess = false, Message = "Error occured while updating account holder.Try again" };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Status() { IsSuccess = false, Message = "Error occured while updating account holder.Try again" };
            }
            
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
                else
                {
                    Console.WriteLine("Account holder not found");
                    return new Status() { IsSuccess = false, Message = "Error occured while deleting account holder.Try again" };
                }
            }
            catch (Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while deleting account holder.Try again" };
            }
            
        }

        public Status revertTransaction(int transId)
        {
            try
            {
                Transaction transaction = BankAppDbctx.Transactions.FirstOrDefault(trans => trans.Id == transId);
                if (transaction != null)
                {
                    AccountHolder senderAccountHolder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                    AccountHolder receiverAccountHolder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                    senderAccountHolder.Balance += transaction.Amount;
                    receiverAccountHolder.Balance -= transaction.Amount;
                    BankAppDbctx.Transactions.Add(transaction);
                    //receiverAccountHolder.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully transfered" };
                }
                else
                {
                    Console.WriteLine("Transaction not found");
                    return new Status() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
                }
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
            }
            
        }

        public Status AddBranch(Branch branch)
        {
            try
            {
                BankAppDbctx.Branches.Add(branch);
                BankAppDbctx.SaveChanges();
                return new Status() { IsSuccess = true, Message = "Successfully Added Branch" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while adding branch.Try again" };
            }
        }

        public Status DeleteBranch(Branch branch)
        {
            try
            {
                if (branch != null)
                {
                    BankAppDbctx.Branches.Remove(branch);
                    BankAppDbctx.SaveChanges();
                    return new Status() { IsSuccess = true, Message = "Successfully deleted" };
                }
                else
                {
                    Console.WriteLine("Branch not found");
                    return new Status() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
                }
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
            }
            
        }

        public Status AddCurrency(string currencyCode, double exchangeRate,Bank bank)
        {
            try
            {
                BankAppDbctx.CurrencyCodes.Add(new CurrencyCode() {Code = currencyCode, ExchangeRate = exchangeRate ,IsDefault = false ,BankId = bank.Id});
                BankAppDbctx.SaveChanges();
                return new Status() { IsSuccess = true, Message = "Successfully currency added" };
            }
            catch(Exception)
            {
                
            }
            return new Status() { IsSuccess = false, Message = "Error occured while adding currency.Try again" };
        }
            

        public bool UpdateCharges(Bank bank)
        {
            try
            {
                Bank oldbank = BankAppDbctx.Banks.FirstOrDefault(bank => bank.Id == bank.Id);
                if (oldbank != null)
                {
                    bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank == default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                    bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank == default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                    bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank == default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                    bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank == default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                    BankAppDbctx.SaveChanges();
                    return true;
                }
                else
                {
                    Console.WriteLine("Bank not found");
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
