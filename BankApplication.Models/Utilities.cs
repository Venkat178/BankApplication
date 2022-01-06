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
                    if (1000000000 > input || input > 9999999999)
                    {
                        Console.WriteLine("Please provide valid phone number");
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

        public static string GetEmployeeId(string name) // chnage name of the method
        {
            return name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static Transaction GetTransaction(int userid, int srcid, int destid, double amt, int bankid, TransactionType type)
        {
            return new Transaction()
            {
                SrcAccId = srcid,
                DestAccId = destid,
                Amount = amt,
                CreatedBy = userid,
                CreatedOn = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Type = type,
                TransactionId = GetTransactionId(srcid, destid, bankid)
            };
        }

    }
}
