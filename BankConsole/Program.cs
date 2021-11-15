using System;
using BankApplication.Models;
using BankApplication.Services;

namespace BankApplication
{
    class Program
    {
        static void Main()
        {
            BankApplication bankapp = new BankApplication();
            bankapp.Initialize();
        }
    }
}

