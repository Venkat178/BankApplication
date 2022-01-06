using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountHolderService
    {
        public APIResponse<AccountHolder> CreateAccountHolder(AccountHolder accountholder);
        public APIResponse<AccountHolder> UpdateAccountHolder(AccountHolder accountholder);
        public APIResponse<AccountHolder> DeleteAccountHolderAccount(int userid);
        public APIResponse<Transaction> RevertTransaction(User user, int transid);
        public AccountHolder GetAccountHolder(int accountid);
        public APIResponse<AccountHolder> ViewAllAccountHolders(int bankid);
        public bool IsExistAccountHolder(int accountid);
        public APIResponse<AccountHolder> ViewBalance(int accountid);
        public APIResponse<Transaction> ViewTransactions(int accountholderid);
    }
}
