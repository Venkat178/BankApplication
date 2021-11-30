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

        public static int GetPhoneNumber(string helpText, bool isRequired)
        {
            int input;
            try
            {
                Console.Write(helpText);
                input = Console.ReadLine();
                if (input > 9999999999)
                {
                    Console.WriteLine("Phone number is not valid");
                }
                if (isRequired && string.IsNullOrEmpty(input) && input.Length != 10)
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
