using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplicationConsole
{
    public enum Mainmenu
    {
        SetUpBank,
        Register, 
        Login,
        Exit
    }

    public enum Usermenu
    {
        Deposit, 
        Withdraw, 
        Transfer, 
        ViewBalance, 
        ViewTransactions, 
        Logout,
        Exit
    }

    public enum Bankstaff
    {
        UpdateName,
        UpdateAddress,
        UpdatePhoneNumber,
        UpdateGender,
        RevertTransaction,
        DeleteAccount,
        Exit
    }
}
