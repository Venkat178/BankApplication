using System;
using System.Collections.Generic;


namespace BankApplication.Models
{
    public class BankDatabase
    {
        public static List<Bank> Banks = new List<Bank>();

        public static Dictionary<string, List<Transactions>> TransList = new Dictionary<string, List<Transactions>>();

    }
}

