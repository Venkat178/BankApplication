using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IEmployeeService
    {
        public Status UpdateEmployee(Employee employee);
        public Status DeleteEmployee(int employeeid);
        public Status UpdateAccountHolder(AccountHolder accountholder);
        public Status DeleteAccountHolderAccount(int userid);
        public Status revertTransaction(int transId);
        public Status AddBranch(Branch branch);
        public Status DeleteBranch(Branch branch);
        public Status AddCurrency(string currencyCode, double exchangeRate,Bank bank);
        public bool UpdateCharges(Bank bank);

    }
}
