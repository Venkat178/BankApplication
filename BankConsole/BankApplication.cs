using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Services;
using System.Collections.Generic;

namespace BankApplication
{
    class BankApplication
    {
        public void Initialize()
        {   
            this.MainMenu();
        }

        public Bank SetUpBank()
        {
            Bank bank = new Bank();
            Status status = new Status();
            BankService bankservice = new BankService();
            bank.BankName = Utilities.GetStringInput("Enter the Bank name  :  ", true);
            bank.BranchName = Utilities.GetStringInput("Enter the Branch Name  :  ", true);
            bank.IFSCCode = Utilities.GetStringInput("Enter the IFSC Code  :  ", true);
            bank.Id = bank.BankName + DateTime.Now.ToString("yyyyMMddHHmmss");
            Console.WriteLine("Please provide admin details to set up admin");
            bank.Admin = new Admin();
            try
            {
                bank.Admin.Name = Utilities.GetStringInput("Enter the Admin Name  :  ", true);
                bank.Admin.BranchName = bank.BranchName;
                bank.Admin.BankId = bank.Id;
                bank.Admin.Type = UserType.Admin;
                bank.Admin.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                if(bank.Admin.PhoneNumber.Length!=10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                bank.Admin.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                bank.Admin.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                try
                {
                    bank.Admin.Id = bank.Admin.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("The Admin Id is " + bank.Admin.Id);
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                bank.Admin.Password = password;
                BankDatabase.Admins.Add(bank.Admin);
                status = bankservice.SetUpBank(bank);
                
                //Console.WriteLine(bank.CurrencyCode);
                if (status.IsSuccess)
                {
                    Console.WriteLine("Bank is Successfuly Created");
                }
                else
                {
                    Console.WriteLine("Unable to save the details");
                }      
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bank;
        }

        public string EmployeeRegistration()
        {
            BankService bankservice = new BankService();
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.GetStringInput("Enter the Employee Name  :  ", true);
                bankservice.ViewAllBankBranches();
                employee.BranchName = Utilities.GetStringInput("Enter the Branch Name from the below  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.BranchName == employee.BranchName);
                if (bank != null)
                {
                    employee.BankId = bank.Id;
                }
                else
                {
                    throw new Exception("No bank found");
                }
                employee.Type = UserType.Employee;
                employee.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                employee.Id = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (employee.PhoneNumber.Length != 10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                employee.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                employee.Admin = bank.Admin;
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
                bank.Employees.Add(employee);
                BankDatabase.Employees.Add(employee);
            }
            catch(Exception ex )
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
                bankaccount.Name = Utilities.GetStringInput("Enter the Your Name   :   ", true);
                //bankaccount.Admin = AdminObj;
                Console.WriteLine("Our Branches are   :   ");
                bankservice.ViewAllBankBranches();
                bankaccount.BranchName = Utilities.GetStringInput("Enter the Branch Name from the above  :  ", true);
                Bank bank = BankDatabase.Banks.Find(bank => bank.BranchName == bankaccount.BranchName);
                if (bank != null)
                {
                    bankaccount.BankId = bank.Id;
                }
                else
                {
                    throw new Exception("No bank found");
                }
                bankaccount.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                if (bankaccount.PhoneNumber.Length != 10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                bankaccount.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                bankaccount.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                
                string userid = bankservice.Register(bank,bankaccount);
                bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                bankaccount.Password = password;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return bankaccount.Id;
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
                        try
                        {
                            this.SetUpBank();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case Mainmenu.Login:
                        BankService bankservice = new BankService();
                        string loginuserid = Utilities.GetStringInput("Enter the user Id  :  ", true);
                        try
                        {
                            Admin admin = BankDatabase.Admins.Find(admin => admin.Id == loginuserid);
                            if(admin!=null)
                            {
                                string adminpassword = Utilities.GetStringInput("Enter the Password  :  ", true);
                                if(adminpassword==admin.Password)
                                {
                                    Admin loginadmin = bankservice.AdminLogin(loginuserid, adminpassword);
                                    Console.WriteLine("Login Successfully");
                                    this.AccountMenu(loginadmin);
                                }
                                else
                                {
                                    throw new WrongPasswordException("Wrong password");
                                }
                                break;
                            }
                            Employee employee = BankDatabase.Employees.Find(employee => employee.Id == loginuserid);
                            if (employee != null)
                            {
                                string employeepassword = Utilities.GetStringInput("Enter the Password  :  ", true);
                                if (employeepassword == employee.Password)
                                {
                                    Employee loginemployee = bankservice.EmployeeLogin(loginuserid, employeepassword);
                                    Console.WriteLine("Login Successfully");
                                    this.BankStaff(loginemployee);
                                }
                                else
                                {
                                    throw new WrongPasswordException("Wrong password");
                                }
                                break;
                            }
                            BankAccount bankaccount = BankDatabase.BankAccounts.Find(bankaccount => bankaccount.Id == loginuserid);
                            if (bankaccount != null)
                            {
                                string bankaccountpassword = Utilities.GetStringInput("Enter the Password  :  ", true);
                                if (bankaccountpassword == bankaccount.Password)
                                {
                                    BankAccount loginuser = bankservice.Login(loginuserid, bankaccountpassword);
                                    Console.WriteLine("Login Successfully");
                                    this.UserMenu(loginuser);
                                }
                                else
                                {
                                    throw new WrongPasswordException("Wrong password");
                                }
                                break;
                            }
                            else
                            {
                                throw new UserNotFoundException("User not found");
                            }
                        }
                        catch(Exception ex)
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
                MainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            this.MainMenu();
        }

        

        public void UserMenu(BankAccount bankaccount)
        {
            BankService bankservice = new BankService();
            bool menuflag = true;
            while(menuflag)
            {
                try
                {
                    if (bankaccount != null)
                    {
                        Console.WriteLine("1. Deposit \n2. Withdraw \n3. Transfer \n4. View Balance \n5. View Transactions \n6. Logout .");
                        Console.Write("Please select your option   :   ");
                        Usermenu ch = (Usermenu)Enum.Parse(typeof(Usermenu), Console.ReadLine(), true);
                        switch (ch)
                        {
                            case Usermenu.Deposit:
                                double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the Amount to Deposit  :  ", true));
                                bankservice.Deposit(bankaccount,amt);
                                break;
                            case Usermenu.Withdraw:
                                try
                                {
                                    Console.Write("Enter the Amount  :  ");
                                    double amt1 = Convert.ToDouble(Console.ReadLine());
                                    bankservice.Withdraw(bankaccount,amt1);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case Usermenu.Transfer:
                                Console.Write("Enter the Reciever's BankId  :  ");
                                string recuid = Console.ReadLine();
                                Console.Write("Enter the Amount  :  ");
                                double amt2 = Convert.ToDouble(Console.ReadLine());
                                bankservice.Transfer(bankaccount, recuid, amt2);
                                break;
                            case Usermenu.ViewBalance:
                                double balance = bankservice.ViewBalance(bankaccount);
                                Console.WriteLine("Your Balance is " + balance);
                                break;
                            case Usermenu.ViewTransactions:
                                bankservice.ViewTransactions(bankaccount);
                                break;
                            case Usermenu.Logout:
                                bankaccount= null;
                                menuflag = false;
                                break;
                            default:
                                Console.WriteLine("Your Option is Not Valid");
                                break;
                        }
                    }
                    else
                    {
                        throw new UserNotFoundException("Sorry,User do not found with this Id .");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            
            
        }

        public void BankStaff(Employee employee)
        {
            EmployeeService employeeservice = new EmployeeService();
            bool employeemenuflag = true;
            while(employeemenuflag)
            {
                try
                {
                    if (employee != null)
                    {
                        Console.WriteLine("1. AccountHolderRegistration \n2. UpdateAccountHolderName \n3. UpdateAccountHolderPhoneNumber \n4. UpdateAccountHolderGender \n5. UpdateAccountHolderAddress \n6. RevertTransaction\n7. DeleteAccountHolderAccount\n8.Exit");
                        Console.Write("Please select your option   :   ");
                        Employeemenu option2 = (Employeemenu)Enum.Parse(typeof(Employeemenu), Console.ReadLine(), true);
                        switch (option2)
                        {
                            case Employeemenu.AccountHolderRegistration:
                                string accountholderid = this.AccountHolderRegistration();
                                Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                                break;
                            case Employeemenu.UpdateAccountHolderName:
                                string userid = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string name = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                employeeservice.UpdateAccountHolderName(userid, name);
                                break;
                            case Employeemenu.UpdateAccountHolderPhoneNumber:
                                string userid1 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string phonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                employeeservice.UpdateAccountHolderPhoneNumber(userid1, phonenumber);
                                break;
                            case Employeemenu.UpdateAccountHolderGender:
                                string userid2 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                GenderType gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                employeeservice.UpdateAccountHolderGender(userid2, gender);
                                break;
                            case Employeemenu.UpdateAccountHolderAddress:
                                string userid3 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string address = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                employeeservice.UpdateAccountHolderAddress(userid3, address);
                                break;
                            case Employeemenu.RevertTransaction:
                                string transid = Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                                employeeservice.revertTransaction(transid);
                                break;
                            case Employeemenu.DeleteAccountHolderAccount:
                                string userid4 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                employeeservice.DeleteAccountHolderAccount(userid4);
                                break;
                            case Employeemenu.Exit:
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
                        throw new UserNotFoundException("Sorry,User do not found with this Id .");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }   
        }


        public void AccountMenu(Admin admin)
        {
            AccountService accountservice = new AccountService();
            try
            {
                bool accountmenuflag = true;
                while (accountmenuflag)
                {
 
                    if (admin != null)
                    {
                        Console.WriteLine("1. EmployeeRegistration \n2. UpdateEmployeeName \n3. UpdateEmployeePhoneNumber \n4. UpdateEmployeeGender \n5. UpdateEmployeeAddress \n6. DeleteEmployeeAccount\n7. AccountHolder Registration\n8. UpdateAccountHolderName \n9. UpdateAccountHolderPhoneNumber \n10. UpdateAccountHolderGender \n11. UpdateAccountHolderAddress \n12. DeleteAccountHolderAccount\n13. RevertTransaction\n14. UpdateBankName\n15. UpdateBankBranchName\n16. Exit");
                        Console.Write("Please select your option   :   ");
                        Accountmenu option2 = (Accountmenu)Enum.Parse(typeof(Accountmenu), Console.ReadLine(), true);
                        switch (option2)
                        {
                            case Accountmenu.EmployeeRegistration:
                                string employeeid = this.EmployeeRegistration();
                                Console.WriteLine("The Employee Id is  :  " + employeeid);
                                break;
                            case Accountmenu.UpdateEmployeeName:
                                string employeeid1 = Utilities.GetStringInput("Enter the employeeid  :  ", true);
                                string employeename = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                accountservice.UpdateEmployeeName(employeeid1, employeename);
                                break;
                            case Accountmenu.UpdateEmployeePhoneNumber:
                                string employeeid2 = Utilities.GetStringInput("Enter the employeeid  :  ", true);
                                string employeephonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                accountservice.UpdateEmployeeName(employeeid2, employeephonenumber);
                                break;
                            case Accountmenu.UpdateEmployeeGender:
                                string employeeid3 = Utilities.GetStringInput("Enter the employeeid  :  ", true);
                                GenderType employeegender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                accountservice.UpdateEmployeeGender(employeeid3, employeegender);
                                break;
                            case Accountmenu.UpdateEmployeeAddress:
                                string employeeid4 = Utilities.GetStringInput("Enter the employeeid  :  ", true);
                                string employeeaddress = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                accountservice.UpdateEmployeeAddress(employeeid4, employeeaddress);
                                break;
                            case Accountmenu.DeleteEmployeeAccount:
                                string employeeid5 = Utilities.GetStringInput("Enter the employeeid  :  ", true);
                                accountservice.DeleteEmployeeAccount(employeeid5);
                                break;
                            case Accountmenu.AccountHolderRegistration:
                                string accountholderid = this.AccountHolderRegistration();
                                Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                                break;
                            case Accountmenu.UpdateAccountHolderName:
                                string userid = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string name = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                accountservice.UpdateAccountHolderName(userid, name);
                                break;
                            case Accountmenu.UpdateAccountHolderPhoneNumber:
                                string userid1 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string phonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                accountservice.UpdateAccountHolderPhoneNumber(userid1, phonenumber);
                                break;
                            case Accountmenu.UpdateAccountHolderGender:
                                string userid2 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                GenderType gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                accountservice.UpdateAccountHolderGender(userid2, gender);
                                break;
                            case Accountmenu.UpdateAccountHolderAddress:
                                string userid3 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                string address = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                accountservice.UpdateAccountHolderAddress(userid3, address);
                                break;
                            case Accountmenu.DeleteAccountHolderAccount:
                                string userid4 = Utilities.GetStringInput("Enter the userid  :  ", true);
                                accountservice.DeleteAccountHolderAccount(userid4);
                                break;
                            case Accountmenu.RevertTransaction:
                                string transid = Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                                accountservice.revertTransaction(transid);
                                break;
                            case Accountmenu.UpdateBankName:
                                string BankId = Utilities.GetStringInput("Enter the  Bank Id  :  ", true);
                                string bankname = Utilities.GetStringInput("Enter the  Bank Name  :  ", true);
                                accountservice.UpdateBankName(BankId, bankname);
                                break;
                            case Accountmenu.UpdateBankBranchName:
                                string bankId = Utilities.GetStringInput("Enter the  Bank Id  :  ", true);
                                string bankbranchname = Utilities.GetStringInput("Enter the  Bank Name  :  ", true);
                                accountservice.UpdateBankBranchName(bankId, bankbranchname);
                                break;
                            case Accountmenu.Exit:
                                admin = null;
                                accountmenuflag = false;
                                break;
                            default:
                                Console.WriteLine("Your option is not valid");
                                break;
                        }
                    }
                    else
                    {
                        throw new UserNotFoundException("Sorry,User do not found with this Id .");
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
