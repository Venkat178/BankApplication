using BankApplication.Models;

namespace BankApplication.Services.Interfaces
{
    public interface IEmployeeService
    {
        public Status UpdateEmployee(Employee employee);
        public Status DeleteEmployee(string employeeid);
        public Status UpdateAccountHolder(AccountHolder accountholder);
        public Status DeleteAccountHolderAccount(string userid);
        public Status revertTransaction(string transId);
        public Status AddBranch(Branch branch, Bank bank);
        public Status DeleteBranch(Bank bank, Branch branch);
        public Status AddCurrency(string currencyCode, double exchangeRate, Bank bank);
        public bool UpdateCharges(Bank bank);
    }
}
