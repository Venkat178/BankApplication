using BankApplication.Models;
using System.Collections.Generic;

namespace BankApplication.Concerns
{
    public interface IBankService
    {
        public APIResponse<Bank> CreateBank(Branch branch,Employee admin);
        public APIResponse<Branch> AddBranch(Branch branch);
        public APIResponse<Branch> DeleteBranch(int branchid);
        public APIResponse<CurrencyCode> AddCurrency(CurrencyCode currencyCode);
        public APIResponse<int> UpdateCharges(Bank bank);
        public APIResponse<Transaction> Deposit(User user, int accountid, double amt);
        public APIResponse<Transaction> Withdraw(User user, int accountid, double amt);
        public APIResponse<Transaction> Transfer(User user, Transaction transaction , Charges charge);
        public APIResponse<Branch> ViewAllBranches(int bankid);
    }
}
