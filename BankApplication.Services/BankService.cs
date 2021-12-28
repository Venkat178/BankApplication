using System;
using BankApplication.Models;
using BankApplication.Concerns;
using System.Linq;

namespace BankApplication.Services
{
    public class BankService : IBankService
    {
        public BankApplicationDbContext BankAppDbctx;

        public BankService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public Status SetUpBank(Branch branch,Bank bank, Employee admin)
        {
            Status status = new Status();
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

        public Status Register(Bank bank, AccountHolder AccountHolder)
        {
            if (BankAppDbctx.AccountHolders.Any(p => p.Name == AccountHolder.Name) == true)
            {
                return new Status() { IsSuccess = false, Message = "Account already exists!" };
            }
            AccountHolder.Type = UserType.AccountHolder;
            BankAppDbctx.AccountHolders.Add(AccountHolder);
            BankAppDbctx.SaveChanges();
            return new Status() { IsSuccess = true, Message = "Account Created successfully...!" };
        }

        public string EmployeeRegister(Employee employee)
        {
            try
            {
                employee.Type = UserType.Employee;
                employee.EmployeeId = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                BankAppDbctx.Employees.Add(employee);
                BankAppDbctx.SaveChanges();
                return employee.EmployeeId;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
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
                TransactionId = "TXN" + accountholder.Id + accountholder.Id + accountholder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = TransactionType.Credit
            };
            accountholder.Balance += amt;
            BankAppDbctx.Transactions.Add(transaction);
            BankAppDbctx.SaveChanges();
            return transaction.TransactionId;
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
                    TransactionId = "TXN" + AccountHolder.Id + AccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Debit
            };
                
                BankAppDbctx.Transactions.Add(transaction);
                BankAppDbctx.SaveChanges();
                txnid = transaction.TransactionId;
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
                transaction.TransactionId = "TXN" + AccountHolder.Id + recevierAccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;
                txnid = transaction.TransactionId;
                if (AccountHolder.BankId == recevierAccountHolder.BankId)
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt ? AccountHolder.Balance - amt : 0) : (AccountHolder.Balance >= amt + (0.05 * amt) ? AccountHolder.Balance - (amt + (0.05 * amt)) : 0);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                }
                else
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt + (0.02 * amt) ? AccountHolder.Balance - (amt + (0.02 * amt)) : 0) : (AccountHolder.Balance >= amt + (0.06 * amt) ? AccountHolder.Balance -= (amt + (0.06 * amt)) : 0);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                }
            }
            return txnid;
        }

        public AccountHolder GetAccountHolder(int accountid)
        {
            AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == accountid);
            return accountholder;
        }

        public Employee GetEmployee(int employeeid,int bankid)
        {
            Employee employee = BankAppDbctx.Employees.FirstOrDefault(employee => employee.Id == employeeid && employee.Type == UserType.Employee);
            return employee;
        }

        public double ViewBalance(AccountHolder AccountHolder)
        {
            return AccountHolder.Balance;
        }

        public string EmployeeDeposit(AccountHolder accountholder, double amt)
        {
            Transaction transaction = new Transaction()
            {
                SrcAccId = accountholder.Id,
                DestAccId = accountholder.Id,
                Amount = amt,
                CreatedBy = accountholder.Id,
                CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                TransactionId = "TXN" + accountholder.Id + accountholder.Id + accountholder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = TransactionType.Credit
            };
            accountholder.Balance += amt;
            BankAppDbctx.Transactions.Add(transaction);
            BankAppDbctx.AccountHolders.Update(accountholder);
            BankAppDbctx.SaveChanges();
            return transaction.TransactionId;
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
                    TransactionId = "TXN" + AccountHolder.Id + AccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Type = TransactionType.Debit
                };
                BankAppDbctx.Transactions.Add(transaction);
                BankAppDbctx.AccountHolders.Update(AccountHolder);
                BankAppDbctx.SaveChanges();
                return transaction.TransactionId;
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
                transaction.TransactionId = "TXN" + AccountHolder.Id + recevierAccountHolder.Id + AccountHolder.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Transfer;

                if (AccountHolder.BankId == recevierAccountHolder.BankId)
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt ? AccountHolder.Balance - amt : 0) : (AccountHolder.Balance >= amt + (0.05 * amt) ? AccountHolder.Balance - (amt + (0.05 * amt)) : 0);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return transaction.TransactionId;
                }
                else
                {
                    AccountHolder.Balance = charge == Charges.RTGS ? (AccountHolder.Balance >= amt + (0.02 * amt) ? AccountHolder.Balance - (amt + (0.02 * amt)) : 0) : (AccountHolder.Balance >= amt + (0.06 * amt) ? AccountHolder.Balance -= (amt + (0.06 * amt)) : 0);
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return transaction.TransactionId;
                }
            }
            return null;
        }
    }
}


