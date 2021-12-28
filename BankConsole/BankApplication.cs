using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Services;
using BankApplication.Concerns;

namespace BankApplication
{
    class BankApplication
    {
        IBankService BankService;
        User LoggedInUser;

        public void Initialize()
        {
            this.BankService = (IBankService) new BankService();
            this.MainMenu();
        }

        public void MainMenu()
        {
            try
            {
                Console.WriteLine("1. SetUpBank \n2. Login \n3. Exit ");
                Console.Write("Please select your option  :  ");
                Mainmenu option = (Mainmenu)Enum.Parse(typeof(Mainmenu), Console.ReadLine(), true);
                switch (option)
                {
                    case Mainmenu.SetUpBank:
                        this.SetUpBank();
                        break;
                    case Mainmenu.Login:
                        this.Login();
                        break;
                    case Mainmenu.Exit:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Your Option is Not Valid");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.MainMenu();
            }
            this.MainMenu();
        }

        public void ViewAllBankBranches(int bankid)
        {
            BankApplicationDbContext bankdbapp = new BankApplicationDbContext();
            foreach (var i in bankdbapp.Branches)
            {
                if(i.BankId == bankid)
                {
                    Console.WriteLine(i.Id + "  -  " + i.Name);
                }
                
            }
        }

        public void ViewAllAccounts(int bankid)
        {
            BankApplicationDbContext bankdbapp = new BankApplicationDbContext();
            foreach (var i in bankdbapp.AccountHolders)
            {
                if(i.BankId == bankid)
                {
                    Console.WriteLine(i.Id + "   -   " + i.Name + "   -   " + i.PhoneNumber + "   -   " + i.Address);
                }
                
            }
        }

        public void ViewAllEmployees(int bankid)
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            foreach (var i in bankappdb.Employees)
            {
                if(i.BankId == bankid)
                {
                    Console.WriteLine(i.Id + "   -   " + i.Name + "   -   " + i.PhoneNumber + "   -   " + i.Address);
                }
                
            }
        }

        public void BankServiceViewTransactions()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault( _=> _.Id == userid);
            if (accountholder != null)
            {
                foreach (var i in bankappdb.Transactions)
                {
                    if(i.SrcAccId == userid || i.DestAccId == userid)
                    {
                        Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
                    }
                }
            }
            else
            {
                Console.WriteLine("User not found");
                this.BankServiceViewTransactions();
            }
        }

        public int BankServiceViewAllBankBranches()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            if(bank != null)
            {
                Console.WriteLine("Our Branches are   :   ");
                this.ViewAllBankBranches(bankid);
                return bankid;
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.BankServiceViewAllBankBranches();
            }
            return 0;
        }

        public void BankServiceViewAllAccounts()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService) new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            if (bank != null)
            {
                Console.WriteLine("Our Branches are   :   ");
                this.ViewAllAccounts(bankid);
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.BankServiceViewAllAccounts();
            }
        }



        public void SetUpBank()
        {
            Bank bank = new Bank()
            {
                CurrencyCodes = new System.Collections.Generic.List<CurrencyCode>() { new CurrencyCode() {  Code = "INR", ExchangeRate = 1, IsDefault = true } },
                IMPSChargesforSameBank = 5,
                RTGSChargesforSameBank = 0,
                IMPSChargesforDifferentBank = 6,
                RTGSChargesforDifferentBank = 2,
                Name = Utilities.Utilities.GetStringInput("Enter the Bank name  :  ", true)
            };
            Branch branch = new Branch()
            {
                Name = Utilities.Utilities.GetStringInput("Enter the branch name  :  ", true),
                Address = Utilities.Utilities.GetStringInput("Enter the branch address  :  ", true),
                IFSCCode = Utilities.Utilities.GetStringInput("Enter the branch IFSC Code  :  ", true)
            };

            Console.WriteLine("Please provide admin details to set up admin");
            Employee admin = new Employee()
            {
                Type = UserType.Admin,
                Name = Utilities.Utilities.GetStringInput("Enter the Admin Name  :  ", true),
                PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone number  :  ", true),
                Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true),
                Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true),
                Password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true)
            };

            while (admin.Password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }

            
            Status status = this.BankService.SetUpBank(branch, bank, admin);
            if (status.IsSuccess)
            {
                Console.WriteLine("The Admin Id is " + admin.EmployeeId);
                Console.WriteLine("Bank and admin are successfuly created");
            }
            else
            {
                Console.WriteLine("Unable to save the details");
                this.SetUpBank();
            }
        }

        public void Login()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            int username = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter userid :  ", true));
            string password = Utilities.Utilities.GetStringInput("Enter password :  ", true);
            try
            {
                if (bankappdb.Banks.Any(_ => _.Employees.Any(emp => emp.Id == username && emp.Password == password && emp.Type == UserType.Admin)))
                {
                    this.LoggedInUser = (Employee)bankappdb.Employees.FirstOrDefault(emp => emp.Id == username && emp.Password == password && emp.Type == UserType.Admin);
                    Console.WriteLine("Login Successfully");
                    Employee LoggedInUser = (Employee)this.LoggedInUser;
                    this.AdminConsole(LoggedInUser);
                    return;
                }

                if (bankappdb.Banks.Any(_ => _.Employees.Any(emp => emp.Id == username && emp.Password == password && emp.Type == UserType.Employee))){
                    this.LoggedInUser = (Employee)bankappdb.Employees.FirstOrDefault(emp => emp.Id == username && emp.Password == password && emp.Type == UserType.Employee);
                    Console.WriteLine("Login Successfully");
                    Employee LoggedInUser = (Employee)this.LoggedInUser;
                    this.EmployeeConsole(LoggedInUser);
                    return;
                }

                if (bankappdb.Banks.Any(_ => _.AccountHolders.Any(emp => emp.Id == username && emp.Password == password))){
                    this.LoggedInUser = (AccountHolder)bankappdb.AccountHolders.FirstOrDefault(emp => emp.Id == username && emp.Password == password );
                    Console.WriteLine("Login Successfully");
                    AccountHolder LoggedInUser = (AccountHolder)this.LoggedInUser;
                    this.AccountHolderConsole(LoggedInUser);
                    return;
                }
                else
                {
                    Console.WriteLine("User not found");
                    this.Login();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to login...! Try again");
                this.Login();
            }
        }

        public string EmployeeRegistration()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                int bankid = this.BankServiceViewAllBankBranches();
                Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
                if (bank == null)
                {
                    this.EmployeeRegistration();
                }
                employee.BankId = bankid;
                employee.BranchId = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true));
                Branch branch = bankappdb.Branches.FirstOrDefault(branch => branch.Id == employee.BranchId);
                if (bank == null)
                {
                    Console.WriteLine("No bank found");
                    this.EmployeeRegistration();
                }
                employee.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Phone number  :  ", true));
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
                string EmployeeId = bankservice.EmployeeRegister(employee);
                if(EmployeeId == null)
                {
                    Console.WriteLine("Name should not be short");
                    this.EmployeeRegistration();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to Register...! Try again");
                this.EmployeeRegistration();
            }
            return employee.EmployeeId;
        }


        public string AccountHolderRegistration()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            string accountholderid = null;
            try
            {
                BankService bankservice = new BankService();
                AccountHolder accountholder = new AccountHolder();
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Your Name   :   ", true);
                int bankid = this.BankServiceViewAllBankBranches();
                Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
                if (bank == null)
                {
                    this.AccountHolderRegistration();
                }
                accountholder.BankId = bankid;
                accountholder.BranchId = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true));
                accountholder.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone number  :  ", true));
                accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                accountholder.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                accountholder.Password = password;
                Status status = bankservice.Register(bank, accountholder);
                if(!status.IsSuccess)
                {
                    Console.WriteLine("Unable to register");
                    this.AccountHolderRegistration();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to Register...! Try again");
                this.AccountHolderRegistration();
            }
            return accountholderid;
        }

        public void AccountHolderConsoleWithdraw(AccountHolder AccountHolder)
        {
            IBankService bankservice = (IBankService) new BankService();
            double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
            if (amt1 <= AccountHolder.Balance)
            {
                string transid1 = bankservice.Withdraw(AccountHolder, amt1);
                Console.WriteLine("Transaction Id is  :  " + transid1);
            }
            else
            {
                Console.WriteLine("Insufficient amount");
                this.AccountHolderConsoleWithdraw(AccountHolder);
            }
        }

        public void AccountHolderConsoleTransfer(AccountHolder accountholder)
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService) new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            int destuserId = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Reciever's Id  :  ", true));
            AccountHolder recevieraccountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == destuserId);
            if (recevieraccountholder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount  :  ", true));
                if (recevieraccountholder != null)
                {
                    if (accountholder.BankId == recevieraccountholder.BankId)
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (accountholder.Balance <= amt)
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(accountholder);
                            }
                        }
                        else
                        {
                            if (accountholder.Balance <= amt + (0.05 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(accountholder);
                            }
                        }
                    }
                    else
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (accountholder.Balance <= amt + (0.02 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(accountholder);
                            }
                        }
                        else
                        {
                            if (accountholder.Balance <= amt + (0.06 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(accountholder);
                            }
                        }
                    }
                }
                string transid2 = bankservice.Transfer(accountholder, recevieraccountholder, amt, charge);
                Console.WriteLine("Transaction Id is  :  " + transid2);
            }
            else
            {
                Console.WriteLine("The destination id is not found");
                AccountHolderConsoleTransfer(accountholder);
            }
        }
        public void AccountHolderConsole(AccountHolder accountholder)
        {
            IBankService bankservice = (IBankService) new BankService();
            bool menuflag = true;
            while (menuflag)
            {
                try
                {
                    Console.WriteLine("1. Deposit \n2. Withdraw \n3. Transfer \n4. View Balance \n5. Exit .");
                    Console.Write("Please select your option   :   ");
                    AccountHolderMenu ch = (AccountHolderMenu)Enum.Parse(typeof(AccountHolderMenu), Console.ReadLine(), true);
                    switch (ch)
                    {
                        case AccountHolderMenu.Deposit:
                            double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount to Deposit  :  ", true));
                            string transid = bankservice.Deposit(accountholder, amt);
                            Console.WriteLine("Transaction Id is  :  " + transid);
                            break;
                        case AccountHolderMenu.Withdraw:
                            this.AccountHolderConsoleWithdraw(accountholder);
                            break;
                        case AccountHolderMenu.Transfer:
                            this.AccountHolderConsoleTransfer(accountholder);
                            break;
                        case AccountHolderMenu.ViewBalance:
                            double balance = bankservice.ViewBalance(accountholder);
                            Console.WriteLine("Your Balance is " + balance);
                            break;
                        case AccountHolderMenu.Exit:
                            accountholder = null;
                            menuflag = false;
                            break;
                        default:
                            Console.WriteLine("Your Option is Not Valid");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error occured...! Try again");
                    this.AccountHolderConsole(accountholder);
                }
            }


        }

        public void EmployeeConsoleUpdateAccountHolder()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IAccountHolderService accountholderservice = new AccountHolderService();
            IBankService bankservice = (IBankService) new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank Id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            AccountHolder accountholder = new AccountHolder();
            if (bank != null )
            {
                this.ViewAllAccounts(bankid);
                int accountNo = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account number  :  ", true));
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                accountholder.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
                accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = accountholderservice.UpdateAccountHolder(accountholder);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.EmployeeConsoleUpdateAccountHolder();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.EmployeeConsoleUpdateAccountHolder();
            }
        }

        public void EmployeeConsoleDeleteAccountHolder()
        {
            IAccountHolderService accountholderservice = new AccountHolderService();
            int userid1 = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            Status status = accountholderservice.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.EmployeeConsoleDeleteAccountHolder();
            }
        }

        public void EmployeeConsoleDeposit()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService) new BankService();
            IAccountHolderService accountholderservice = new AccountHolderService();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(_ => _.Id == userid);
            if (accountholder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                string transid = bankservice.EmployeeDeposit(accountholder, amt);
                Console.WriteLine("the transaction id is : " + transid);
            }
            else
            {
                Console.WriteLine("User not found");
                this.EmployeeConsoleDeposit();
            }

        }

        public void EmployeeConsoleWithdraw()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == userid);
            if (accountholder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                string transid1 = bankservice.EmployeeWithdraw(accountholder, amt);
                Console.WriteLine("the transaction id is : " + transid1);
            }
            else
            {
                Console.WriteLine("User not found");
                this.EmployeeConsoleWithdraw();
            }
        }

        public void EmployeeConsoleTransfer()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS  :  ", true), true);
            int srcid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == srcid);
            int destuserid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true));
            AccountHolder recevierAccountHolder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == destuserid);
            if (accountholder != null && recevierAccountHolder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                if (accountholder != null)
                {
                    if (recevierAccountHolder != null)
                    {
                        if (accountholder.BankId == recevierAccountHolder.BankId)
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (accountholder.Balance <= amt)
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (accountholder.Balance <= amt + (0.05 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                        }
                        else
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (accountholder.Balance <= amt + (0.02 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (accountholder.Balance <= amt + (0.06 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                        }
                    }
                }
                string transid2 = bankservice.EmployeeTransfer(accountholder, recevierAccountHolder, amt, charge);
                Console.WriteLine("The transaction id is : " + transid2);
            }
            else
            {
                Console.WriteLine("Users Not found");
                this.EmployeeConsoleTransfer();
            }
        }



        public void EmployeeConsoleAddCurrency()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IAccountHolderService accountholderservice = new AccountHolderService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            if (bank != null)
            {
                string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                Status status = accountholderservice.AddCurrency(currencycode, exchangerate, bank);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.EmployeeConsoleAddCurrency();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.EmployeeConsoleAddCurrency();
            }
        }

        public void EmployeeConsoleUpdateCharges()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IAccountHolderService accountholderservice = new AccountHolderService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            if (bank != null)
            {
                bank.RTGSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for RTGSChargesforSameBank  :  ", false));
                bank.RTGSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for  RTGSChargesforDifferentBank  :  ", false));
                bank.IMPSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforSameBank   :  ", false));
                bank.IMPSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforDifferentBank  :  ", false));
                Status status = accountholderservice.UpdateCharges(bank);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.EmployeeConsoleUpdateCharges();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.EmployeeConsoleUpdateCharges();
            }
        }

        public bool EmployeeConsoleRevertTransaction()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            try
            {
                IAccountHolderService accountholderservice = new AccountHolderService();
                int transid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true));
                Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.AccountHolders.Any(account => account.Transactions.Any(trans => trans.Id == transid)));
                AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Transactions.Any(trans => trans.Id == transid)) : null;
                Transaction transaction = accountholder != null ? accountholder.Transactions.Find(trans => trans.Id == transid) : null;
                if (transaction != null)
                {
                    accountholderservice.revertTransaction(transaction);
                    return true;
                }
                else
                {
                    Console.WriteLine("Transaction not found");
                    this.EmployeeConsoleRevertTransaction();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void EmployeeConsoleViewTransactions()
        {
            BankApplicationDbContext bankdbapp = new BankApplicationDbContext();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            AccountHolder accountholder = bankdbapp.AccountHolders.FirstOrDefault(_=>_.Id == userid);
            if (accountholder != null)
            {
                this.BankServiceViewTransactions();
            }
            else
            {
                Console.WriteLine("Account Holder Not found");
                this.EmployeeConsoleViewTransactions();
            }
        }


        public void EmployeeConsole(Employee employee)
        {
            IAccountHolderService accountholderservice = new AccountHolderService();
            IBankService bankservice = (IBankService)new BankService();
            bool employeemenuflag = true;
            while (employeemenuflag)
            {
                try
                {
                    Console.WriteLine("1. CreateAccountHolder\n2. UpdateAccountHolder\n3. DeleteAccountHolder\n4. Deposit\n5. Withdraw\n6. Transfer\n7. RevertTransaction\n8. ViewTransactions\n9. AddCurrency\n10. UpdateCharges\n11. Exit");
                    Console.Write("Please select your option   :   ");
                    EmployeeMenu option2 = (EmployeeMenu)Enum.Parse(typeof(EmployeeMenu), Console.ReadLine(), true);
                    switch (option2)
                    {
                        case EmployeeMenu.CreateAccountHolder:
                            string accountholderid = this.AccountHolderRegistration();
                            Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                            break;
                        case EmployeeMenu.UpdateAccountHolder:
                            this.EmployeeConsoleUpdateAccountHolder();
                            break;
                        case EmployeeMenu.DeleteAccountHolder:
                            this.EmployeeConsoleDeleteAccountHolder();
                            break;
                        case EmployeeMenu.Deposit:
                            this.EmployeeConsoleDeposit();
                            break;
                        case EmployeeMenu.Withdraw:
                            this.EmployeeConsoleWithdraw();
                            break;
                        case EmployeeMenu.Transfer:
                            this.EmployeeConsoleTransfer();
                            break;
                        case EmployeeMenu.RevertTransaction:
                            this.EmployeeConsoleRevertTransaction();
                            break;
                        case EmployeeMenu.ViewTransactions:
                            this.EmployeeConsoleViewTransactions();
                            break;
                        case EmployeeMenu.AddCurrency:
                            this.EmployeeConsoleAddCurrency();
                            break;
                        case EmployeeMenu.UpdateCharges:
                            this.EmployeeConsoleUpdateCharges();
                            break;
                        case EmployeeMenu.Exit:
                            employee = null;
                            employeemenuflag = false;
                            break;
                        default:
                            Console.WriteLine("Your option is not valid");
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error occured...! Try again");
                    this.EmployeeConsole(employee);
                }
            }
        }

        public void AdminConsoleUpdateEmployee()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IEmployeeService employeeservice = new EmployeeService();
            IBankService bankservice = (IBankService)new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank Id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(b => b.Id == bankid);
            if(bank != null)
            {
                this.ViewAllEmployees(bankid);
                Employee employee = new Employee();
                employee.Id = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the employee id  :  ", true));
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                employee.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = employeeservice.UpdateEmployee(employee);
                if (!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleUpdateEmployee();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.AdminConsoleUpdateEmployee();
            }
        }

        public void AdminConsoleDeleteEmployee()
        {
            IEmployeeService employeeservice = new EmployeeService();
            int employeeid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the employeeid  :  ", true));
            Status status = employeeservice.DeleteEmployee(employeeid);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AdminConsoleDeleteEmployee();
            }
        }

        public void AdminConsoleUpdateAccountHolder()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IEmployeeService employeeservice = new EmployeeService();
            BankService bankservice = new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            AccountHolder accountholder = new AccountHolder();
            if (bank != null)
            {
                this.ViewAllAccounts(bankid);
                accountholder.Id = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account number  :  ", true));
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                accountholder.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false);
                accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = employeeservice.UpdateAccountHolder(accountholder);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleUpdateAccountHolder();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.AdminConsoleUpdateAccountHolder();
            }
        }

        public void AdminConsoleDeleteAccountHolder()
        {
            IEmployeeService employeeservice = new EmployeeService();
            int userid1 = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            Status status = employeeservice.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AdminConsoleDeleteAccountHolder();
            }
        }

        public void AdminConsoleDeposit()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == userid);
            if (accountholder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                string transid = bankservice.EmployeeDeposit(accountholder, amt);
                Console.WriteLine("the transaction id is : " + transid);
            }
            else
            {
                Console.WriteLine("User not found");
                this.AdminConsoleDeposit();
            }

        }

        public void AdminConsoleWithdraw()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            int userid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == userid);
            if (accountholder != null)
            {
                double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                string transid1 = bankservice.EmployeeWithdraw(accountholder, amt1);
                Console.WriteLine("the Transaction id is : " + transid1);
            }
            else
            {
                Console.WriteLine("User not found");
                this.AdminConsoleWithdraw();
            }
        }

        public void AdminConsoleTransfer()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IBankService bankservice = (IBankService)new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            int srcid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true));
            AccountHolder accountholder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == srcid);
            int destuserid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true));
            AccountHolder recevierAccountHolder = bankappdb.AccountHolders.FirstOrDefault(account => account.Id == destuserid);
            if (accountholder != null && recevierAccountHolder != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                if (accountholder != null)
                {
                    if (recevierAccountHolder != null)
                    {
                        if (accountholder.BankId == recevierAccountHolder.BankId)
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (accountholder.Balance <= amt)
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (accountholder.Balance <= amt + (0.05 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                        }
                        else
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (accountholder.Balance <= amt + (0.02 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (accountholder.Balance <= amt + (0.06 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                        }
                    }
                }
                string transid2 = bankservice.EmployeeTransfer(accountholder, recevierAccountHolder, amt, charge);
                Console.WriteLine("The transaction id is : " + transid2);
            }
            else
            {
                Console.WriteLine("User not found");
                this.AdminConsoleTransfer();
            }
        }

        public void AdminConsoleAddCurrency()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IEmployeeService employeeservice = new EmployeeService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);
            if (bank != null)
            {
                string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                Status status = employeeservice.AddCurrency(currencycode, exchangerate,bank);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleAddCurrency();
                }
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.AdminConsoleAddCurrency();
            }
        }

        public void AdminConsoleDeleteBranch()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IEmployeeService employeeservice = new EmployeeService();
            int branchid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Branch id to Delete  :  ", true));
            Branch branch = bankappdb.Branches.FirstOrDefault(branch => branch.Id == branchid);
            if (branch != null)
            {
                employeeservice.DeleteBranch(branch);
                Console.WriteLine("Branch Deleted Successfully");
            }
            else
            {
                Console.WriteLine("Branch not found");
                this.AdminConsoleDeleteBranch();
            }

        }

        public void AdminConsoleRevertTransaction()
        {
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            IEmployeeService employeeservice = new EmployeeService();
            int transid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true));
            Transaction transaction = bankappdb.Transactions.FirstOrDefault(trans => trans.Id == transid);
            if (transaction != null)
            {
                Status status = employeeservice.revertTransaction(transid);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleRevertTransaction();
                }
            }
            else
            {
                Console.WriteLine("Transaction not found");
                this.AdminConsoleRevertTransaction();
            }
        }


        public void AdminConsole(Employee admin)
        {
            IEmployeeService employeeservice = new EmployeeService();
            IBankService bankservice = (IBankService)new BankService();
            BankApplicationDbContext bankappdb = new BankApplicationDbContext();
            try
            {
                bool AdminMenuflag = true;
                while (AdminMenuflag)
                {
                    Console.WriteLine("1. CreateEmployee\n2. UpdateEmployee\n3. DeleteEmployee\n4. CreateAccountHolder\n5. UpdateAccountHolder\n6. DeleteAccountHolder\n7. Deposit\n8. Withdraw\n9. Transfer\n10. RevertTransaction\n11. AddBranch\n12. DeleteBranch\n13. AddCurrency\n14. Exit");
                    AdminMenu option = (AdminMenu)Enum.Parse(typeof(AdminMenu), Utilities.Utilities.GetStringInput("Please select your option   :   ", true), true);
                    switch (option)
                    {
                        case AdminMenu.CreateEmployee:
                            string employeeid = this.EmployeeRegistration();
                            Console.WriteLine("The Employee Id is  :  " + employeeid);
                            break;
                        case AdminMenu.UpdateEmployee:
                            this.AdminConsoleUpdateEmployee();
                            break;
                        case AdminMenu.DeleteEmployee:
                            this.AdminConsoleDeleteEmployee();
                            break;
                        case AdminMenu.CreateAccountHolder:
                            string accountholderid = this.AccountHolderRegistration();
                            Console.WriteLine("The Account Holder Id is  :  " + accountholderid);
                            break;
                        case AdminMenu.UpdateAccountHolder:
                            this.AdminConsoleUpdateAccountHolder();
                            break;
                        case AdminMenu.DeleteAccountHolder:
                            this.AdminConsoleDeleteAccountHolder();
                            break;
                        case AdminMenu.Deposit:
                            this.AdminConsoleDeposit();
                            break;
                        case AdminMenu.Withdraw:
                            this.AdminConsoleWithdraw();
                            break;
                        case AdminMenu.Transfer:
                            this.AdminConsoleTransfer();
                            break;
                        case AdminMenu.RevertTransaction:
                            this.AdminConsoleRevertTransaction();
                            break;
                        case AdminMenu.AddBranch:
                            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
                            Bank bank = bankappdb.Banks.FirstOrDefault(bank => bank.Id == bankid);

                            Branch branch = new Branch()
                            {
                                Name = Utilities.Utilities.GetStringInput("Enter the Branch Name  :  ", true),
                                Address = Utilities.Utilities.GetStringInput("Enter the Branch Address  :  ", true),
                                IFSCCode = Utilities.Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true),
                                BankId = bank.Id,
                                IsMainBranch = false
                            };
                            employeeservice.AddBranch(branch);
                            Console.WriteLine("Branch added Successfully");
                            break;
                        case AdminMenu.DeleteBranch:
                            this.AdminConsoleDeleteBranch();
                            break;
                        case AdminMenu.AddCurrency:
                            this.AdminConsoleAddCurrency();
                            break;
                        case AdminMenu.Exit:
                            admin = null;
                            AdminMenuflag = false;
                            break;
                        default:
                            Console.WriteLine("Your option is not valid");
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Eroor occured...! Try again");
                this.AdminConsole(admin);
            }

        }
    }
}
