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
                GetStringInput(helpText, isRequired);
            }
            return input;
        }

        public static long GetPhoneNumber(string helpText, bool isRequired)
        {
            long input = default(int);
            try
            {
                Console.Write(helpText);
                input = Convert.ToInt64(Console.ReadLine());
                if (isRequired && string.IsNullOrEmpty(input.ToString()) && input > 9999999999)
                {
                    input = GetPhoneNumber(helpText, isRequired);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Phone number is not valid");
                GetStringInput(helpText, isRequired);
            }
            return input;
        }
    }
}
