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
            catch (Exception ex)
            {
                Console.WriteLine("Please provide valid input");
                GetStringInput(helpText, isRequired);
            }
            return input;
        }

        public static long GetPhoneNumber(string helpText, bool isRequired)
        {
            long input=0;
            try
            {
                Console.Write(helpText);
                input = Convert.ToInt64(Console.ReadLine());
                if (input > 9999999999)
                {
                    Console.WriteLine("Phone number is not valid");
                }
                if (isRequired && string.IsNullOrEmpty(input.ToString()))
                {
                    input = GetPhoneNumber(helpText, isRequired);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                GetStringInput(helpText, isRequired);
            }
            return input;
        }
    }
}
