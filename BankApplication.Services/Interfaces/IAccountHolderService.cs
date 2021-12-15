using BankApplication.Models;

namespace BankApplication.Services.Interfaces
{
    public interface IAccountHolderService
    {
        public Status UpdateAccountHolder(AccountHolder accountholder, string accountholderid);
        public Status DeleteAccountHolderAccount(string userid);
        public void ViewTransactions(AccountHolder AccountHolder);
        public Status revertTransaction(Transaction transaction);
        public Status AddCurrency(string currencyCode, double exchangeRate, Bank bank);
        public Status UpdateCharges(Bank bank);
    }
}
