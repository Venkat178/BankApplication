using System;
using System.Linq;
using BankApplication.Models;

namespace BankApplication.Services
{
    public class EmployeeService
    {
        public Status UpdateEmployee(Employee employee,string employeeid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid));
                var oldemployee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
                if (oldemployee != null)
                {
                    oldemployee.Name = employee.Name == string.Empty ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber == default(long) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = employee.Address == string.Empty ? oldemployee.Address : employee.Address;
                    return new Status() { IsSuccess = true, Message = "Succefully updated" };
                }
            }
            catch (Exception) 
            {
                return new Status() { IsSuccess = false, Message = "Error occured while updating employee.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while updating employee.Try again" };
        }

        public Status DeleteEmployee(string employeeid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid));
                Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
                if (employee != null)
                {
                    bank.Employees.Remove(employee);
                    return new Status() { IsSuccess = true, Message = "Succefully deleted" };
                }
            }
            catch (Exception) { }
            return new Status() { IsSuccess = false, Message = "Error occured while deleting employee.Try again" };
        }

        public Status UpdateAccountHolder(AccountHolder AccountHolder,string accountholderid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(b => b.AccountHolders.Any(accountholder => accountholder.Id == accountholderid));
                var oldaccountholder = bank != null ? bank.AccountHolders.Find(employee => employee.Id == accountholderid) : null;
                oldaccountholder.Name = AccountHolder.Name == string.Empty ? oldaccountholder.Name : AccountHolder.Name;
                oldaccountholder.PhoneNumber = AccountHolder.PhoneNumber == default(long) ? oldaccountholder.PhoneNumber : AccountHolder.PhoneNumber;
                oldaccountholder.Address = AccountHolder.Address == string.Empty ? oldaccountholder.Address : AccountHolder.Address;
                return new Status() { IsSuccess = true, Message = "Succefully updated" };
            }
            catch (Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while updating account holder.Try again" };
            }
        }

        public Status DeleteAccountHolderAccount(string userid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
                AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
                if (accountholder != null)
                {
                    bank.AccountHolders.Remove(accountholder);
                    return new Status() { IsSuccess = true, Message = "Succefully deleted" };
                }
            }
            catch (Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while deleting account holder.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while deleting account holder.Try again" };
        }

        public Status revertTransaction(string transId)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Transactions.Any(trans => trans.Id == transId)));
                AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Transactions.Any(trans => trans.Id == transId)) : null;
                Transaction transaction = accountholder != null ? accountholder.Transactions.Find(trans => trans.Id == transId) : null;
                if (transaction != null)
                {
                    AccountHolder senderAccountHolder = bank.AccountHolders.Find(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                    AccountHolder receiverAccountHolder = bank.AccountHolders.Find(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                    senderAccountHolder.Balance += transaction.Amount;
                    receiverAccountHolder.Balance -= transaction.Amount;
                    senderAccountHolder.Transactions.Add(transaction);
                    receiverAccountHolder.Transactions.Add(transaction);
                    return new Status() { IsSuccess = true, Message = "Successfully transfered" };
                }
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
        }

        public Status AddBranch(Branch branch, Bank bank)
        {
            try
            {
                branch.Id = bank.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                bank.Branches.Add(branch);
                return new Status() { IsSuccess = true, Message = "Successfully Added Branch" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while adding branch.Try again" };
            }
        }

        public Status DeleteBranch(Bank bank, Branch branch)
        {
            try
            {
                if (branch != null)
                {
                    bank.Branches.Remove(branch);
                    return new Status() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
            }
            return new Status() { IsSuccess = false, Message = "Error occured while deleting branch.Try again" };
        }

        public Status AddCurrency(string currencyCode, double exchangeRate, Bank bank)
        {
            try
            {
                bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate ,IsDefault = false });
                return new Status() { IsSuccess = true, Message = "Successfully currency added" };
            }
            catch(Exception)
            {
                return new Status() { IsSuccess = false, Message = "Error occured while adding currency.Try again" };
            }
        }
            

        public bool UpdateCharges(Bank bank)
        {
            try
            {
                Bank oldbank = BankDatabase.Banks.Find(bank => bank.Id == bank.Id);
                bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank == default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank == default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank == default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank == default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
