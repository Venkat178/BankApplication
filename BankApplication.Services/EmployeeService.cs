using System;
using System.Linq;
using BankApplication.Models;

namespace BankApplication.Services
{
    public class EmployeeService
    {
        public bool UpdateEmployee(Employee employee)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employee.Id));
                var oldemployee = bank != null ? bank.Employees.Find(employee => employee.Id == employee.Id) : null;
                if (oldemployee != null)
                {
                    oldemployee.Name = employee.Name != null ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber != default(int) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = employee.Address != null ? oldemployee.Address : employee.Address;
                    return true;
                }
            }
            catch (Exception ex) 
            {
                return false;
            }
            return false;
        }

        public bool DeleteEmployee(string employeeid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid));
                Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid) : null;
                if (employee != null)
                {
                    bank.Employees.Remove(employee);
                    return true;
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public bool UpdateAccountHolder(AccountHolder AccountHolder)
        {
            try
            {
                var bank = BankDatabase.Banks.Find(bank => bank.Id == AccountHolder.BankId);
                if (bank != null)
                {
                    AccountHolder oldAccountHolder = bank.AccountHolders.Find(account => account.Id == AccountHolder.Id);
                    if (oldAccountHolder != null)
                    {
                        oldAccountHolder.Name = AccountHolder.Name != null ? oldAccountHolder.Name : AccountHolder.Name;
                        oldAccountHolder.PhoneNumber = AccountHolder.PhoneNumber != default(int) ? oldAccountHolder.PhoneNumber : AccountHolder.PhoneNumber;
                        oldAccountHolder.Address = AccountHolder.Address != null ? oldAccountHolder.Address : AccountHolder.Address;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool DeleteAccountHolderAccount(string userid)
        {
            try
            {
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
                AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
                if (accountholder != null)
                {
                    bank.AccountHolders.Remove(accountholder);
                    return true;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            return false;
        }

        public void ViewTransactions(string userid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
            if(accountholder != null)
            {
                foreach (var i in accountholder.Transactions)
                {
                    Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
                }
            }
            
        }

        public bool revertTransaction(string transId)
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
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }

        public bool AddBranch(Branch branch, Bank bank)
        {
            try
            {
                branch.Id = bank.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                bank.Branches.Add(branch);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }

        public bool DeleteBranch(Bank bank, Branch branch)
        {
            try
            {
                if (branch != null)
                {
                    bank.Branches.Remove(branch);
                    return true;
                }
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }

        public bool AddCurrency(string currencyCode, double exchangeRate, Bank bank)
        {
            try
            {
                bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate ,IsDefault = false });
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            return false;
        }
            

        public bool UpdateCharges(Bank bank)
        {
            try
            {
                Bank oldbank = BankDatabase.Banks.Find(bank => bank.Id == bank.Id);
                bank.RTGSChargesforSameBank = bank.RTGSChargesforSameBank != default(int) ? oldbank.RTGSChargesforSameBank : bank.RTGSChargesforSameBank;
                bank.RTGSChargesforDifferentBank = bank.RTGSChargesforDifferentBank != default(int) ? oldbank.RTGSChargesforDifferentBank : bank.RTGSChargesforDifferentBank;
                bank.IMPSChargesforSameBank = bank.IMPSChargesforSameBank != default(int) ? oldbank.IMPSChargesforSameBank : bank.IMPSChargesforSameBank;
                bank.IMPSChargesforDifferentBank = bank.IMPSChargesforDifferentBank != default(int) ? oldbank.IMPSChargesforDifferentBank : bank.IMPSChargesforDifferentBank;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }
    }
}
