using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    public class BankServices
    {
        public bool SetUpBank(Bank bank)
        {
            if (string.IsNullOrEmpty(bank.BranchName))
            {
                throw new Exception("Empty Name is Not valid!");
            }

            if (string.IsNullOrEmpty(bank.IFSCCode))
            {
                throw new Exception("Empty IFSCCode is Not valid!");
            }

            if (string.IsNullOrEmpty(bank.BankName))
            {
                throw new Exception("Empty Name is Not valid!");
            }

            BankDatabase.Banks.Add(bank);
            return true;
        }

        public string Register(Bank bank,BankAccount bankaccount)
        {
            if (string.IsNullOrEmpty(bankaccount.Name))
            {
                throw new Exception("Empty Name is Not valid!");
            }

            if (bank.BankAccounts.Count != 0 && bank.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                throw new Exception("Account already exists!");
            }

            if (string.IsNullOrEmpty(bankaccount.PhoneNumber))
            {
                throw new Exception("Empty Phone Number is not valid!");
            }

            if (string.IsNullOrEmpty(bankaccount.Address))
            {
                throw new Exception("Empty Address is Not valid!");
            }
            return bankaccount.Id;
        }


        public BankAccount Login(Bank bank,string userid, string password)
        {
            BankAccount user = null;
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userid && i.Password == password)
                {
                    user = i;
                }
            }
            return user;
        }

        public BankAccount BankStaffLogin(Bank bank,string userid,string password)
        {
            BankAccount user = null;
            foreach(var i in bank.Employees)
            {
                if (i.Id == userid && i.Password == password)
                {
                    user = i;
                }
            }
            return user;
        }

        public string Deposit(Bank bank,string userId, double amt)
        {

            string txnid = "TXN";
            foreach (var i in bank.BankAccounts)
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

        public string Withdraw (Bank bank, string userId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in bank.BankAccounts)
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


        public string Transfer(Bank bank,string recuserId, string snduserId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in bank.BankAccounts)
            {
                foreach (var j in bank.BankAccounts)
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

        public double ViewBalance(Bank bank,string userId)
        {
            double bal = 0.0;
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userId)
                {
                    bal = i.Balance;
                }
            }
            return bal;
        }

        public void ViewTransactions(Bank bank,string userId)
        {
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userId)
                {
                    foreach (var j in BankDatabase.TransList[i.Id])
                    {
                        Console.WriteLine(j.SenderAccountId + "to" + j.RecieverAccountId + "of" + j.Amount);
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
    }
}


