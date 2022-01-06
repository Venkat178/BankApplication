using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountHolderService
    {
        public APIResponse<AccountHolder> CreateAccountHolder(AccountHolder accountholder);
        public APIResponse<AccountHolder> UpdateAccountHolder(AccountHolder accountholder);
        public APIResponse<AccountHolder> DeleteAccountHolderAccount(int userid);
        public AccountHolder GetAccountHolder(int accountid);
        public APIResponse<AccountHolder> GetAccountHoldersByBank(int bankid);
        public bool IsExistAccountHolder(int accountid);
        public APIResponse<AccountHolder> GetBalance(int accountid);
        public APIResponse<Transaction> GetTransactions(int accountholderid);
    }
}
