using BankApplication.Models;
using System.Collections.Generic;

namespace BankApplication.Concerns
{
    public interface IBankService
    {
        public APIResponse SetUpBank(Branch branch, Bank bank, Employee admin);
        public APIResponse AddBranch(Branch branch, int bankid);
        public APIResponse DeleteBranch(int branchid);
        public APIResponse AddCurrency(string currencyCode, double exchangeRate, int bankid);
        public APIResponse UpdateCharges(Bank bank);
        public APIResponse Deposit(User user, int accountid, double amt);
        public APIResponse Withdraw(User user, int accountid, double amt);
        public APIResponse Transfer(User user, int srcid, int destid, double amt, Charges charge);
        public APIResponse ViewAllBranches(int bankid);
    }
}
