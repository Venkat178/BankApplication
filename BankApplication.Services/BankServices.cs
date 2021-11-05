using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    public class BankServices
    {
        Bank Bank;
        //Admin AdminObj;

        public BankServices()
        {
            this.Bank = new Bank();
        }
        public Status SetUpBank(Bank bank)
        {
            Status status = new Status();
            bank.Id = bank.BankName + DateTime.Now.ToString("yyyyMMddHHmmss");
            bank.CurrencyCode = "INR";
            status.IsSuccess = true;
            return status;
        }

        public bool IsEmployee(string userid)
        {
            foreach(var i in BankDatabase.Employees)
            {
                if(i.Id==userid)
                {
                    return true;
                }
            }
            return false;
            //bool Isemployee = BankDatabase.Employees.Any(s => s.Id == userid);
            //Console.WriteLine(Isemployee);
            //return Isemployee;
        }

        public bool IsUser(string userid)
        {
            foreach(var i in BankDatabase.BankAccounts)
            {
                if(i.Id==userid)
                {
                    return true;
                }
            }
            return false;
            //bool Isuser = BankDatabase.BankAccounts.Any(s => s.Id == userid);
            //Console.WriteLine(Isuser);
            //return Isuser;
        }


        public string Register(BankAccount bankaccount)
        {
            try
            {
                if (BankDatabase.BankAccounts.Count != 0 && BankDatabase.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
                {
                    throw new Exception("Account already exists!");
                }
                int flag = 0;
                foreach (var i in BankDatabase.Banks)
                {
                    if (i.BranchName == bankaccount.BranchName)
                    {
                        flag = 1;
                        bankaccount.BankId = i.Id;
                        Bank = i;
                        break;
                    }
                }
                if (flag == 0)
                {
                    throw new Exception("No bank found");
                }
                bankaccount.Type = EnumHolderType.AccountHolder;
                Console.WriteLine(bankaccount.BankId);
                if (bankaccount.BankId == string.Empty)
                {
                    throw new Exception("No bank found");
                }
                bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                Bank.BankAccounts.Add(bankaccount);
                BankDatabase.BankAccounts.Add(bankaccount); 
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bankaccount.Id;
        }


        public BankAccount Login(string userid)
        {
            BankAccount user = null;
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userid)
                {
                    user = i;
                }
            }
            return user;
        }

        public Employee EmployeeLogin(string userid)
        {
            Employee user = null;
            foreach(var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    user = i;
                }
            }
            return user;
        }

        public string Deposit( string userId,double amt)
        {

            string txnid = "TXN";
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userId)
                {
                    i.Balance += amt;
                    Transaction trans = new Transaction(userId, userId, amt, i.BankId, TransactionType.Credited);
                    BankDatabase.TransList[userId].Add(trans);
                    txnid = trans.TransactionId;
                }
            }
            return txnid;
        }

        public string Withdraw (string userId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userId)
                {
                    if (i.Balance >= amt)
                    {
                        i.Balance -= amt;
                        Transaction trans = new Transaction(userId, userId, amt, i.BankId, TransactionType.Debited);
                        BankDatabase.TransList[userId].Add(trans);
                        txnid = trans.TransactionId;
                    }
                    else
                    {
                        throw new OutofMoneyException("You do not have sufficient money .");
                    }
                }
            }
            return txnid;
        }


        public string Transfer(string snduserId, string recuserId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in BankDatabase.BankAccounts)
            {
                foreach (var j in BankDatabase.BankAccounts)
                {
                    if (i.Id == snduserId && j.Id == recuserId)
                    {
                        if (i.Balance >= amt)
                        {
                            i.Balance -= amt;
                            j.Balance += amt;
                            Transaction trans = new Transaction(snduserId, recuserId, amt, i.BankId, TransactionType.Debited);
                            Transaction trans1 = new Transaction(snduserId, recuserId, amt, i.BankId, TransactionType.Credited);
                            BankDatabase.TransList[snduserId].Add(trans);
                            BankDatabase.TransList[recuserId].Add(trans1);
                            txnid = trans.TransactionId;
                        }
                        else
                        {
                            Console.WriteLine("Insufficient Amount to Transfer");
                        }
                    }
                }
            }
            return txnid;
        }

        public double ViewBalance(string userId)
        {
            double bal = 0.0;
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userId)
                {
                    bal = i.Balance;
                }
            }
            return bal;
        }

        public void ViewTransactions(string userId)
        {
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userId)
                {
                    foreach (var j in BankDatabase.TransList[i.Id])
                    {
                        Console.WriteLine(j.SenderAccountId + " to " + j.RecieverAccountId + " of " + j.Amount);
                    }
                }
            }
        }

        /*public void logout()
        {
            BankAccount b = null;
        }*/

        public void ViewAllBankBranches()
        {
            foreach(var i in BankDatabase.Banks)
            {
                Console.WriteLine(i.BranchName);
            }
        }

        public void ViewAllAccounts(string Bankid)
        {
            foreach(var i in BankDatabase.Banks)
            {
                if(i.Id == Bankid)
                {
                    foreach(var j in i.BankAccounts)
                    {
                        Console.WriteLine(j.Name);
                    }
                }
            }
        }
    }
}


