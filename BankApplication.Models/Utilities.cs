using System;

namespace BankApplication.Utilities
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
            try
            {
                Console.Write(helpText);
                string input1 = Console.ReadLine();
                if(input1 == string.Empty)
                {
                    input = default(long);
                }
                else
                {
                    input = Convert.ToInt64(input1);
                }
                if (isRequired && 1000000000 < input && input > 9999999999)
                {
                    input = GetPhoneNumber(helpText, isRequired);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Phone number is not valid");
                input = GetPhoneNumber(helpText, isRequired);
            }
            return input;
        }
    }
}
