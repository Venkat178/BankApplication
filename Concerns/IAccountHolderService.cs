using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountHolderService
    {
        public Status UpdateAccountHolder(AccountHolder accountholder);
        public Status DeleteAccountHolderAccount(int userid);
        public Status revertTransaction(Transaction transaction);
        public Status AddCurrency(string currencyCode, double exchangeRate, Bank bank);
        public Status UpdateCharges(Bank bank);

    }
}
