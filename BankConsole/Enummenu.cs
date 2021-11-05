using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplicationConsole
{
    public enum Mainmenu
    {
        SetUpBank=1, 
        Login,
        Exit
    }

    public enum Usermenu
    {
        Deposit=1, 
        Withdraw, 
        Transfer, 
        ViewBalance, 
        ViewTransactions, 
        Logout,
        Exit
    }

    public enum Employeemenu
    {
        AccountHolderRegistration = 1,
        UpdateAccountHolderName,
        UpdateAccountHolderPhoneNumber,
        UpdateAccountHolderGender,
        UpdateAccountHolderAddress,
        RevertTransaction,
        DeleteAccountHolderAccount,
        Exit
    }

    public enum Accountmenu
    {
        EmployeeRegistration=1,  
        UpdateEmployeeName,
        UpdateEmployeePhoneNumber,
        UpdateEmployeeGender,
        UpdateEmployeeAddress,
        DeleteEmployeeAccount,
        AccountHolderRegistration,
        UpdateAccountHolderName,
        UpdateAccountHolderPhoneNumber,
        UpdateAccountHolderGender,
        UpdateAccountHolderAddress,
        DeleteAccountHolderAccount,
        RevertTransaction, 
        ViewAllAccounts,
        Exit
    }
}
