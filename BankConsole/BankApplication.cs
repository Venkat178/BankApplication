using System;
using System.Linq;
using System.Collections.Generic;
using BankApplication.Models;
using BankApplication.Services;
using BankApplication.Concerns;

namespace BankApplication
{
    class BankApplication
    {
        IBankService BankService;

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

        public int ViewAllAccountHolders()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse apiresponse = accountholderservice.ViewAllAccountHolders(bankid);
            if (apiresponse.IsSuccess)
            {
                List<AccountHolder> accountholderlist = apiresponse.AccountHolderList;
                foreach (var i in accountholderlist)
                {
                    Console.WriteLine(i.Id + " - " + i.Name);
                }
                return bankid;
            }
            else
            {
                this.ViewAllAccountHolders();
                return bankid;
            }
        }

        public void ViewAllEmployees()
        {
            EmployeeService employeeservice = new EmployeeService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse apiresponse = employeeservice.ViewAllEmployees(bankid);
            if (apiresponse.IsSuccess)
            {
                List<Employee> employeelist = apiresponse.EmployeeList;
                foreach (var i in employeelist)
                {
                    Console.WriteLine(i.Id + " - " + i.EmployeeId + " - " + i.Name);
                }
            }
            else
            {
                this.ViewAllEmployees();
            }
        }

        public void ViewTransactions()
        {
            AccountHolderService accountholderservice =new AccountHolderService();
            int accountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account id  :  ", true));
            APIResponse apiresponse = accountholderservice.ViewTransactions(accountid);
            if (apiresponse.IsSuccess)
            {
                List<Transaction> transactionlist = apiresponse.TransactionList;
                foreach (var i in transactionlist)
                {
                    Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount);
                }
            }
            else
            {
                this.ViewTransactions();
            }
        }

        public int BankServiceViewAllBranches()
        {
            IBankService bankservice = new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse apiresponse = bankservice.ViewAllBranches(bankid);
            if (apiresponse.IsSuccess)
            {
                List<Branch> branchlist = apiresponse.BranchList;
                foreach (var i in branchlist)
                {
                    Console.WriteLine(i.Id + " - " + i.Name);
                }
                return bankid;
            }
            else
            {
                this.BankServiceViewAllBranches();
                return bankid;
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

            
            APIResponse status = this.BankService.SetUpBank(branch, bank, admin);
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
            int username = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter userid :  ", true));
            string password = Utilities.Utilities.GetStringInput("Enter password :  ", true);
            try
            {
                Employee employee = accountservice.AdminLogin(username, password);
                if(employee != null)
                {
                    this.AdminConsole(employee);
                    return;
                }

                Employee employee1 = accountservice.EmployeeLogin(username, password);
                if (employee1 != null)
                {
                    this.EmployeeConsole(employee1);
                    return;
                }

                AccountHolder accountholder = accountservice.AccountHolderLogin(username, password);
                if (accountholder != null)
                {
                    this.AccountHolderConsole(accountholder);
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
            IEmployeeService employeeservice = new EmployeeService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                int bankid = this.BankServiceViewAllBranches();
                employee.BankId = bankid;
                employee.BranchId = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Branch Id from above  :  ", true));
                employee.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Phone number  :  ", true));
                employee.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
                APIResponse apiresponse = employeeservice.CreateEmployee(employee,employee.BranchId);
                if(apiresponse.Message == null)
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
            string accountholderid = null;
            try
            {
                AccountHolderService accountholderservice = new AccountHolderService();
                AccountHolder accountholder = new AccountHolder();
                accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Your Name   :   ", true);
                int bankid = this.BankServiceViewAllBranches();
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
                //check branchid==================
                APIResponse status = accountholderservice.CreateAccountHolder(accountholder, accountholder.BranchId);
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

        public void Deposit(User user)
        {
            IBankService bankservice = new BankService();
            APIResponse apiresponse;
            if (user.Type == UserType.AccountHolder)
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
                apiresponse = bankservice.Deposit(user, user.Id, amt);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine(apiresponse.Message);
                    this.Deposit(user);
                }
            }
            else
            {
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
                int accountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account id  :  ", true));
                apiresponse = bankservice.Deposit(user,accountid, amt);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine(apiresponse.Message);
                    this.Deposit(user);
                }
            }
            
        }

        public void Withdraw(User user)
        {
            IBankService bankservice = new BankService();
            double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
            if(user.Type == UserType.AccountHolder)
            {
                APIResponse apiresponse = bankservice.Withdraw(user, user.Id, amt);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine(apiresponse.Message);
                    this.Withdraw(user);
                }
            }
            else
            {
                int accountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account id  :  ", true));
                APIResponse apiresponse = bankservice.Withdraw(user, accountid, amt);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine(apiresponse.Message);
                    this.Withdraw(user);
                }
            }
        }

        public void Transfer(User user)
        {
            IBankService bankservice = new BankService();
            if(user.Type == UserType.AccountHolder)
            {
                int recevieraccountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the DestinationAccount Id  :  ", true));
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount  :  ", true));
                Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);

                APIResponse apiresponse = bankservice.Transfer(user, user.Id, recevieraccountid, amt, charge);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine("The destination id is not found");
                    this.Transfer(user);
                }
            }
            else
            {
                int srcaccountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Source Id  :  ", true));
                int recevieraccountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the DestinationAccount Id  :  ", true));
                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount  :  ", true));
                Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);

                APIResponse apiresponse = bankservice.Transfer(user, srcaccountid, recevieraccountid, amt, charge);
                if (apiresponse.IsSuccess)
                {
                    Console.WriteLine("Transaction Id is  :  " + apiresponse.Message);
                }
                else
                {
                    Console.WriteLine("The destination id is not found");
                    this.Transfer(user);
                }
            }
        }

        public void ViewBalance()
        {
            AccountHolderService accountholderservice = new AccountHolderService();
            int accountid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Account Id  :  ", true));
            APIResponse apiresponse = accountholderservice.ViewBalance(accountid);
            if(apiresponse.IsSuccess)
            {
                Console.WriteLine("Your Balance is " + apiresponse.Message);
            }
            else
            {
                Console.WriteLine(apiresponse.Message);
                this.ViewBalance();
            }
        }

        public void AddBranch()
        {
            IBankService bankservice = new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            Branch branch = new Branch()
            {
                Name = Utilities.Utilities.GetStringInput("Enter the Branch Name  :  ", true),
                Address = Utilities.Utilities.GetStringInput("Enter the Branch Address  :  ", true),
                IFSCCode = Utilities.Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true),
                IsMainBranch = false
            };
            APIResponse apiresponse = bankservice.AddBranch(branch,bankid);
            Console.WriteLine("Branch added Successfully");
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
                            this.Deposit(accountholder);
                            break;
                        case AccountHolderMenu.Withdraw:
                            this.Withdraw(accountholder);
                            break;
                        case AccountHolderMenu.Transfer:
                            this.Transfer(accountholder);
                            break;
                        case AccountHolderMenu.ViewBalance:
                            this.ViewBalance();
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

        public void UpdateAccountHolder()
        {
            IAccountHolderService accountholderservice = new AccountHolderService();
            int bankid = this.ViewAllAccountHolders();
            AccountHolder accountholder = new AccountHolder();
            int accountNo = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the account number  :  ", true));
            accountholder.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
            accountholder.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
            accountholder.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
            APIResponse status = accountholderservice.UpdateAccountHolder(accountholder);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.UpdateAccountHolder();
            }
        }

        public void DeleteAccountHolder()
        {
            IAccountHolderService accountholderservice = new AccountHolderService();
            int userid1 = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the userid  :  ", true));
            APIResponse status = accountholderservice.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.DeleteAccountHolder();
            }
        }

        public void AddCurrency()
        {
            IBankService bankservice = new BankService();
            int bankid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true));
            string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
            double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
            APIResponse status = bankservice.AddCurrency(currencycode, exchangerate, bankid);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AddCurrency();
            }
        }

        public void UpdateCharges()
        {
            IBankService bankservice = new BankService();

            Bank bank = new Bank
            {
                Id = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true)),
                RTGSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for RTGSChargesforSameBank  :  ", false)),
                RTGSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for  RTGSChargesforDifferentBank  :  ", false)),
                IMPSChargesforSameBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforSameBank   :  ", false)),
                IMPSChargesforDifferentBank = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the charges for IMPSChargesforDifferentBank  :  ", false)),  
            };
            APIResponse apiresponse = bankservice.UpdateCharges(bank);
            if(!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.UpdateCharges();
            }
        }

        public void RevertTransaction(User user)
        {
            IAccountHolderService accountholderservice = new AccountHolderService();
            int transid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true));
            APIResponse apiresponse = accountholderservice.RevertTransaction(user, transid);
            if (!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.RevertTransaction(user);
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
                            this.UpdateAccountHolder();
                            break;
                        case EmployeeMenu.DeleteAccountHolder:
                            this.DeleteAccountHolder();
                            break;
                        case EmployeeMenu.Deposit:
                            this.Deposit(employee);
                            break;
                        case EmployeeMenu.Withdraw:
                            this.Withdraw(employee);
                            break;
                        case EmployeeMenu.Transfer:
                            this.Transfer(employee);
                            break;
                        case EmployeeMenu.RevertTransaction:
                            this.RevertTransaction(employee);
                            break;
                        case EmployeeMenu.ViewTransactions:
                            this.ViewTransactions();
                            break;
                        case EmployeeMenu.AddCurrency:
                            this.AddCurrency();
                            break;
                        case EmployeeMenu.UpdateCharges:
                            this.UpdateCharges();
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

        public void UpdateEmployee()
        {
            IEmployeeService employeeservice = new EmployeeService();
            this.ViewAllEmployees();
            Employee employee = new Employee();
            employee.Id = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the employee id  :  ", true));
            employee.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", false);
            employee.PhoneNumber = Convert.ToInt32(Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
            employee.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", false);
            APIResponse status = employeeservice.UpdateEmployee(employee);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.UpdateEmployee();
            }
        }

        public void DeleteEmployee()
        {
            IEmployeeService employeeservice = new EmployeeService();
            int employeeid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the employeeid  :  ", true));
            APIResponse status = employeeservice.DeleteEmployee(employeeid);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.DeleteEmployee();
            }
        }

        public void DeleteBranch()
        {
            IBankService bankservice = new BankService();
            int branchid = Convert.ToInt32(Utilities.Utilities.GetStringInput("Enter the branch id  :  ", true));
            APIResponse apiresponse = bankservice.DeleteBranch(branchid);
            if(!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.DeleteBranch();
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
                            this.UpdateEmployee();
                            break;
                        case AdminMenu.DeleteEmployee:
                            this.DeleteEmployee();
                            break;
                        case AdminMenu.CreateAccountHolder:
                            string accountholderid = this.AccountHolderRegistration();
                            Console.WriteLine("The Account Holder Id is  :  " + accountholderid);
                            break;
                        case AdminMenu.UpdateAccountHolder:
                            this.UpdateAccountHolder();
                            break;
                        case AdminMenu.DeleteAccountHolder:
                            this.DeleteAccountHolder();
                            break;
                        case AdminMenu.Deposit:
                            this.Deposit(admin);
                            break;
                        case AdminMenu.Withdraw:
                            this.Withdraw(admin);
                            break;
                        case AdminMenu.Transfer:
                            this.Transfer(admin);
                            break;
                        case AdminMenu.RevertTransaction:
                            this.RevertTransaction(admin);
                            break;
                        case AdminMenu.AddBranch:
                            this.AddBranch();
                            break;
                        case AdminMenu.DeleteBranch:
                            this.DeleteBranch();
                            break;
                        case AdminMenu.AddCurrency:
                            this.AddCurrency();
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
