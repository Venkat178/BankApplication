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
                Console.WriteLine(ex.Message);
                GetStringInput(helpText, isRequired);
            }

            return input;
        }

        public static string GetPhoneNumber(string helpText, bool isRequired)
        {
            string input = string.Empty;
            try
            {
                Console.Write(helpText);
                input = Console.ReadLine();
                if (input.Length != 10)
                {
                    //throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                if (isRequired && string.IsNullOrEmpty(input))
                {
                    input = GetStringInput(helpText, isRequired);
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
