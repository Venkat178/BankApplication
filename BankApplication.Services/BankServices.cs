using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    public class BankServices
    {
        public void CreateBank(Bank bank, string branchname)
        {
            bank.Id = "THB" + BankDatabase.Banks.Count + 1;
            bank.IFSCCode = "THB";
            bank.BranchName = branchname;
            bank.BankName = "TechBank";
            BankDatabase.Banks.Add(bank);
        }

        public void CreateAccount(string HolderName, string pass, string branchname, string phnnumber, string Adrs, GenderType gnd)
        {
            if (string.IsNullOrEmpty(HolderName))
            {
                throw new Exception("Empty Name is not valid!");
            }

            if (Bank.BankAccounts.Count != 0 && Bank.BankAccounts.Any(p => p.Name == HolderName) == true)
            {
                throw new Exception("Account already exists!");
            }

            BankAccount bankAccount = new BankAccount();
            BankAccount b = bankAccount;
            b.Name = HolderName;
            b.Id = branchname + (Bank.BankAccounts).Count.ToString();
            b.BranchName = branchname;
            foreach (var i in BankDatabase.Banks)
            {
                if (i.BranchName == branchname)
                {
                    b.BankId = i.Id;
                }
            }
            b.Password = pass;
            b.Balance = 0.0;
            b.PhoneNumber = phnnumber;
            b.Address = Adrs;
            b.Gender = gnd;
            Bank.BankAccounts.Add(b);
        }

        public void deleteAccount(string userid)
        {
            foreach (var i in Bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    Bank.BankAccounts.Remove(i);
                }
            }
        }

        public void revertTransaction(string transId)
        {
            int word1 = transId.IndexOf("TB");
            int word2 = transId.IndexOf("TB", word1 + 1);
            int word3 = transId.IndexOf("THB");
            string senderId = transId.Substring(word1, (word2 - word1));
            string reciverId = transId.Substring(word2, (word3 - word2));
            foreach (var i in BankDatabase.TransList[senderId])
            {
                if ((i.SenderAccountId == senderId) & (i.RecieverAccountId == reciverId) & (i.TransactionId == transId))
                {
                    foreach (var j in Bank.BankAccounts)
                    {
                        foreach (var k in Bank.BankAccounts)
                        {
                            if ((j.Id == senderId) & (k.Id == reciverId))
                            {
                                j.Balance += i.Amount;
                                k.Balance -= i.Amount;
                                Transactions trans = new Transactions(senderId, reciverId, i.Amount, j.BankId, TransactionType.Debited);
                                Transactions trans1 = new Transactions(senderId, reciverId, i.Amount, j.BankId, TransactionType.Credited);
                            }
                        }
                    }
                }
            }
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
    }
}


