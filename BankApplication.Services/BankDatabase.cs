using BankApplication.Models;
using System.Collections.Generic;


namespace BankApplication.Services
{
    public class BankDatabase
    {
        public BankDatabase()
        {
            Banks = new List<Bank>();
        }

        public static List<Bank> Banks;
    }
}