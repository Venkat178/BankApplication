using System;

namespace BankApplication.Models
{
    public static class Utilities
    {
        public static string GetStringInput(string helpText, bool isRequired)
        {
            string input = string.Empty;
            try
            {
                Console.Write(helpText);
                input = Console.ReadLine();
                if (isRequired && string.IsNullOrEmpty(input))
                {
                    input = GetStringInput(helpText, isRequired);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Please provide valid input");
                input = GetStringInput(helpText, isRequired);
            }
            return input;
        }

        public static long GetPhoneNumber(string helpText, bool isRequired)
        {
            long input = default(long);
            Console.Write(helpText);
            try
            {
                if(isRequired)
                {
                    input = Convert.ToInt64(Console.ReadLine());
                    if (1000000000 < input && input > 9999999999)
                    {
                        input = GetPhoneNumber(helpText, isRequired);
                    }
                }
                else
                {
                    string input1 = Console.ReadLine();
                    if (string.IsNullOrEmpty(input1))
                    {
                        input = default(long);
                    }
                    else
                    {
                        input = Convert.ToInt64(input1);
                        if (1000000000 < input && input > 9999999999)
                        {
                            input = GetPhoneNumber(helpText, isRequired);
                        }
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Phone number is not valid");
                input = GetPhoneNumber(helpText, isRequired);
            }
            return input;
        }

        public static string GetTransactionId(int srcid, int desid , int bankid)
        {
            string Id = "TXN" + srcid + desid + bankid + DateTime.Now.ToString("yyyyMMddHHmmss");
            return Id;
        }

        public static string GetEmployeeId(string name)
        {
            return name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static Transaction GetTransaction(int userid , int srcid,int destid,double amt , int bankid,TransactionType type)
        {
            Transaction transaction = new Transaction();
            if (type == TransactionType.Credit)
            {
                transaction.SrcAccId = default(int);
                transaction.DestAccId = destid;
            }
            else if(type == TransactionType.Debit)
            {
                transaction.SrcAccId = srcid;
                transaction.DestAccId = default(int);
            }
            else
            {
                transaction.SrcAccId = srcid;
                transaction.DestAccId = destid;
                
            }
            transaction.Amount = amt;
            transaction.CreatedBy = userid;
            transaction.CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss");
            transaction.Type = type;
            transaction.TransactionId = GetTransactionId(srcid,destid,bankid);

            return transaction;
        }

    }
}
