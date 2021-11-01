using System;
using System.Linq;

namespace BankApplicationConsole
{
    public static class  Utilities
    {
        public static  string GetStringInput(string helpText,bool isRequired)
        {
            string input = null;
            Console.Write(helpText);
            input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) && isRequired)
            {
                input = GetStringInput(helpText,isRequired);
            }
            return input;
        }
    }
}
