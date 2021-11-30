using System;
using System.Linq;
using BankApplication.Models;

namespace BankApplication.Services
{
    public class EmployeeService
    {
        public void UpdateEmployee(Employee employee)
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
                }
            }
            catch (Exception ex) { }
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

        public bool UpdateAccountHolder(AccountHolder bankaccount)
        {
            try
            {
                var bank = BankDatabase.Banks.Find(bank => bank.Id == bankaccount.BankId);
                if (bank != null)
                {
                    AccountHolder oldbankaccount = bank.AccountHolders.Find(account => account.Id == bankaccount.Id);
                    if (oldbankaccount != null)
                    {
                        oldbankaccount.Name = bankaccount.Name != null ? oldbankaccount.Name : bankaccount.Name;
                        oldbankaccount.PhoneNumber = bankaccount.PhoneNumber != default(int) ? oldbankaccount.PhoneNumber : bankaccount.PhoneNumber;
                        oldbankaccount.Address = bankaccount.Address != null ? oldbankaccount.Address : bankaccount.Address;
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
            bool Isdeleted = false;
            AccountHolder bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                BankDatabase.BankAccounts.Remove(bankaccount);
                Isdeleted = true;
            }
            return Isdeleted;
        }

        public string Deposit(BankAccount bankaccount, double amt)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction()
            {
                SrcAccId = bankaccount.Id,
                DestAccId = bankaccount.Id,
                Amount = amt,
                CreatedBy = bankaccount.Id,
                CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = TransactionType.Credit
            };
            bankaccount.Balance += amt;
            bankaccount.Transactions.Add(transaction);
            txnid = transaction.Id;
            return txnid;
        }

        public string Withdraw(BankAccount bankaccount, double amt)
        {
            string txnid = "TXN";
            if (bankaccount.Balance >= amt)
            {
                bankaccount.Balance -= amt;
                Transaction transaction = new Transaction()
                {
                    SrcAccId = bankaccount.Id,
                    DestAccId = bankaccount.Id,
                    Amount = amt,
                    CreatedBy = bankaccount.Id,
                    CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Debit
                };
                bankaccount.Transactions.Add(transaction);
                txnid = transaction.Id;
            }
            return txnid;
        }

        public string Transfer(BankAccount bankaccount, BankAccount recevierbankaccount, double amt, Charges charge)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (bankaccount != null && recevierbankaccount != null)
            {
                recevierbankaccount.Balance += amt;
                transaction.SrcAccId = bankaccount.Id;
                transaction.DestAccId = recevierbankaccount.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = bankaccount.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + bankaccount.Id + recevierbankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;
                txnid = transaction.Id;
                if (bankaccount.BankId == recevierbankaccount.BankId)
                {
                    bankaccount.Balance = charge == Charges.RTGS ? (bankaccount.Balance >= amt ? bankaccount.Balance - amt : 0) : (bankaccount.Balance >= amt + (0.05 * amt) ? bankaccount.Balance - (amt + (0.05 * amt)) : 0);
                    bankaccount.Transactions.Add(transaction);
                    recevierbankaccount.Transactions.Add(transaction);
                }
                else
                {
                    bankaccount.Balance = charge == Charges.RTGS ? (bankaccount.Balance >= amt + (0.02 * amt) ? bankaccount.Balance - (amt + (0.02 * amt)) : 0) : (bankaccount.Balance >= amt + (0.06 * amt) ? bankaccount.Balance -= (amt + (0.06 * amt)) : 0);
                    bankaccount.Transactions.Add(transaction);
                    recevierbankaccount.Transactions.Add(transaction);
                }
            }
            return txnid;
        }

        public void ViewTransactions(string userid)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            foreach (var i in bankaccount.Transactions)
            {
                Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
            }
        }

        public void revertTransaction(string transId)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(b => b.Transactions.Any(transaction => transaction.Id == transId));
            Transaction transaction = bankaccount != null ? bankaccount.Transactions.Find(transaction => transaction.Id == transId) : null;
            if (transaction != null)
            {
                BankAccount senderbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.SrcAccId);
                BankAccount receiverbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == transaction.DestAccId);
                senderbankaccount.Balance += transaction.Amount;
                receiverbankaccount.Balance -= transaction.Amount;
                senderbankaccount.Transactions.Add(transaction);
                receiverbankaccount.Transactions.Add(transaction);

            }
        }

        public void AddBranch(Branch branch, Bank bank)
        {
            branch.Id = bank.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
            bank.Branches.Add(branch);
        }

        public void DeleteBranch(Bank bank, Branch branch)
        {
            if (branch != null)
            {
                bank.Branches.Remove(branch);
            }
        }

        public void AddCurrency(string currencyCode, double exchangeRate, Bank bank)
        {
            bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate });
        }

        public void UpdateCharges()
        {
            //bank.CurrencyCodes
        }
    }
}
