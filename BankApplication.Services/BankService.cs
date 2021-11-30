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
                bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "INR", ExchangeRate = 1, IsDefault = true });
                bank.IMPSChargesforSameBank = 5;
                bank.RTGSChargesforSameBank = 0;
                bank.IMPSChargesforDifferentBank = 6;
                bank.RTGSChargesforDifferentBank = 2;

                branch.Id = branch.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                branch.IsMainBranch = true;

                admin.Id = admin.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
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

        public Status Register(Bank bank, BankAccount bankaccount)
        {
            if (BankDatabase.BankAccounts.Count != 0 && BankDatabase.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                return new Status() { IsSuccess = false, Message = "Account already exists!" };
            }
            bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            bankaccount.Type = UserType.AccountHolder;
            bank.BankAccounts.Add(bankaccount);
            BankDatabase.BankAccounts.Add(bankaccount);
            bankaccount.Id;
            return new Status() { IsSuccess = true, Message = "Account Created successfully...!" };
        }

        public string EmployeeRegister(Employee employee,Bank bank)
        {
            employee.Type = UserType.Employee;
            employee.Id = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            bank.Employees.Add(employee);
            return employee.Id;
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


        public string Transfer(BankAccount bankaccount,BankAccount recevierbankaccount, double amt,Charges charge)
        {
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (recevierbankaccount != null)
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
                //bankaccount.Balance = bankaccount.BankId == recevierbankaccount.BankId ? (charge == Charges.RTGS ? (bankaccount.Balance >= amt ? amt : ))

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

        public AccountHolder GetAccountHolder(string accountid)
        {
            AccountHolder bankaccount = BankDatabase.Banks.Find(bankaccount => bankaccount.Id == accountid);
            return bankaccount;
        }

        public Employee GetEmployee(string employeeid,string bankid)
        {
            Bank bank = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == employeeid && employee.Type == UserType.Employee));
            Employee employee = bank != null ? bank.Employees.Find(employee => employee.Id == employeeid && employee.Type == UserType.Employee) : null;
            return employee;
        }

        public double ViewBalance(BankAccount bankaccount)
        {
            return bankaccount.Balance;
        }

        public void ViewAllBankBranches(string Bankid)
        {
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == Bankid);
            foreach(var i in bank.Branches)
            {
                Console.WriteLine(i.Id + "  -  " + i.Name);
            }
        }

        public void ViewAllAccounts()
        {
            foreach(var i in BankDatabase.BankAccounts)
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
    }
}


