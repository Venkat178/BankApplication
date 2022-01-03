using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IAccountHolderService
    {
        public APIResponse CreateAccountHolder(AccountHolder accountholder, int branchid);
        public APIResponse UpdateAccountHolder(AccountHolder accountholder);
        public APIResponse DeleteAccountHolderAccount(int userid);
        public APIResponse RevertTransaction(User user, int transid);
        public AccountHolder GetAccountHolder(int accountid);
        public APIResponse ViewAllAccountHolders(int bankid);
        public bool IsExitAccountHolder(int accountid);
        public APIResponse ViewBalance(int accountid);
        public APIResponse ViewTransactions(int accountholderid);
    }
}
