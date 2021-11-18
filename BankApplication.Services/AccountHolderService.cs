﻿using System;
using BankApplication.Models;
using System.Linq;

namespace BankApplication.Services
{
    public class AccountHolderService
    {
        public void UpdateAccountHolderDetails(BankAccount bankaccount,string userid)
        {
            BankAccount oldbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            oldbankaccount.Name = bankaccount.Name == "no" ? oldbankaccount.Name : bankaccount.Name;
            oldbankaccount.PhoneNumber = bankaccount.PhoneNumber == "no" ? oldbankaccount.PhoneNumber : bankaccount.PhoneNumber;
            oldbankaccount.Address = bankaccount.Address == "no" ? oldbankaccount.Address : bankaccount.Address;
        }

        public void DeleteAccountHolderAccount(string userid)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                BankDatabase.BankAccounts.Remove(bankaccount);
            }
        }

        public string Deposit(string userid, double amt)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            bankaccount.Balance += amt;
            transaction.SrcAccId = bankaccount.Id;
            transaction.DestAccId = bankaccount.Id;
            transaction.Amount = amt;
            transaction.CreatedBy = bankaccount.Id;
            transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
            transaction.Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
            transaction.Type = TransactionType.Credit;
            bankaccount.Transactions.Add(transaction);
            txnid = transaction.Id;
            return txnid;
        }

        public string Withdraw(string userid, double amt)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            if (bankaccount.Balance >= amt)
            {
                bankaccount.Balance -= amt;
                transaction.SrcAccId = bankaccount.Id;
                transaction.DestAccId = bankaccount.Id;
                transaction.Amount = amt;
                transaction.CreatedBy = bankaccount.Id;
                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Id = "TXN" + bankaccount.Id + bankaccount.Id + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                transaction.Type = TransactionType.Debit;
                bankaccount.Transactions.Add(transaction);
                txnid = transaction.Id;
            }
            else
            {
                Console.WriteLine("You do not have sufficient money");
            }
            return txnid;
        }

        public string Transfer(string srcId,string destuserId,double amt,Charges charge)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == srcId);
            string txnid = "TXN";
            Transaction transaction = new Transaction();
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankacount => bankaccount.Id == destuserId);
            if(bankaccount!=null)
            {
                if(recevierbankaccount!=null)
                {
                    if (bankaccount.BankId == recevierbankaccount.BankId)
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (bankaccount.Balance >= amt)
                            {
                                bankaccount.Balance -= amt;
                                recevierbankaccount.Balance += amt;
                                transaction.SrcAccId = bankaccount.Id;
                                transaction.DestAccId = destuserId;
                                transaction.Amount = amt;
                                transaction.CreatedBy = bankaccount.Id;
                                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Id = "TXN" + bankaccount.Id + destuserId + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Type = TransactionType.Transfer;
                                bankaccount.Transactions.Add(transaction);
                                recevierbankaccount.Transactions.Add(transaction);
                                txnid = transaction.Id;
                            }
                            else
                            {
                                Console.WriteLine("You do not have sufficient money");
                            }
                        }
                        else
                        {
                            if (bankaccount.Balance >= amt+ (0.05 * amt))
                            {
                                bankaccount.Balance -= (amt + (0.05 * amt));
                                recevierbankaccount.Balance += amt;
                                transaction.SrcAccId = bankaccount.Id;
                                transaction.DestAccId = destuserId;
                                transaction.Amount = amt;
                                transaction.CreatedBy = bankaccount.Id;
                                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Id = "TXN" + bankaccount.Id + destuserId + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Type = TransactionType.Transfer;
                                bankaccount.Transactions.Add(transaction);
                                recevierbankaccount.Transactions.Add(transaction);
                                txnid = transaction.Id;
                            }
                            else
                            {
                                Console.WriteLine("You do not have sufficient money");
                            }
                            
                        }
                    }
                    else
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (bankaccount.Balance >= amt + (0.02 * amt))
                            {
                                bankaccount.Balance -= (amt + (0.02 * amt));
                                recevierbankaccount.Balance += amt;
                                transaction.SrcAccId = bankaccount.Id;
                                transaction.DestAccId = destuserId;
                                transaction.Amount = amt;
                                transaction.CreatedBy = bankaccount.Id;
                                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Id = "TXN" + bankaccount.Id + destuserId + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Type = TransactionType.Transfer;
                                bankaccount.Transactions.Add(transaction);
                                recevierbankaccount.Transactions.Add(transaction);
                                txnid = transaction.Id;
                            }
                            else
                            {
                                Console.WriteLine("You do not have sufficient money");
                            }
                        }
                        else
                        {
                            if (bankaccount.Balance >= amt + (0.06 * amt))
                            {
                                bankaccount.Balance -= (amt + (0.06 * amt));
                                recevierbankaccount.Balance += amt;
                                transaction.SrcAccId = bankaccount.Id;
                                transaction.DestAccId = destuserId;
                                transaction.Amount = amt;
                                transaction.CreatedBy = bankaccount.Id;
                                transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Id = "TXN" + bankaccount.Id + destuserId + bankaccount.BankId + DateTime.Now.ToString("yyyyMMddHHmmss");
                                transaction.Type = TransactionType.Transfer;
                                bankaccount.Transactions.Add(transaction);
                                recevierbankaccount.Transactions.Add(transaction);
                                txnid = transaction.Id;
                            }
                            else
                            {
                                Console.WriteLine("You do not have sufficient money");
                            }
                            
                        }
                    }
                }
            }
            return txnid;
        }

        public void ViewTransactions(string userid)
        {
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            foreach(var i in bankaccount.Transactions)
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

        public void AddCurrency(string currencyCode, double exchangeRate,Bank bank)
        {
            bank.CurrencyCodes.Add(new CurrencyCode() { Id = bank.CurrencyCodes.Count + 1, Code = "currencyCode", ExchangeRate = exchangeRate });
        }

        public void UpdateCharges()
        {

        }
    }
}
