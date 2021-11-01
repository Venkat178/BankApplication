using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    public class BankStaff
    {
        public void CreateAccount(Bank bank,BankAccount bankaccount)
        {
            if (string.IsNullOrEmpty(bankaccount.Name))
            {
                throw new Exception("Empty Name is not valid!");
            }

            if (bank.BankAccounts.Count != 0 && bank.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                throw new Exception("Account already exists!");
            }
            //bank.Employees.Add(employee);
            //bank.BankAccounts.Add(employee);
        }
        
        public void UpdateName(Bank bank,string userid,string HolderName)
        {
            foreach(var i in bank.BankAccounts)
            {
                if (i.Id==userid)
                {
                    i.Name = HolderName;
                }
            }
        }

        public void UpdatePhoneNumber(Bank bank, string userid, string phonenumber)
        {
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.PhoneNumber = phonenumber;
                }
            }
        }

        public void UpdateGender(Bank bank, string userid, GenderType gender)
        {
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.Gender = gender;
                }
            }
        }

        public void UpdateAdress(Bank bank, string userid, string address)
        {
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.Address = address;
                }
            }
        }

        public void DeleteAccount(Bank bank, string userid)
        {
            foreach (var i in bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    bank.BankAccounts.Remove(i);
                }
            }
        }

        public void revertTransaction(Bank bank, string transId)
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
                    foreach (var j in bank.BankAccounts)
                    {
                        foreach (var k in bank.BankAccounts)
                        {
                            if ((j.Id == senderId) & (k.Id == reciverId))
                            {
                                j.Balance += i.Amount;
                                k.Balance -= i.Amount;
                                Transaction trans = new Transaction(senderId, reciverId, i.Amount, j.BankId, TransactionType.Debited);
                                Transaction trans1 = new Transaction(senderId, reciverId, i.Amount, j.BankId, TransactionType.Credited);
                                BankDatabase.TransList[senderId].Add(trans);
                                BankDatabase.TransList[reciverId].Add(trans);
                            }
                        }
                    }
                }
            }
        }

        public string returnId(Bank bank, string HolderName)
        {
            string id = "";
            foreach (var i in bank.BankAccounts)
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
