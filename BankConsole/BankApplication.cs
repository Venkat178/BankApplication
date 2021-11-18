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
                            Bank bank = BankDatabase.Banks.Find(bank => bank.Admins.Any(employee => employee.Id == loginid));
                            Employee admin = bank != null ? bank.Admins.Find(employee => employee.Id == loginid) : null;
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
                            Bank bank1 = BankDatabase.Banks.Find(b => b.Employees.Any(employee => employee.Id == loginid));
                            Employee employee = bank1 != null ? bank1.Employees.Find(employee => employee.Id == loginid) : null;
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
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
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
            }
            this.MainMenu();
        }

        public void SetUpBank()
        {
            try
            {
                Bank bank = new Bank();
                Branch branch = new Branch();
                bank.Name = Utilities.Utilities.GetStringInput("Enter the Bank name  :  ", true);
                branch.Name = Utilities.Utilities.GetStringInput("Enter the Branch name  :  ", true);
                branch.Address = Utilities.Utilities.GetStringInput("Enter the Branch Address  :  ", true);
                branch.IFSCCode = Utilities.Utilities.GetStringInput("Enter the Branch IFSCCode  :  ", true);

                Console.WriteLine("Please provide admin details to set up admin");
                Employee admin = new Employee();
                admin.Type = UserType.Admin;

                admin.Name = Utilities.Utilities.GetStringInput("Enter the Admin Name  :  ", true);
                admin.PhoneNumber = Utilities.Utilities.GetStringInput("Enter the Phone number  :  ", true);
                admin.Address = Utilities.Utilities.GetStringInput("Enter the Your Address  :  ", true);
                admin.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string password = Utilities.Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                admin.Password = password;
                BankService bankService = new BankService();
                Status status = bankService.SetUpBank(branch,bank, admin);
                Console.WriteLine(bank.Id);
                if (status.IsSuccess)
                {
                    Console.WriteLine("The Admin Id is " + admin.Id);
                    Console.WriteLine("Bank and Admin are Successfuly Created");
                }
                else
                {
                    Console.WriteLine("Unable to save the details");
                }
            }
            catch (Exception ex)
            {
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
            }
            return employee.Id;
        }


        public string AccountHolderRegistration()
        {
            BankService bankservice = new BankService();
            BankAccount bankaccount = new BankAccount();
            try
            {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bankaccount.Id;
        }





        public void AccountHolderConsole(BankAccount bankaccount)
        {
            BankService bankservice = new BankService();
            bool menuflag = true;
            while (menuflag)
            {
                try
                {
                    Console.WriteLine("1. Deposit \n2. Withdraw \n3. Transfer \n4. View Balance \n5. View Transactions \n6. Exit .");
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
                            double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount  :  ", true));
                            string transid1= bankservice.Withdraw(bankaccount, amt1);
                            Console.WriteLine("Transaction Id is  :  " + transid1);
                            break;
                        case AccountHolderMenu.Transfer:
                            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true) , true);
                            string destid = Utilities.Utilities.GetStringInput("Enter the Reciever's BankId  :  ", true);
                            double amt2 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Amount  :  ", true));
                            string transid2 = bankservice.Transfer(bankaccount, destid, amt2,charge);
                            Console.WriteLine("Transaction Id is  :  " + transid2);
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
                }
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
                    if (employee != null)
                    {
                        Console.WriteLine("1. CreateAccountHolder\n2. UpdateAccountHolderDetails\n3. DeleteAccountHolder\n4. Deposit\n5. Withdraw\n6. Transfer\n7. RevertTransaction\n8. ViewTransactions\n9. AddCurrency\n10. UpdateCharges\n11. Exit");
                        Console.Write("Please select your option   :   ");
                        EmployeeMenu option2 = (EmployeeMenu)Enum.Parse(typeof(EmployeeMenu), Console.ReadLine(), true);
                        switch (option2)
                        {
                            case EmployeeMenu.CreateAccountHolder:
                                string accountholderid = this.AccountHolderRegistration();
                                Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                                break;
                            case EmployeeMenu.UpdateAccountHolderDetails:
                                bankservice.ViewAllAccounts();
                                string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                                BankAccount bankaccount = new BankAccount();
                                bankaccount.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                bankaccount.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                                bankaccount.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                accountholderservice.UpdateAccountHolderDetails(bankaccount, userid);
                                break;
                            case EmployeeMenu.DeleteAccountHolder:
                                string userid1 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                                accountholderservice.DeleteAccountHolderAccount(userid1);
                                break;
                            case EmployeeMenu.Deposit:
                                string userid2 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                                double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                                string transid=accountholderservice.Deposit(userid2,amt);
                                Console.WriteLine("the transaction id is : " + transid);
                                break;
                            case EmployeeMenu.Withdraw:
                                string userid3 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                                double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                                string transid1 =accountholderservice.Deposit(userid3, amt1);
                                Console.WriteLine("the transaction id is : " + transid1);
                                break;
                            case EmployeeMenu.Transfer:
                                Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
                                string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
                                string destid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
                                double amt2 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                                string transid2 = accountholderservice.Transfer(srcid,destid, amt2,charge);
                                Console.WriteLine("The transaction id is : " + transid2);
                                break;
                            case EmployeeMenu.RevertTransaction:
                                string transid3 = Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                                accountholderservice.revertTransaction(transid3);
                                break;
                            case EmployeeMenu.ViewTransactions:
                                string userid4 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                                accountholderservice.ViewTransactions(userid4);
                                break;
                            case EmployeeMenu.AddCurrency:
                                string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
                                Bank bank = BankDatabase.Banks.Find(bank => bank.Id == bankid);
                                string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                                double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                                accountholderservice.AddCurrency(currencycode,exchangerate,bank);
                                break;
                            case EmployeeMenu.UpdateCharges:
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
                    else
                    {
                        Console.WriteLine("Sorry,User do not found with this Id .");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
                    AdminMenu option2 = (AdminMenu)Enum.Parse(typeof(AdminMenu), Utilities.Utilities.GetStringInput("Please select your option   :   ", true), true);
                    switch (option2)
                    {
                        case AdminMenu.CreateEmployee:
                            string employeeid = this.EmployeeRegistration();
                            Console.WriteLine("The Employee Id is  :  " + employeeid);
                            break;
                        case AdminMenu.UpdateEmployeeDetails:
                            string bankid = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
                            bankservice.ViewAllEmployees(bankid);
                            Employee employee = new Employee();
                            string employeeid1 = Utilities.Utilities.GetStringInput("Enter the employeeid  :  ", true);
                            employee.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                            employee.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                            employee.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                            employeeservice.UpdateEmployeeDetails(employee, employeeid1);
                            break;
                        case AdminMenu.DeleteEmployee:
                            string employeeid2 = Utilities.Utilities.GetStringInput("Enter the employeeid  :  ", true);
                            employeeservice.DeleteEmployee(employeeid2);
                            break;
                        case AdminMenu.CreateAccountHolder:
                            string accountholderid = this.AccountHolderRegistration();
                            Console.WriteLine("The Account Holder Id is  :  " + accountholderid);
                            break;
                        case AdminMenu.UpdateAccountHolderDetails:
                            bankservice.ViewAllAccounts();
                            string userid = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                            BankAccount bankaccount = new BankAccount();
                            bankaccount.Name = Utilities.Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                            bankaccount.PhoneNumber = Utilities.Utilities.GetPhoneNumber("Enter the Phone Number to Update  :  ", true);
                            bankaccount.Address = Utilities.Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                            employeeservice.UpdateAccountHolderDetails(bankaccount, userid);
                            break;
                        case AdminMenu.DeleteAccountHolder:
                            string userid1 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                            employeeservice.DeleteAccountHolderAccount(userid1);
                            break;
                        case AdminMenu.Deposit:
                            string userid2 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                            double amt = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to deposit  :  ", true));
                            string transid = employeeservice.Deposit(userid2, amt);
                            Console.WriteLine("the transaction id is : " + transid);
                            break;
                        case AdminMenu.Withdraw:
                            string userid3 = Utilities.Utilities.GetStringInput("Enter the userid  :  ", true);
                            double amt1 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to withdraw  :  ", true));
                            string transid1 = employeeservice.Deposit(userid3, amt1);
                            Console.WriteLine("the transaction id is : " + transid1);
                            break;
                        case AdminMenu.Transfer:
                            Charges charge = (Charges)Enum.Parse(typeof(Charges), Utilities.Utilities.GetStringInput("What type of transfer you want IMPS/RTGS", true), true);
                            string srcid = Utilities.Utilities.GetStringInput("Enter the source user id  :  ", true);
                            string destid = Utilities.Utilities.GetStringInput("Enter the destination user id  :  ", true);
                            double amt2 = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the amount to transfer  :  ", true));
                            string transid2 = employeeservice.Transfer(srcid, destid, amt2,charge);
                            Console.WriteLine("the transaction id is : " + transid2);
                            break;
                        case AdminMenu.RevertTransaction:
                            string transid3 = Utilities.Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                            employeeservice.revertTransaction(transid3);
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
                            string branchid = Utilities.Utilities.GetStringInput("Enter the Branch id to Delete  :  ", true);
                            employeeservice.DeleteBranch(branchid);
                            Console.WriteLine("Branch Deleted Successfully");
                            break;
                        case AdminMenu.AddCurrency:
                            string bankid2 = Utilities.Utilities.GetStringInput("Enter the Bank id  :  ", true);
                            Bank bank1 = BankDatabase.Banks.Find(bank => bank.Id == bankid2);
                            string currencycode = Utilities.Utilities.GetStringInput("Enter the Currency Code  :  ", true);
                            double exchangerate = Convert.ToDouble(Utilities.Utilities.GetStringInput("Enter the Exchange rate  :  ", true));
                            employeeservice.AddCurrency(currencycode, exchangerate, bank1);
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
            }

        }
    }
}
