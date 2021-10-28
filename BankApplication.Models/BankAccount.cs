using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
    public class BankAccount
    {
        public string BankId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string BranchName { get; set; }
        public string Id { get; set; }
        public double Balance { get; set; }
        public GenderType Gender { set; get; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public BankAccount Login(string userid, string pass)
        {
            BankAccount user = null;
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Id == userid && i.Password == pass)
                {
                    user = i;
                }
            }
            return user;
        }

        public string returnId(string HolderName)
        {
            string id = "";
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Name == HolderName)
                {
                    id = i.Id;
                }
            }
            return id;
        }

        public string Deposit(string userId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Id == userId)
                {
                    i.Balance += amt;
                    Transactions trans = new Transactions(userId, userId, amt, i.BankId, TransactionType.Credited);
                    BankDatabase.TransList[userId].Add(trans);
                    txnid = trans.TransactionId;
                }
            }
            return txnid;
        }

        public string Withdraw(string userId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Id == userId)
                {
                    if (i.Balance >= amt)
                    {
                        i.Balance -= amt;
                        Transactions trans = new Transactions(userId, userId, amt, i.BankId, TransactionType.Debited);
                        BankDatabase.TransList[userId].Add(trans);
                        txnid = trans.TransactionId;
                    }
                    else
                    {
                        Console.WriteLine("Insufficient Amount to Withdraw!");
                    }
                }
            }
            return txnid;
        }


        public string Transfer(string recuserId, string snduserId, double amt)
        {
            string txnid = "TXN";
            foreach (var i in Bank.BankAccounts)
            {
                foreach (var j in Bank.BankAccounts)
                {
                    if (i.Id == snduserId && j.Id == recuserId)
                    {
                        if (i.Balance >= amt)
                        {
                            i.Balance -= amt;
                            j.Balance += amt;
                            Transactions trans = new Transactions(snduserId, recuserId, amt, i.BankId, TransactionType.Debited);
                            Transactions trans1 = new Transactions(snduserId, recuserId, amt, i.BankId, TransactionType.Credited);
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
            foreach (var i in Bank.BankAccounts)
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
            foreach (var i in Bank.BankAccounts)
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

        public string ViewPassword(string userId)
        {
            string pass = "";
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Id == userId)
                {
                    pass = i.Password;
                }
            }
            return pass;
        }

        public void logout()
        {
            BankAccount b= null;
        }
    }
}


