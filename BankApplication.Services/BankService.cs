using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class BankService
    {
        public Status SetUpBank(Branch branch,Bank bank, Employee admin)
        {
            Status status = new Status();
            try
            {
                bank.Id = bank.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                Console.WriteLine(bank.Id);
                branch.Id = branch.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                branch.IsMainBranch = true;

                admin.EmployeeId = admin.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                admin.Role = "Admin";
                bank.Employees.Add(admin);

                BankDatabase.Banks.Add(bank);
                bank.Branches.Add(branch);
                status.IsSuccess = true;
            }
            catch (Exception)
            {
                status.IsSuccess = false;
                status.Message = "Unable to create a bank...!";
            }

            return status;
        }

        public Status Register(Bank bank, AccountHolder AccountHolder)
        {
            if (bank.AccountHolders.Count != 0 && bank.AccountHolders.Any(p => p.Name == AccountHolder.Name) == true)
            {
                return new Status() { IsSuccess = false, Message = "Account already exists!" };
            }
            AccountHolder.Id = AccountHolder.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            AccountHolder.Type = UserType.AccountHolder;
            bank.AccountHolders.Add(AccountHolder);
            return new Status() { IsSuccess = true, Message = "Account Created successfully...!" };
        }

        public string EmployeeRegister(Employee employee,Bank bank)
        {
            try
            {
                employee.Type = UserType.Employee;
                employee.EmployeeId = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                bank.Employees.Add(employee);
                return employee.EmployeeId;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public string Deposit(AccountHolder accountholder, double amt)
        {
            Transaction transaction = new Transaction()
            {
                SrcAccId = accountholder.Id,
                DestAccId = accountholder.Id,
                Amount = amt,
                CreatedBy = accountholder.Id,
                CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Id = "TXN" + accountholder.Id + accountholder.Id + accountholder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = TransactionType.Credit
            };
            accountholder.Balance += amt;
            accountholder.Transactions.Add(transaction);
            return transaction.Id;
        }

        public string Withdraw(AccountHolder AccountHolder, double amt)
        {
            string txnid = "TXN";
            if (AccountHolder.Balance >= amt)
            {
                AccountHolder.Balance -= amt;
                Transaction transaction = new Transaction()
                {
                    SrcAccId = AccountHolder.Id,
                    DestAccId = AccountHolder.Id,
                    Amount = amt,
                    CreatedBy = AccountHolder.Id,
                    CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Id = "TXN" + AccountHolder.Id + AccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Debit
            };
                
                AccountHolder.Transactions.Add(transaction);
                txnid = transaction.Id;
            }
            return txnid;
        }


        public string Transfer(AccountHolder AccountHolder,AccountHolder recevierAccountHolder, double amt,Charges charge)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (recevierAccountHolder != null)
            {
                recevierAccountHolder.Balance += amt;
                transaction.SrcAccId = AccountHolder.Id;
                transaction.DestAccId = recevierAccountHolder.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = AccountHolder.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + AccountHolder.Id + recevierAccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;
                txnid = transaction.Id;
                //AccountHolder.Balance = AccountHolder.BankId == recevierAccountHolder.BankId ? (charge == Charges.RTGS ? (AccountHolder.Balance >= amt ? amt : ))

                if (AccountHolder.BankId == recevierAccountHolder.BankId)
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt ? AccountHolder.Balance - amt : 0) : (AccountHolder.Balance >= amt + (0.05 * amt) ? AccountHolder.Balance - (amt + (0.05 * amt)) : 0);
                    AccountHolder.Transactions.Add(transaction);
                    recevierAccountHolder.Transactions.Add(transaction);
                }
                else
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt + (0.02 * amt) ? AccountHolder.Balance - (amt + (0.02 * amt)) : 0) : (AccountHolder.Balance >= amt + (0.06 * amt) ? AccountHolder.Balance -= (amt + (0.06 * amt)) : 0);
                    AccountHolder.Transactions.Add(transaction);
                    recevierAccountHolder.Transactions.Add(transaction);
                }
            }
            return txnid;
        }

        public AccountHolder GetAccountHolder(string accountid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == accountid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == accountid) : null;
            return accountholder;
        }

        public Employee GetEmployee(string employeeid,string bankid)
        {
            Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid && employee.Type == UserType.Employee));
            Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid && employee.Type == UserType.Employee) : null;
            return employee;
        }

        public double ViewBalance(AccountHolder AccountHolder)
        {
            return AccountHolder.Balance;
        }

        public void ViewAllBankBranches(Bank bank)
        {
            foreach (var i in bank.Branches)
            {
                Console.WriteLine(i.Id + "  -  " + i.Name);
            }
        }

        public void ViewAllAccounts(Bank bank)
        { 
            foreach (var i in bank.AccountHolders)
            {
                Console.WriteLine(i.Id + "   -   " + i.Name+"   -   "+i.PhoneNumber+"   -   "+i.Address);
            }
        }

        public void ViewAllEmployees(string bankid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            foreach(var i in bank.Employees)
            {
                Console.WriteLine(i.Id + "   -   " + i.Name + "   -   " + i.PhoneNumber + "   -   " + i.Address);
            }
        }

        public string EmployeeDeposit(AccountHolder AccountHolder, double amt)
        {
            Transaction transaction = new Transaction()
            {
                SrcAccId = AccountHolder.Id,
                DestAccId = AccountHolder.Id,
                Amount = amt,
                CreatedBy = AccountHolder.Id,
                CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Id = "TXN" + AccountHolder.Id + AccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = TransactionType.Credit
            };
            AccountHolder.Balance += amt;
            AccountHolder.Transactions.Add(transaction);
            return transaction.Id;
        }

        public string EmployeeWithdraw(AccountHolder AccountHolder, double amt)
        {
            if (AccountHolder.Balance >= amt)
            {
                AccountHolder.Balance -= amt;
                Transaction transaction = new Transaction()
                {
                    SrcAccId = AccountHolder.Id,
                    DestAccId = AccountHolder.Id,
                    Amount = amt,
                    CreatedBy = AccountHolder.Id,
                    CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Id = "TXN" + AccountHolder.Id + AccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Debit
                };
                AccountHolder.Transactions.Add(transaction);
                return transaction.Id;
            }
            return null;
        }

        public string EmployeeTransfer(AccountHolder AccountHolder, AccountHolder recevierAccountHolder, double amt, Charges charge)
        {
            Transaction transaction = new Transaction();
            if (AccountHolder != null && recevierAccountHolder != null)
            {
                recevierAccountHolder.Balance += amt;
                transaction.SrcAccId = AccountHolder.Id;
                transaction.DestAccId = recevierAccountHolder.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = AccountHolder.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + AccountHolder.Id + recevierAccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;

                if (AccountHolder.BankId == recevierAccountHolder.BankId)
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt ? AccountHolder.Balance - amt : 0) : (AccountHolder.Balance >= amt + (0.05 * amt) ? AccountHolder.Balance - (amt + (0.05 * amt)) : 0);
                    AccountHolder.Transactions.Add(transaction);
                    recevierAccountHolder.Transactions.Add(transaction);
                    return transaction.Id;
                }
                else
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt + (0.02 * amt) ? AccountHolder.Balance - (amt + (0.02 * amt)) : 0) : (AccountHolder.Balance >= amt + (0.06 * amt) ? AccountHolder.Balance -= (amt + (0.06 * amt)) : 0);
                    AccountHolder.Transactions.Add(transaction);
                    recevierAccountHolder.Transactions.Add(transaction);
                    return transaction.Id;
                }
            }
            return null;
        }
    }
}


