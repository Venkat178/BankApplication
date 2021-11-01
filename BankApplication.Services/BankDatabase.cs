using System;
using BankApplication.Models;
using System.Collections.Generic;


namespace BankApplication.Services
{
    public class BankDatabase
    {
        public static List<Bank> Banks = new List<Bank>();

        public static Dictionary<string, List<Transaction>> TransList = new Dictionary<string, List<Transaction>>();

    }
}

