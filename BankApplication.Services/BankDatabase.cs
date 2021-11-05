using BankApplication.Models;
using System.Collections.Generic;


namespace BankApplication.Services
{
    public class BankDatabase
    {
        public static List<Bank> Banks = new List<Bank>();

        public static List<BankAccount> BankAccounts = new List<BankAccount>();

        public static List<Employee> Employees = new List<Employee>();

        public static List<Admin> Admins = new List<Admin>();

        public static Dictionary<string, List<Transaction>> TransList = new Dictionary<string, List<Transaction>>();

    }
}