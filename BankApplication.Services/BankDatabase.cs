using BankApplication.Models;
using System.Collections.Generic;


namespace BankApplication.Services
{
    public class BankDatabase
    {
        public static List<Bank> Banks = new List<Bank>();

        public static List<Admin> Admins = new List<Admin>();

        public static List<BankAccount> BankAccounts = new List<BankAccount>();

        public static List<Employee> Employees = new List<Employee>();

    }
}