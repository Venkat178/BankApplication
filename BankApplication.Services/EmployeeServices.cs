using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    public class EmployeeServices
    {
        public string AccountHolderRegister(BankAccount bankaccount)
        {
            if (BankDatabase.BankAccounts.Count != 0 && BankDatabase.BankAccounts.Any(p => p.Name == bankaccount.Name) == true)
            {
                throw new Exception("Account already exists!");
            }
            foreach (var i in BankDatabase.Banks)
            {
                if (i.BranchName == bankaccount.BranchName)
                {
                    bankaccount.BankId = i.Id;
                }
            }
            bankaccount.Type = EnumHolderType.AccountHolder;
            Console.WriteLine(bankaccount.BankId);
            if (bankaccount.BankId == string.Empty)
            {
                throw new Exception("No bank found");
            }
            bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
            BankDatabase.BankAccounts.Add(bankaccount);
            return bankaccount.Id;
        }

        public void UpdateAccountHolderName(string userid,string HolderName)
        {
            foreach(var i in BankDatabase.BankAccounts)
            {
                if (i.Id==userid)
                {
                    i.Name = HolderName;
                }
            }
        }

        public void UpdateAccountHolderPhoneNumber(string userid, string phonenumber)
        {
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.PhoneNumber = phonenumber;
                }
            }
        }

        public void UpdateAccountHolderGender(string userid, GenderType gender)
        {
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.Gender = gender;
                }
            }
        }

        public void UpdateAccountHolderAddress(string userid, string address)
        {
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userid)
                {
                    i.Address = address;
                }
            }
        }

        public void DeleteAccountHolderAccount(string userid)
        {
            foreach (var i in BankDatabase.BankAccounts)
            {
                if (i.Id == userid)
                {
                    BankDatabase.BankAccounts.Remove(i);
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
                    foreach (var j in BankDatabase.BankAccounts)
                    {
                        foreach (var k in BankDatabase.BankAccounts)
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
            string id = string.Empty;
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
