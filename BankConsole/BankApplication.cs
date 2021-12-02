using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Services;

namespace BankApplication
{
    class BankApplication
    {
        BankService BankService;
        User LoggedInUser;

        public void Initialize()
        {
            this.BankService = new BankService();
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

        public void BankServiceViewAllBankBranches()
        {
            BankService bankservice = new BankService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            if(bank != null)
            {
                Console.WriteLine("Our Branches are   :   ");
                bankservice.ViewAllBankBranches(bank);
            }
            else
            {
                this.BankServiceViewAllBankBranches();
            }
        }

        public void BankServiceViewAllAccounts()
        {
            BankService bankservice = new BankService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            if (bank != null)
            {
                Console.WriteLine("Our Branches are   :   ");
                bankservice.ViewAllAccounts(bank);
            }
            else
            {
                this.BankServiceViewAllAccounts();
            }
        }



        public void SetUpBank()
        {
            Bank bank = new Bank()
            {
                CurrencyCodes = new System.Collections.Generic.List<CurrencyCode>() { new CurrencyCode() { Id = 1, Code = "INR", ExchangeRate = 1, IsDefault = true } },
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
            AccountService accountservice = new AccountService();
            string username = Utilities.Utilities.GetStringInput("Enter userid :  ", true);
            string password = Utilities.Utilities.GetStringInput("Enter password :  ", true);
            try
            {
                if (BankDatabase.Banks.Any(_ => _.Employees.Any(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Admin)))
                {
                    Bank bank = BankDatabase.Banks.Find(_ => _.Employees.Any(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Admin));
                    this.LoggedInUser = bank != null ? (Employee)bank.Employees.Find(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Admin) : null;
                    Console.WriteLine("Login Successfully");
                    Employee LoggedInUser = (Employee)this.LoggedInUser;
                    this.AdminConsole(LoggedInUser);
                    return;
                }

                if (BankDatabase.Banks.Any(_ => _.Employees.Any(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Employee))){
                    Bank bank = BankDatabase.Banks.Find(_ => _.Employees.Any(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Employee));
                    this.LoggedInUser = bank != null ? (Employee)bank.Employees.Find(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Employee) : null;
                    //this.LoggedInUser = BankDatabase.Banks.FindAll(_ => _.Employees.Any(emp => emp.EmployeeId.ToLower() == username.ToLower() && emp.Password == password && emp.Type == UserType.Employee)).Select(_ => _.Employees).Cast<User>().ToList().FirstOrDefault();
                    Console.WriteLine("Login Successfully");
                    Employee LoggedInUser = (Employee)this.LoggedInUser;
                    this.EmployeeConsole(LoggedInUser);
                    return;
                }

                if (BankDatabase.Banks.Any(_ => _.AccountHolders.Any(emp => emp.Id.ToLower() == username.ToLower() && emp.Password == password))){
                    Bank bank = BankDatabase.Banks.Find(_ => _.AccountHolders.Any(emp => emp.Id.ToLower() == username.ToLower() && emp.Password == password ));
                    this.LoggedInUser = bank != null ? (AccountHolder)bank.AccountHolders.Find(emp => emp.Id.ToLower() == username.ToLower() && emp.Password == password ) : null;
                    //this.LoggedInUser = BankDatabase.Banks.FindAll(_ => _.AccountHolders.Any(emp => emp.Id.ToLower() == username.ToLower() && emp.Password == password)).Select(_ => _.AccountHolders).Cast<User>().ToList().FirstOrDefault();
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
            BankService bankservice = new BankService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                this.BankServiceViewAllBankBranches();
                employee.BranchId = Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.Branches.Any(branch => branch.Id == employee.BranchId));
                if (bank == null)
                {
                    Console.WriteLine("No bank found");
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
                employee.Id = bankservice.EmployeeRegister(employee, bank);
                if(employee.Id == null)
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
            return employee.Id;
        }


        public string AccountHolderRegistration()
        {
            string accountholderid = null;
            try
            {
                BankService bankservice = new BankService();
                AccountHolder accountholder = new AccountHolder();
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Your Name   :   ", true);

                this.BankServiceViewAllBankBranches();
                
                accountholder.BranchId = Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.Branches.Any(branch => branch.Id == accountholder.BranchId));
                if (bank == null)
                {
                    Console.WriteLine("No bank found");
                }
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
                accountholderid = accountholder.Id;
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
            BankService bankservice = new BankService();
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
            BankService bankservice = new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            string destuserId = Utilities.Utilities.GetStringInput("Enter the Reciever's Id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == destuserId));
            AccountHolder recevieraccountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == destuserId) : null;
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
            BankService bankservice = new BankService();
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
            AccountHolderService accountholderservice = new AccountHolderService();
            BankService bankservice = new BankService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank Id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            bankservice.ViewAllAccounts(bank);
            string accountNo = Utilities.Utilities.GetStringInput("Enter the account number  :  ", true);
            //AccountHolder AccountHolder = bankservice.GetAccountHolder(accountNo);
            AccountHolder accountholder = new AccountHolder();
            if (bank != null )
            {
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                accountholder.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
                accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = accountholderservice.UpdateAccountHolder(accountholder,accountNo);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.EmployeeConsoleUpdateAccountHolder();
                }
            }
            else
            {
                Console.WriteLine("User not found");
                this.EmployeeConsoleUpdateAccountHolder();
            }
        }

        public void EmployeeConsoleDeleteAccountHolder()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid1 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Status status = accountholderservice.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.EmployeeConsoleDeleteAccountHolder();
            }
        }

        public void EmployeeConsoleDeposit()
        {
            BankService bankservice = new BankService();
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
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
            BankService bankservice = new BankService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
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
            BankService bankservice = new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS  :  ", true), true);
            string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == srcid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == srcid) : null;
            string destuserid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
            Bank bank1 = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == destuserid));
            AccountHolder recevierAccountHolder = bank1 != null ? bank1.AccountHolders.Find(account => account.Id == destuserid) : null;
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
            AccountHolderService accountholderservice = new AccountHolderService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
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
            AccountHolderService accountholderservice = new AccountHolderService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
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
            try
            {
                AccountHolderService accountholderservice = new AccountHolderService();
                string transid = Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Transactions.Any(trans => trans.Id == transid)));
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
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            AccountHolder AccountHolder = new AccountHolder();
            if (AccountHolder != null)
            {
                accountholderservice.ViewTransactions(AccountHolder);
            }
            else
            {
                Console.WriteLine("Account Holder Not found");
                this.EmployeeConsoleViewTransactions();
            }
        }


        public void EmployeeConsole(Employee employee)
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            BankService bankservice = new BankService();
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
            EmployeeService employeeservice = new EmployeeService();
            BankService bankservice = new BankService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank Id  :  ", true);
            bankservice.ViewAllEmployees(bankid);
            string employeeNo = Utilities.Utilities.GetStringInput("Enter the account number  :  ", true);
            Employee employee = bankservice.GetEmployee(employeeNo, bankid);
            if (employee != null)
            {
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                employee.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = employeeservice.UpdateEmployee(employee,employeeNo);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleUpdateEmployee();
                }
            }
            else
            {
                Console.WriteLine("Employee not found");
                this.AdminConsoleUpdateEmployee();
            }
        }

        public void AdminConsoleDeleteEmployee()
        {
            EmployeeService employeeservice = new EmployeeService();
            string employeeid = Utilities.Utilities.GetStringInput("Enter the employeeid  :  ", true);
            Status status = employeeservice.DeleteEmployee(employeeid);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AdminConsoleDeleteEmployee();
            }
        }

        public void AdminConsoleUpdateAccountHolder()
        {
            EmployeeService employeeservice = new EmployeeService();
            BankService bankservice = new BankService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            bankservice.ViewAllAccounts(bank);
            string accountNo = Utilities.Utilities.GetStringInput("Enter the account number  :  ", true);
            //AccountHolder AccountHolder = bankservice.GetAccountHolder(accountNo);
            AccountHolder accountholder = new AccountHolder();
            if (bank != null)
            {
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
                accountholder.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false);
                accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
                Status status = employeeservice.UpdateAccountHolder(accountholder, accountNo);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AdminConsoleUpdateAccountHolder();
                }
            }
            else
            {
                Console.WriteLine("User not found");
                this.AdminConsoleUpdateAccountHolder();
            }
        }

        public void AdminConsoleDeleteAccountHolder()
        {
            EmployeeService employeeservice = new EmployeeService();
            string userid1 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Status status = employeeservice.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AdminConsoleDeleteAccountHolder();
            }
        }

        public void AdminConsoleDeposit()
        {
            BankService bankservice = new BankService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
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
            BankService bankservice = new BankService();
            EmployeeService employeeservice = new EmployeeService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == userid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == userid) : null;
            if (accountholder != null)
            {
                double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                string transid1 = bankservice.EmployeeWithdraw(accountholder, amt1);
                Console.WriteLine("the transaction id is : " + transid1);
            }
            else
            {
                Console.WriteLine("User not found");
                this.AdminConsoleWithdraw();
            }
        }

        public void AdminConsoleTransfer()
        {
            BankService bankservice = new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == srcid));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Id == srcid) : null;
            string destuserid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
            Bank bank1 = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Id == destuserid));
            AccountHolder recevierAccountHolder = bank != null ? bank.AccountHolders.Find(account => account.Id == destuserid) : null;
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
            EmployeeService employeeservice = new EmployeeService();
            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
            if (bank != null)
            {
                string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                Status status = employeeservice.AddCurrency(currencycode, exchangerate, bank);
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
            EmployeeService employeeservice = new EmployeeService();
            string branchid = Utilities.Utilities.GetStringInput("Enter the Branch id to Delete  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.Employees.Any(branch => branch.Id == branchid));
            Branch branch = bank != null ? bank.Branches.Find(branch => branch.Id == branchid) : null;
            if (branch != null)
            {
                employeeservice.DeleteBranch(bank, branch);
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
            EmployeeService employeeservice = new EmployeeService();
            string transid = Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true);
            Bank bank = BankDatabase.Banks.Find(bank => bank.AccountHolders.Any(account => account.Transactions.Any(trans => trans.Id == transid)));
            AccountHolder accountholder = bank != null ? bank.AccountHolders.Find(account => account.Transactions.Any(trans => trans.Id == transid)) : null;
            Transaction transaction = accountholder != null ? accountholder.Transactions.Find(trans => trans.Id == transid) : null;
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
            EmployeeService employeeservice = new EmployeeService();
            BankService bankservice = new BankService();
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
                            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
                            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
                            Branch branch = new Branch();
                            branch.Name = Utilities.Utilities.GetStringInput("Enter the Branch Name  :  ", true);
                            branch.Address = Utilities.Utilities.GetStringInput("Enter the Branch Address  :  ", true);
                            branch.IFSCCode = Utilities.Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true);
                            employeeservice.AddBranch(branch, bank);
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
