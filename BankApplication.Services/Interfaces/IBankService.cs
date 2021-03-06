using BankApplication.Models;

namespace BankApplication.Services.Interfaces
{
    public interface IBankService
    {
        public Status SetUpBank(Branch branch, Bank bank, Employee admin);
        public Status Register(Bank bank, AccountHolder AccountHolder);
        public string EmployeeRegister(Employee employee, Bank bank);
        public string Deposit(AccountHolder accountholder, double amt);
        public string Withdraw(AccountHolder AccountHolder, double amt);
        public string Transfer(AccountHolder AccountHolder, AccountHolder recevierAccountHolder, double amt, Charges charge);
        public AccountHolder GetAccountHolder(string accountid);
        public Employee GetEmployee(string employeeid, string bankid);
        public double ViewBalance(AccountHolder AccountHolder);
        public string EmployeeDeposit(AccountHolder AccountHolder, double amt);
        public string EmployeeWithdraw(AccountHolder AccountHolder, double amt);
        public string EmployeeTransfer(AccountHolder AccountHolder, AccountHolder recevierAccountHolder, double amt, Charges charge);
    }
}
