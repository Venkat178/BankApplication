namespace BankApplication.Models
{
    public enum Mainmenu
    {
        SetUpBank=1, 
        Login,
        Exit
    }

    public enum AccountHolderMenu
    {
        Deposit=1,
        Withdraw,
        Transfer,
        ViewBalance,
        Exit
    }

    public enum EmployeeMenu
    {
        CreateAccountHolder = 1,
        UpdateAccountHolder,
        DeleteAccountHolder,
        Deposit,
        Withdraw,
        Transfer,
        RevertTransaction,
        ViewTransactions,
        AddCurrency,
        UpdateCharges,
        Exit
    }

    public enum AdminMenu
    {
        CreateEmployee=1,  
        UpdateEmployee,
        DeleteEmployee,
        CreateAccountHolder,
        UpdateAccountHolder,
        DeleteAccountHolder,
        Deposit,
        Withdraw,
        Transfer,
        RevertTransaction,
        AddBranch,
        DeleteBranch,
        AddCurrency,
        Exit
    }

    public enum GenderType
    {
        Male,
        Female,
        other
    }

    public enum TransactionType
    {
        Debit,
        Credit,
        Transfer,
        ViewBalance
    }

    public enum UserType
    {
        AccountHolder,
        Employee,
        Admin
    }

    public enum Charges
    {
        IMPS,
        RTGS
    }
}
