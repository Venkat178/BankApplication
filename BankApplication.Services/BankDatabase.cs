﻿using BankApplication.Models;
using System.Collections.Generic;


namespace BankApplication.Services
{
    public class BankDatabase
    {
        public static List<Bank> Banks = new List<Bank>();

        public static List<BankAccount> BankAccounts = new List<BankAccount>();
    }
}