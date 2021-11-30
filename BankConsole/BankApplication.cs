using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Services;

namespace BankApplication
{
    class BankApplication
    {
        public void Initialize()
        {
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
                        AccountService accountservice = new AccountService();
                        string loginid = Utilities.Utilities.GetStringInput("Enter the user Id  :  ", true);
                        try
                        {
                            Bank bank = BankDatabase.Banks.Find(bank => bank.Employees.Any(employee => employee.Id == loginid && employee.Type == UserType.Admin));
                            Employee admin = bank != null ? bank.Employees.Find(employee => employee.Id == loginid && employee.Type == UserType.Admin) : null;
                            if (admin != null)
                            {
                                string adminpassword = Utilities.Utilities.GetStringInput("Enter the Password  :  ", true);
                                if (adminpassword == admin.Password)
                                {
                                    Employee loginadmin = accountservice.AdminLogin(loginid, adminpassword);
                                    Console.WriteLine("Login Successfully");
                                    this.AdminConsole(loginadmin);
                                }
                                else
                                {
                                    Console.WriteLine("Wrong password");
                                }
                                break;
                            }
                            
                            Bank bank1 = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == loginid && employee.Type == UserType.Employee));
                            Employee employee = bank1 != null ? bank1.Employees.Find(employee => employee.Id == loginid && employee.Type == UserType.Employee) : null;
                            if (employee != null)
                            {
                                string employeepassword = Utilities.Utilities.GetStringInput("Enter the Password  :  ", true);
                                if (employeepassword == employee.Password)
                                {
                                    Employee loginemployee = accountservice.EmployeeLogin(loginid, employeepassword);
                                    Console.WriteLine("Login Successfully");
                                    this.EmployeeConsole(loginemployee);
                                }
                                else
                                {
                                    Console.WriteLine("Wrong password");
                                }
                                break;
                            }
                            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == loginid);
                            if (bankaccount != null)
                            {
                                string bankaccountpassword = Utilities.Utilities.GetStringInput("Enter the Password  :  ", true);
                                if (bankaccountpassword == bankaccount.Password)
                                {
                                    BankAccount loginuser = accountservice.Login(loginid, bankaccountpassword);
                                    Console.WriteLine("Login Successfully");
                                    this.AccountHolderConsole(loginuser);
                                }
                                else
                                {
                                    Console.WriteLine("Wrong password");
                                }
                                break;
                            }
                            else
                            {
                                Console.WriteLine("User not found");
                                this.MainmenuLogin();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            this.MainmenuLogin();
                        }
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

        public void SetUpBank()
        {
            Bank bank = new Bank();
            bank.Name = Utilities.Utilities.GetStringInput("Enter the Bank name  :  ", true);
            Branch branch = new Branch()
            {
                Name = Utilities.Utilities.GetStringInput("Enter the Branch name  :  ", true),
                Address = Utilities.Utilities.GetStringInput("Enter the Branch Address  :  ", true),
                IFSCCode = Utilities.Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true)
            };

            Console.WriteLine("Please provide admin details to set up admin");
            Employee admin = new Employee()
            {
                Type = UserType.Admin,
                Name = Utilities.Utilities.GetStringInput("Enter the Admin Name  :  ", true),
                PhoneNumber = Utilities.Utilities.GetStringInput("Enter the Phone number  :  ", true),
                Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true),
                Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true)
            };

            admin.Password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
            while (admin.Password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }
            BankService bankService = new BankService();
            Status status = bankService.SetUpBank(branch, bank, admin);
            Console.WriteLine(bank.Id);
            if (status.IsSuccess)
            {
                Console.WriteLine("The Admin Id is " + admin.Id);
                Console.WriteLine("Bank and Admin are Successfuly Created");
            }
            else
            {
                Console.WriteLine("Unable to save the details");
                this.SetUpBank();
            }
        }

        public string EmployeeRegistration()
        {
            BankService bankservice = new BankService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                string Bankid = Utilities.Utilities.GetStringInput("Enter the bank Id  :  ", true);
                Console.WriteLine("Our Branches are   :   ");
                bankservice.ViewAllBankBranches(Bankid);
                employee.BranchId = Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.Branches.Any(branch => branch.Id == employee.BranchId));
                if (bank == null)
                {
                    Console.WriteLine("No bank found");
                }
                employee.PhoneNumber = Utilities.Utilities.GetStringInput("Enter the Phone number  :  ", true);
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
                employee.Id = bankservice.EmployeeRegister(employee, bank);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.EmployeeRegistration();
            }
            return employee.Id;
        }


        public string AccountHolderRegistration()
        {
            string bankaccountid = null;
            try
            {
                BankService bankservice = new BankService();
                BankAccount bankaccount = new BankAccount();
                bankaccount.Name = Utilities.Utilities.GetStringInput("Enter the Your Name   :   ", true);
                string Bankid = Utilities.Utilities.GetStringInput("Enter the bank Id  :  ", true);
                Console.WriteLine("Our Branches are   :   ");
                bankservice.ViewAllBankBranches(Bankid);
                bankaccount.BranchId = Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.Branches.Any(branch => branch.Id == bankaccount.BranchId));
                if (bank == null)
                {
                    Console.WriteLine("No bank found");
                }
                bankaccount.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone number  :  ", true);
                bankaccount.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                bankaccount.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                bankaccount.Password = password;
                string userid = bankservice.Register(bank, bankaccount);
                bankaccountid = bankaccount.Id;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.AccountHolderRegistration();
            }
            return bankaccountid;
        }

        public void AccountHolderConsoleWithdraw(BankAccount bankaccount)
        {
            BankService bankservice = new BankService();
            double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
            if(amt1<=bankaccount.Balance)
            {
                string transid1 = bankservice.Withdraw(bankaccount, amt1);
                Console.WriteLine("Transaction Id is  :  " + transid1);
            }
            else
            {
                Console.WriteLine("Insufficient amount");
                this.AccountHolderConsoleWithdraw(bankaccount);
            }
        }

        public void AccountHolderConsoleTransfer(BankAccount bankaccount)
        {
            BankService bankservice = new BankService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            string destuserId = Utilities.Utilities.GetStringInput("Enter the Reciever's Id  :  ", true);
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == destuserId);
            if (recevierbankaccount!=null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount  :  ", true));
                if (recevierbankaccount != null)
                {
                    if (bankaccount.BankId == recevierbankaccount.BankId)
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (bankaccount.Balance <= amt)
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(bankaccount);
                            }
                        }
                        else
                        {
                            if (bankaccount.Balance <= amt + (0.05 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(bankaccount);
                            }
                        }
                    }
                    else
                    {
                        if (charge == Charges.RTGS)
                        {
                            if (bankaccount.Balance <= amt + (0.02 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(bankaccount);
                            }
                        }
                        else
                        {
                            if (bankaccount.Balance <= amt + (0.06 * amt))
                            {
                                Console.WriteLine("You do not have sufficient money");
                                this.AccountHolderConsoleTransfer(bankaccount);
                            }
                        }
                    }
                }
                string transid2 = bankservice.Transfer(bankaccount, recevierbankaccount, amt, charge);
                Console.WriteLine("Transaction Id is  :  " + transid2);
            }
            else
            {
                Console.WriteLine("The destination id is not found");
                AccountHolderConsoleTransfer(bankaccount);
            }
        }
        public void AccountHolderConsole(BankAccount bankaccount)
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
                            string transid = bankservice.Deposit(bankaccount, amt);
                            Console.WriteLine("Transaction Id is  :  " + transid);
                            break;
                        case AccountHolderMenu.Withdraw:
                            this.AccountHolderConsoleWithdraw(bankaccount);
                            break;
                        case AccountHolderMenu.Transfer:
                            this.AccountHolderConsoleTransfer(bankaccount);
                            break;
                        case AccountHolderMenu.ViewBalance:
                            double balance = bankservice.ViewBalance(bankaccount);
                            Console.WriteLine("Your Balance is " + balance);
                            break;
                        case AccountHolderMenu.Exit:
                            bankaccount = null;
                            menuflag = false;
                            break;
                        default:
                            Console.WriteLine("Your Option is Not Valid");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    this.AccountHolderConsole(bankaccount);
                }
            }


        }

        public void EmployeeConsoleUpdateAccountHolder()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            BankService bankservice = new BankService();
            bankservice.ViewAllAccounts();
            string accountNo = Utilities.Utilities.GetStringInput("Enter the account number  :  ", true);
            BankAccount bankaccount = bankservice.GetAccountHolder(accountNo);
            if(bankaccount!=null)
            {
                bankaccount.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                bankaccount.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                bankaccount.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                accountholderservice.UpdateAccountHolder(bankaccount, accountNo);
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
            bool Isaccountholder = accountholderservice.DeleteAccountHolderAccount(userid1);
            if(!Isaccountholder)
            {
                Console.WriteLine("Account holder does not exit");
                this.EmployeeConsoleDeleteAccountHolder();
            }
        }

        public void EmployeeConsoleDeposit()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if(bankaccount!=null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                string transid = accountholderservice.Deposit(bankaccount, amt);
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
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if(bankaccount!=null)
            {
                double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                string transid1 = accountholderservice.Deposit(bankaccount, amt1);
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
            AccountHolderService accountholderservice = new AccountHolderService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == srcid);
            string destuserid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankacount => bankaccount.Id == destuserid);
            if(bankaccount != null && recevierbankaccount!=null)
            {

                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                if (bankaccount != null)
                {
                    if (recevierbankaccount != null)
                    {
                        if (bankaccount.BankId == recevierbankaccount.BankId)
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (bankaccount.Balance <= amt)
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (bankaccount.Balance <= amt + (0.05 * amt))
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
                                if (bankaccount.Balance <= amt + (0.02 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (bankaccount.Balance <= amt + (0.06 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.EmployeeConsoleTransfer();
                                }
                            }
                        }
                    }
                }
                string transid2 = accountholderservice.Transfer(bankaccount,recevierbankaccount, amt, charge);
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
            if(bank != null)
            {
                string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                accountholderservice.AddCurrency(currencycode, exchangerate, bank);
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
            if(bank != null)
            {
                bank.RTGSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for RTGSChargesforSameBank  :  ", true));
                bank.RTGSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for  RTGSChargesforDifferentBank  :  ", true));
                bank.IMPSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforSameBank   :  ", true));
                bank.IMPSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforDifferentBank  :  ", true));
                accountholderservice.UpdateCharges(bank,bankid);
            }
            else
            {
                Console.WriteLine("Bank not found");
                this.EmployeeConsoleUpdateCharges();
            }
        }

        public void EmployeeConsoleRevertTransaction()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            string transid = Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(b => b.Transactions.Any(transaction => transaction.Id == transid));
            Transaction transaction = bankaccount != null ? bankaccount.Transactions.Find(transaction => transaction.Id == transid) : null;
            if(transaction != null)
            {
                accountholderservice.revertTransaction(transaction);
            }
            else
            {
                Console.WriteLine("Transaction not found");
                this.EmployeeConsoleRevertTransaction();
            }
        }

        public void EmployeeConsoleViewTransactions()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            BankAccount bankaccount = new BankAccount();
            if(bankaccount != null)
            {
                accountholderservice.ViewTransactions(bankaccount);
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                employee.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                employeeservice.UpdateEmployee(employee, employeeNo);
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
            bool Isdeleted = employeeservice.DeleteEmployee(employeeid);
            if (!Isdeleted)
            {
                Console.WriteLine("Employee not found");
                this.AdminConsoleDeleteEmployee();
            }
        }

        public void AdminConsoleUpdateAccountHolder()
        {
            EmployeeService employeeservice = new EmployeeService();
            BankService bankservice = new BankService();
            bankservice.ViewAllAccounts();
            string accountNo = Utilities.Utilities.GetStringInput("Enter the account number  :  ", true);
            AccountHolder bankaccount = bankservice.GetAccountHolder(accountNo);
            if (bankaccount != null)
            {
                bankaccount.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                bankaccount.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                bankaccount.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                employeeservice.UpdateAccountHolder(bankaccount);
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
            bool Isaccountholder = employeeservice.DeleteAccountHolderAccount(userid1);
            if (!Isaccountholder)
            {
                Console.WriteLine("Account holder does not exit");
                this.AdminConsoleDeleteAccountHolder();
            }
        }

        public void AdminConsoleDeposit()
        {
            EmployeeService employeeservice = new EmployeeService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                string transid = employeeservice.Deposit(bankaccount, amt);
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
            EmployeeService employeeservice = new EmployeeService();
            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == userid);
            if (bankaccount != null)
            {
                double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                string transid1 = employeeservice.Deposit(bankaccount, amt1);
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
            EmployeeService employeeservice = new EmployeeService();
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == srcid);
            string destuserid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
            BankAccount recevierbankaccount = BankDatabase.BankAccounts.Find(bankacount => bankaccount.Id == destuserid);
            if (bankaccount != null && recevierbankaccount != null)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                if (bankaccount != null)
                {
                    if (recevierbankaccount != null)
                    {
                        if (bankaccount.BankId == recevierbankaccount.BankId)
                        {
                            if (charge == Charges.RTGS)
                            {
                                if (bankaccount.Balance <= amt)
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (bankaccount.Balance <= amt + (0.05 * amt))
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
                                if (bankaccount.Balance <= amt + (0.02 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                            else
                            {
                                if (bankaccount.Balance <= amt + (0.06 * amt))
                                {
                                    Console.WriteLine("You do not have sufficient money");
                                    this.AdminConsoleTransfer();
                                }
                            }
                        }
                    }
                }
                string transid2 = employeeservice.Transfer(bankaccount, recevierbankaccount, amt, charge);
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
                employeeservice.AddCurrency(currencycode, exchangerate, bank);
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
            if(branch != null)
            {
                employeeservice.DeleteBranch(bank,branch);
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
            BankAccount bankaccount = BankDatabase.BankAccounts.Find(b => b.Transactions.Any(transaction => transaction.Id == transid));
            Transaction transaction = bankaccount != null ? bankaccount.Transactions.Find(transaction => transaction.Id == transid) : null;
            if (transaction != null)
            {
                employeeservice.revertTransaction(transid);
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
                            string bankid1 = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
                            Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid1);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.AdminConsole(admin);
            }

        }
    }
}
