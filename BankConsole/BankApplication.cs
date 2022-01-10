using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using BankApplication.Models;
using BankApplication.Services;
using BankApplication.Concerns;

namespace BankApplication
{
    class BankApplication
    {
        IBankService BankService;
        IEmployeeService EmployeeService;
        IAccountService AccountService;
        IAccountHolderService AccountHolderService;

        public void Initialize(IHost host)
        {
            AccountHolderService = host.Services.GetRequiredService<AccountHolderService>();
            AccountService = host.Services.GetRequiredService<AccountService>();
            BankService = host.Services.GetRequiredService<BankService>();
            EmployeeService = host.Services.GetRequiredService<EmployeeService>();
            this.BankService = new BankService(new AccountHolderService());
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
            //AccountHolderService = new AccountHolderService();
            int bankid = Convert.ToInt32(Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse<AccountHolder> apiresponse = AccountHolderService.GetAccountHoldersByBank(bankid);
            if (apiresponse.IsSuccess)
            {
                List<AccountHolder> accountholderlist = apiresponse.ListData;
                foreach (var i in accountholderlist)
                {
                    Console.WriteLine(i.Id + " - " + i.Name + " - " + i.Address + " - " + i.PhoneNumber);
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
            EmployeeService = new EmployeeService();
            int bankid = Convert.ToInt32(Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse<Employee> apiresponse = EmployeeService.GetEmployeesByBank(bankid);
            if (apiresponse.IsSuccess)
            {
                List<Employee> employeelist = apiresponse.ListData;
                foreach (var i in employeelist)
                {
                    Console.WriteLine(i.Id + " - " + i.EmployeeId + " - " + i.Name + " - " + i.Address + " - " + i.PhoneNumber);
                }
            }
            else
            {
                this.ViewAllEmployees();
            }
        }

        public void ViewTransactions()
        {
            //AccountHolderService accountholderservice =new AccountHolderService();
            int accountid = Convert.ToInt32(Utilities.GetStringInput("Enter the account id  :  ", true));
            APIResponse<Transaction> apiresponse = AccountHolderService.GetTransactions(accountid);
            if (apiresponse.IsSuccess)
            {
                List<Transaction> transactionlist = apiresponse.ListData;
                foreach (var i in transactionlist)
                {
                    Console.WriteLine(i.SrcAccId + " to " + i.DestAccId + " of " + i.Amount + "Created by "+ i.CreatedBy);
                }
            }
            else
            {
                this.ViewTransactions();
            }
        }

        public int BankServiceViewAllBranches()
        {
            int bankid = Convert.ToInt32(Utilities.GetStringInput("Enter the bank id  :  ", true));
            APIResponse<Branch> apiresponse = BankService.GetBranchesBybank(bankid);
            if (apiresponse.IsSuccess)
            {
                List<Branch> branchlist = apiresponse.ListData;
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
            //IEmployeeService employeeservice = new EmployeeService();
            Bank bank = new Bank()
            {
                Name = Utilities.GetStringInput("Enter the Bank name  :  ", true)
            };
            APIResponse<Bank> bankRes = this.BankService.CreateBank(bank);
            if(bankRes.IsSuccess)
            {
                Branch branch = new Branch()
                {
                    Name = Utilities.GetStringInput("Enter the branch name  :  ", true),
                    Address = Utilities.GetStringInput("Enter the branch address  :  ", true),
                    IFSCCode = Utilities.GetStringInput("Enter the branch IFSC Code  :  ", true),
                    BankId = bankRes.Data.Id,
                    IsMainBranch = true
                };
                APIResponse<Branch> branchRes = this.BankService.CreateBranch(branch);
                if(branchRes.IsSuccess)
                {
                    Console.WriteLine("Please provide admin details to set up admin");
                    Employee admin = new Employee()
                    {
                        Type = UserType.Admin,
                        Name = Utilities.GetStringInput("Enter the Admin Name  :  ", true),
                        PhoneNumber = Utilities.GetPhoneNumber("Enter the Phone number  :  ", true),
                        Address = Utilities.GetStringInput("Enter the Your Address  :  ", true),
                        Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true),
                        Password = Utilities.GetStringInput("Enter the new password  :  ", true),
                        BankId = bankRes.Data.Id,
                        BranchId = branchRes.Data.Id
                    };

                    while (admin.Password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                    {
                        Console.WriteLine("Password does not matched! Please try Again");
                    }
                    var adminRes = EmployeeService.CreateEmployee(admin);
                    if (adminRes.IsSuccess)
                    {
                        Console.WriteLine("The Admin Id is " + admin.EmployeeId);
                        Console.WriteLine("Bank and admin are successfuly created");
                    }
                    else
                    {
                        Console.WriteLine(adminRes.Message);
                        while(!adminRes.IsSuccess)
                        {
                            adminRes = EmployeeService.CreateEmployee(admin);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine(bankRes.Message);
                this.SetUpBank();
            }
        }

        public void Login()
        {
            //AccountService accountservice = new AccountService();
            int username = Convert.ToInt32(Utilities.GetStringInput("Enter userid :  ", true));
            string password = Utilities.GetStringInput("Enter password :  ", true);
            try
            {
                User user = AccountService.Login(username, password);
                if(user != null)
                {
                    switch (user.Type)
                    {
                        case UserType.Employee:
                            this.EmployeeConsole((Employee)user);
                            break;

                        case UserType.Admin:
                            this.AdminConsole((Employee)user);
                            break;

                        case UserType.AccountHolder:
                            this.AccountHolderConsole((AccountHolder)user);
                            break;
                    }
                }
                else
                {
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
            //IEmployeeService employeeservice = new EmployeeService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                int bankid = this.BankServiceViewAllBranches();
                employee.BankId = bankid;
                employee.BranchId = Convert.ToInt32(Utilities.GetStringInput("Enter the Branch Id from above  :  ", true));
                employee.PhoneNumber = Convert.ToInt32(Utilities.GetStringInput("Enter the Phone number  :  ", true));
                employee.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
                APIResponse<Employee> apiresponse = EmployeeService.CreateEmployee(employee);
                if(!apiresponse.IsSuccess)
                {
                    Console.WriteLine(apiresponse.Message);
                    this.EmployeeRegistration();
                }
                else
                {
                    employee.EmployeeId = apiresponse.Message;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to Register...! Try again");
                this.EmployeeRegistration();
            }
            return employee.EmployeeId;
        }


        public int AccountHolderRegistration()
        {
            AccountHolder accountholder = new AccountHolder();
            try
            {
                //AccountHolderService accountholderservice = new AccountHolderService();
                
                accountholder.Name = Utilities.GetStringInput("Enter the Your Name   :   ", true);
                int bankid = this.BankServiceViewAllBranches();
                accountholder.BankId = bankid;
                accountholder.BranchId = Convert.ToInt32(Utilities.GetStringInput("Enter the Branch Id from above  :  ", true));
                accountholder.PhoneNumber = Convert.ToInt32(Utilities.GetPhoneNumber("Enter the Phone number  :  ", true));
                accountholder.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                accountholder.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                accountholder.Password = password;
                APIResponse<AccountHolder> status = AccountHolderService.CreateAccountHolder(accountholder);
                if(!status.IsSuccess)
                {
                    Console.WriteLine(status.Message);
                    this.AccountHolderRegistration();
                }
                else
                {
                    accountholder.Id = Convert.ToInt32(status.Message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to Register...! Try again");
                this.AccountHolderRegistration();
            }
            return accountholder.Id;
        }

        public void Deposit(User user)
        {
            //IBankService bankservice = new BankService();
            APIResponse<Transaction> apiresponse;
            if (user.Type == UserType.AccountHolder)
            {
                double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the amount  :  ", true));
                apiresponse = BankService.Deposit(user, user.Id, amt);
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
                int accountid = Convert.ToInt32(Utilities.GetStringInput("Enter the account id  :  ", true));
                double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the amount  :  ", true));
                apiresponse = BankService.Deposit(user,accountid, amt);
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
            //IBankService bankservice = new BankService();
            double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the amount  :  ", true));
            if(user.Type == UserType.AccountHolder)
            {
                APIResponse<Transaction> apiresponse = BankService.Withdraw(user, user.Id, amt);
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
                int accountid = Convert.ToInt32(Utilities.GetStringInput("Enter the account id  :  ", true));
                APIResponse<Transaction> apiresponse = BankService.Withdraw(user, accountid, amt);
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
            int srcAccId = user.Type == UserType.AccountHolder ? user.Id : Convert.ToInt32(Utilities.GetStringInput("Enter the Source Id  :  ", true));
            int destAccId = Convert.ToInt32(Utilities.GetStringInput("Enter the DestinationAccount Id  :  ", true));
            double amount = Convert.ToDouble(Utilities.GetStringInput("Enter the Amount  :  ", true));
            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
            
            APIResponse<Transaction> apiresponse = BankService.Transfer(user.Id, srcAccId, destAccId, amount, charge);
            Console.WriteLine(apiresponse.Message);
            if (!apiresponse.IsSuccess)
            {
                this.Transfer(user);
            }
        }

        public void ViewBalance()
        {
            //AccountHolderService accountholderservice = new AccountHolderService();
            int accountid = Convert.ToInt32(Utilities.GetStringInput("Enter the Account Id  :  ", true));
            APIResponse<AccountHolder> apiresponse = AccountHolderService.GetBalance(accountid);
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
            //IBankService bankservice = new BankService();
            Branch branch = new Branch()
            {
                BankId = Convert.ToInt32(Utilities.GetStringInput("Enter the Bank id  :  ", true)),
                Name = Utilities.GetStringInput("Enter the Branch Name  :  ", true),
                Address = Utilities.GetStringInput("Enter the Branch Address  :  ", true),
                IFSCCode = Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true),
                IsMainBranch = false
            };
            APIResponse<Branch> apiresponse = BankService.AddBranch(branch);
            if(apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
            }
            else
            {
                Console.WriteLine(apiresponse.Message);
                this.AddBranch();
            }
        }


        public void AccountHolderConsole(AccountHolder accountholder)
        {
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
            //IAccountHolderService accountholderservice = new AccountHolderService();
            int bankid = this.ViewAllAccountHolders();
            AccountHolder accountholder = new AccountHolder();
            int accountNo = Convert.ToInt32(Utilities.GetStringInput("Enter the account number  :  ", true));
            accountholder.Name = Utilities.GetStringInput("Enter the Name to Update  :  ", false);
            accountholder.PhoneNumber = Convert.ToInt32(Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
            accountholder.Address = Utilities.GetStringInput("Enter the Address to Update  :  ", false);
            APIResponse<AccountHolder> status = AccountHolderService.UpdateAccountHolder(accountholder);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.UpdateAccountHolder();
            }
        }

        public void DeleteAccountHolder()
        {
            //IAccountHolderService accountholderservice = new AccountHolderService();
            int userid1 = Convert.ToInt32(Utilities.GetStringInput("Enter the userid  :  ", true));
            APIResponse<AccountHolder> status = AccountHolderService.DeleteAccountHolderAccount(userid1);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.DeleteAccountHolder();
            }
        }

        public void AddCurrency()
        {
            CurrencyCode currencycode = new CurrencyCode()
            {
                BankId = Convert.ToInt32(Utilities.GetStringInput("Enter the Bank id  :  ", true)),
                Code = Utilities.GetStringInput("Enter the Currency Code  :  ", true),
                ExchangeRate = Convert.ToDouble(Utilities.GetStringInput("Enter the Exchange rate  :  ", true)),
            };
            APIResponse<CurrencyCode> status = BankService.AddCurrency(currencycode);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.AddCurrency();
            }
        }

        public void UpdateCharges()
        {
            //IBankService bankservice = new BankService();
            Bank bank = new Bank
            {
                Id = Convert.ToInt32(Utilities.GetStringInput("Enter the Bank id  :  ", true)),
                RTGSChargesforSameBank = Convert.ToInt32(Utilities.GetStringInput("Enter the charges for RTGSChargesforSameBank  :  ", false)),
                RTGSChargesforDifferentBank = Convert.ToInt32(Utilities.GetStringInput("Enter the charges for  RTGSChargesforDifferentBank  :  ", false)),
                IMPSChargesforSameBank = Convert.ToInt32(Utilities.GetStringInput("Enter the charges for IMPSChargesforSameBank   :  ", false)),
                IMPSChargesforDifferentBank = Convert.ToInt32(Utilities.GetStringInput("Enter the charges for IMPSChargesforDifferentBank  :  ", false)),  
            };
            APIResponse<int> apiresponse = BankService.UpdateCharges(bank);
            if(!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.UpdateCharges();
            }
        }

        public void RevertTransaction(User user)
        {
            int transid = Convert.ToInt32(Utilities.GetStringInput("Enter the TransactionID  :  ", true));
            APIResponse<Transaction> apiresponse = BankService.RevertTransaction(user, transid);
            if (!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.RevertTransaction(user);
            }
        }

        public void ResetPassword()
        {
            //IAccountService accountservice = new AccountService();
            int id = Convert.ToInt32(Utilities.GetStringInput("Enter the account id  :  ", true));
            string password = Utilities.GetStringInput("Enter the new password  :  ", true);
            while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
            {
                Console.WriteLine("Password does not matched! Please try Again");
            }
            APIResponse<string> apiresponse = AccountService.ResetPassword(id, password);
            if(apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
            }
            else
            {
                Console.WriteLine(apiresponse.Message);
                this.ResetPassword();
            }
        }


        public void EmployeeConsole(Employee employee)
        {
            bool employeemenuflag = true;
            while (employeemenuflag)
            {
                try
                {
                    Console.WriteLine("1. CreateAccountHolder\n2. UpdateAccountHolder\n3. DeleteAccountHolder\n4. Deposit\n5. Withdraw\n6. Transfer\n7. RevertTransaction\n8. ViewTransactions\n9. AddCurrency\n10. UpdateCharges\n11. Reset Password\n12. Exit");
                    Console.Write("Please select your option   :   ");
                    EmployeeMenu option2 = (EmployeeMenu)Enum.Parse(typeof(EmployeeMenu), Console.ReadLine(), true);
                    switch (option2)
                    {
                        case EmployeeMenu.CreateAccountHolder:
                            int accountholderid = this.AccountHolderRegistration();
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
                        case EmployeeMenu.ResetPassword:
                            this.ResetPassword();
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
            //IEmployeeService employeeservice = new EmployeeService();
            this.ViewAllEmployees();
            Employee employee = new Employee();
            employee.Id = Convert.ToInt32(Utilities.GetStringInput("Enter the employee id  :  ", true));
            employee.Name = Utilities.GetStringInput("Enter the Name to Update  :  ", false);
            employee.PhoneNumber = Convert.ToInt32(Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", false));
            employee.Address = Utilities.GetStringInput("Enter the Address to Update  :  ", false);
            APIResponse<Employee> status = EmployeeService.UpdateEmployee(employee);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.UpdateEmployee();
            }
        }

        public void DeleteEmployee()
        {
            //IEmployeeService employeeservice = new EmployeeService();
            int employeeid = Convert.ToInt32(Utilities.GetStringInput("Enter the employeeid  :  ", true));
            APIResponse<Employee> status = EmployeeService.DeleteEmployee(employeeid);
            if (!status.IsSuccess)
            {
                Console.WriteLine(status.Message);
                this.DeleteEmployee();
            }
        }

        public void DeleteBranch()
        {
            //IBankService bankservice = new BankService();
            int branchid = Convert.ToInt32(Utilities.GetStringInput("Enter the branch id  :  ", true));
            APIResponse<Branch> apiresponse = BankService.DeleteBranch(branchid);
            if(!apiresponse.IsSuccess)
            {
                Console.WriteLine(apiresponse.Message);
                this.DeleteBranch();
            }
        }


        public void AdminConsole(Employee admin)
        {
            try
            {
                bool AdminMenuflag = true;
                while (AdminMenuflag)
                {
                    Console.WriteLine("1. CreateEmployee\n2. UpdateEmployee\n3. DeleteEmployee\n4. CreateAccountHolder\n5. UpdateAccountHolder\n6. DeleteAccountHolder\n7. Deposit\n8. Withdraw\n9. Transfer\n10. RevertTransaction\n11. AddBranch\n12. DeleteBranch\n13. AddCurrency\n14. Update Charges\n15. Reset Password\n16. Exit");
                    AdminMenu option = (AdminMenu)Enum.Parse(typeof(AdminMenu), Utilities.GetStringInput("Please select your option   :   ", true), true);
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
                            int accountholderid = this.AccountHolderRegistration();
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
                        case AdminMenu.UpdateCharges:
                            this.UpdateCharges();
                            break;
                        case AdminMenu.ResetPassword:
                            this.ResetPassword();
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
