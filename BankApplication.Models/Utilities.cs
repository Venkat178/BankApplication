using System;
using System.Linq;

namespace BankApplication.Models
{
    public static class  Utilities
    {
        public static  string GetStringInput(string helpText,bool isRequired)
        {
            string input = string.Empty;
            try
            {  
                Console.Write(helpText);
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input) && isRequired)
                {
                    input = GetStringInput(helpText, isRequired);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                GetStringInput(helpText,isRequired);
            }
            
            return input;
        }
    }
}
