using BankApplication.Models;

namespace BankApplication.Services.Interfaces
{
    public interface IEmployeeService
    {
        public Status UpdateEmployee(Employee employee, string employeeid);
        public Status DeleteEmployee(string employeeid);
        public Status UpdateAccountHolder(AccountHolder AccountHolder, string accountholderid);
        public Status DeleteAccountHolderAccount(string userid);
        public Status revertTransaction(string transId);
        public Status AddBranch(Branch branch, Bank bank);
        public Status DeleteBranch(Bank bank, Branch branch);
        public Status AddCurrency(string currencyCode, double exchangeRate, Bank bank);
        public bool UpdateCharges(Bank bank);

    }
}
