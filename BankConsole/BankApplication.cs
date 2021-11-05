using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Services;
using System.Collections.Generic;

namespace BankApplicationConsole
{
    class BankApplication
    {
        public BankServices BankService;
        public Bank Bank;
        //public Employee Employee;
        public EmployeeServices EmployeeService;
        public AccountServices AccountService;
        public Admin AdminObj;
        public Status status;

        public void Initialize()
        {
            this.BankService = new BankServices();
            //this.Bank = new Bank();
            //this.Employee = new Employee();
            this.AccountService = new AccountServices();
            this.AdminObj = new Admin();
            this.status = new Status();
            this.MainMenu();
        }

        public Bank SetUpBank(Status status)
        {
            Bank Bank = new Bank();
            Bank.BankName = Utilities.GetStringInput("Enter the Bank name  :  ", true);
            Bank.BranchName = Utilities.GetStringInput("Enter the Branch Name  :  ", true);
            Bank.IFSCCode = Utilities.GetStringInput("Enter the IFSC Code  :  ", true);
            Bank.Id = Bank.BankName + DateTime.Now.ToString("yyyyMMddHHmmss");
            Bank.CurrencyCode = "INR";
            status = BankService.SetUpBank(Bank);
            if (status.IsSuccess)
            {
                Console.WriteLine("Bank is Successfuly Created");
            }
            else
            {
                Console.WriteLine("Unable to save the details");
            }
            Console.WriteLine("Please provide admin details to set up admin");
            Admin AdminObj = new Admin();
            Bank.Admin = AdminObj;
            try
            {
                AdminObj.Name = Utilities.GetStringInput("Enter the Admin Name  :  ", true);
                AdminObj.BranchName = Bank.BranchName;
                AdminObj.BankId = Bank.Id;
                AdminObj.Type = EnumHolderType.Admin;
                AdminObj.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                if(AdminObj.PhoneNumber.Length!=10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                AdminObj.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                AdminObj.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                Console.WriteLine(AdminObj.BankId);
                Console.WriteLine(Bank.Id);
                AdminObj.Id = AdminObj.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                Console.WriteLine("The Admin Id is " + AdminObj.Id);
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                AdminObj.Password = password;
                BankDatabase.Banks.Add(Bank);
                BankDatabase.Admins.Add(AdminObj);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Bank;
        }

        public string EmployeeRegistration()
        {
            Employee employee = new Employee();
            try
            {
                employee.Name = Utilities.GetStringInput("Enter the Admin Name  :  ", true);
                employee.Admin = AdminObj;
                BankService.ViewAllBankBranches();
                employee.BranchName = Utilities.GetStringInput("Enter the Branch Name from the below  :  ", true);
                employee.Type = EnumHolderType.Employee;
                employee.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                employee.Id = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                if (employee.PhoneNumber.Length != 10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                employee.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                employee.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                //bank.Employees.Add(Employee);
                int flag = 0;
                foreach (var i in BankDatabase.Banks)
                {
                    if (i.BranchName == employee.BranchName)
                    {
                        flag = 1;
                        employee.BankId = i.Id;
                        break;
                    }
                }
                if (flag == 0)
                {
                    throw new Exception("No bank found");
                }
                string password = Utilities.GetStringInput("Enter the new password  :  ", true);
                while (password != Utilities.GetStringInput("Re-Enter the password  :  ", true))
                {
                    Console.WriteLine("Password does not matched! Please try Again");
                }
                employee.Password = password;
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
            BankAccount bankaccount = new BankAccount();
            try
            {   
                bankaccount.Name = Utilities.GetStringInput("Enter the Your Name   :   ", true);
                //bankaccount.Admin = AdminObj;
                Console.WriteLine("Our Branches are   :   ");
                BankService.ViewAllBankBranches();
                bankaccount.BranchName = Utilities.GetStringInput("Enter the Branch Name from the above  :  ", true);
                bankaccount.PhoneNumber = Utilities.GetStringInput("Enter the Phone number  :  ", true);
                if (bankaccount.PhoneNumber.Length != 10)
                {
                    throw new PhoneNumberNotValidException("Phone number is not valid");
                }
                bankaccount.Address = Utilities.GetStringInput("Enter the Your Address  :  ", true);
                bankaccount.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender  :  ", true), true);
                string userid = BankService.Register(bankaccount);
                BankDatabase.TransList[bankaccount.Id] = new List<Transaction>();
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
                            
                            Status status = new Status();
                            this.SetUpBank(status);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case Mainmenu.Login:
                        string loginuserid = Utilities.GetStringInput("Enter the user Id  :  ", true);
                        string password = Utilities.GetStringInput("Enter the Password  :  ", true);
                        try
                        {
                            bool IsAdmin = BankDatabase.Admins.Any(s => s.Id == loginuserid && s.Password == password);
                            if (IsAdmin)
                            {
                                this.AccountMenu(loginuserid);
                                break;
                            }
                            bool IsEmployee = BankDatabase.Employees.Any(s => s.Id == loginuserid && s.Password == password);
                            if (IsEmployee)
                            {
                                this.BankStaff(loginuserid);
                                break;
                            }
                            bool IsUser = BankDatabase.BankAccounts.Any(s => s.Id == loginuserid && s.Password == password);
                            if (IsUser)
                            {
                                this.UserMenu(loginuserid);
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

        

        public void UserMenu(string uid)
        {
            bool menuflag = true;
            while(menuflag)
            {
                try
                {
                    BankAccount user = BankService.Login(uid);
                    if (user != null)
                    {
                        Console.WriteLine("1. Deposit \n2. Withdraw \n3. Transfer \n4. View Balance \n5. View Transactions \n6. Logout .");
                        Console.Write("Please select your option   :   ");
                        Usermenu ch = (Usermenu)Enum.Parse(typeof(Usermenu), Console.ReadLine(), true);
                        switch (ch)
                        {
                            case Usermenu.Deposit:
                                double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the Amount to Deposit  :  ", true));
                                BankService.Deposit(uid, amt);
                                break;
                            case Usermenu.Withdraw:
                                try
                                {
                                    Console.Write("Enter the Amount  :  ");
                                    double amt1 = Convert.ToDouble(Console.ReadLine());
                                    BankService.Withdraw(uid, amt1);
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
                                BankService.Transfer(uid, recuid, amt2);
                                break;
                            case Usermenu.ViewBalance:
                                double balance = BankService.ViewBalance(uid);
                                Console.WriteLine("Your Balance is " + balance);
                                break;
                            case Usermenu.ViewTransactions:
                                BankService.ViewTransactions(uid);
                                break;
                            case Usermenu.Logout:
                                user = null;
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

        public void BankStaff(string userid)
        {
            bool employeemenuflag = true;
            while(employeemenuflag)
            {
                try
                {
                    Employee user = BankService.EmployeeLogin(userid);
                    if (user != null)
                    {
                        Console.WriteLine("1.AccountHolderRegistration \n2. UpdateName \n3. UpdatePhoneNumber \n4. UpdateGender \n5. UpdateAddress \n6. RevertTransaction\n7. DeleteAccount\n8.Exit");
                        Console.Write("Please select your option   :   ");
                        Employeemenu option2 = (Employeemenu)Enum.Parse(typeof(Employeemenu), Console.ReadLine(), true);
                        switch (option2)
                        {
                            case Employeemenu.AccountHolderRegistration:
                                string accountholderid = this.AccountHolderRegistration();
                                Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                                break;
                            case Employeemenu.UpdateAccountHolderName:
                                string name = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                EmployeeService.UpdateAccountHolderName(userid, name);
                                break;
                            case Employeemenu.UpdateAccountHolderPhoneNumber:
                                string phonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                EmployeeService.UpdateAccountHolderPhoneNumber(userid, phonenumber);
                                break;
                            case Employeemenu.UpdateAccountHolderGender:
                                GenderType gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                EmployeeService.UpdateAccountHolderGender(userid, gender);
                                break;
                            case Employeemenu.UpdateAccountHolderAddress:
                                string address = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                EmployeeService.UpdateAccountHolderAddress(userid, address);
                                break;
                            case Employeemenu.RevertTransaction:
                                string transid = Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                                EmployeeService.revertTransaction(transid);
                                break;
                            case Employeemenu.DeleteAccountHolderAccount:
                                EmployeeService.DeleteAccountHolderAccount(userid);
                                break;
                            case Employeemenu.Exit:
                                user = null;
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


        public void AccountMenu(string userid)
        {
            try
            {
                bool accountmenuflag = true;
                while (accountmenuflag)
                {
                    Admin user = AccountService.AdminLogin(userid, AdminObj);
                    if (user != null)
                    {
                        Console.WriteLine("1.EmployeeRegistration \n2. UpdateEmployeeName \n3. UpdateEmployeePhoneNumber \n4. UpdateEmployeeGender \n5. UpdateEmployeeAddress \n6. DeleteEmployeeAccount\n7. AccountHolder Registration\n8. UpdateAccountHolderName \n9. UpdateAccountHolderPhoneNumber \n10. UpdateAccountHolderGender \n11. UpdateAccountHolderAddress \n12. DeleteAccountHolderAccount\n13. RevertTransaction\n14. VeiwAllAccounts\n15. Exit");
                        Console.Write("Please select your option   :   ");
                        Accountmenu option2 = (Accountmenu)Enum.Parse(typeof(Accountmenu), Console.ReadLine(), true);
                        switch (option2)
                        {
                            case Accountmenu.EmployeeRegistration:
                                string employeeid = this.EmployeeRegistration();
                                Console.WriteLine("The Employee Id is  :  " + employeeid);
                                break;
                            case Accountmenu.UpdateEmployeeName:
                                string employeename = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                AccountService.UpdateEmployeeName(userid, employeename);
                                break;
                            case Accountmenu.UpdateEmployeePhoneNumber:
                                string employeephonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                AccountService.UpdateEmployeeName(userid, employeephonenumber);
                                break;
                            case Accountmenu.UpdateEmployeeGender:
                                GenderType employeegender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                AccountService.UpdateEmployeeGender(userid, employeegender);
                                break;
                            case Accountmenu.UpdateEmployeeAddress:
                                string employeeaddress = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                AccountService.UpdateEmployeeAddress(userid, employeeaddress);
                                break;
                            case Accountmenu.DeleteEmployeeAccount:
                                AccountService.DeleteEmployeeAccount(userid);
                                break;
                            case Accountmenu.AccountHolderRegistration:
                                string accountholderid = this.AccountHolderRegistration();
                                Console.WriteLine("The Account Holder Is  :  " + accountholderid);
                                break;
                            case Accountmenu.UpdateAccountHolderName:
                                string name = Utilities.GetStringInput("Enter the Name to Update  :  ", true);
                                //AccountService.UpdateAccountHolderName(userid, name);
                                break;
                            case Accountmenu.UpdateAccountHolderPhoneNumber:
                                string phonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  :  ", true);
                                //AccountService.UpdateAccountHolderPhoneNumber(userid, phonenumber);
                                break;
                            case Accountmenu.UpdateAccountHolderGender:
                                GenderType gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                //AccountService.UpdateAccountHolderGender(userid, gender);
                                break;
                            case Accountmenu.UpdateAccountHolderAddress:
                                string address = Utilities.GetStringInput("Enter the Address to Update  :  ", true);
                                //AccountService.UpdateAccountHolderAddress(userid, address);
                                break;
                            case Accountmenu.DeleteAccountHolderAccount:
                                //AccountService.DeleteAccountHolderAccount(userid);
                                break;
                            case Accountmenu.RevertTransaction:
                                string transid = Utilities.GetStringInput("Enter the TransactionID  :  ", true);
                                //AccountService.revertTransaction(transid);
                                break;
                            case Accountmenu.ViewAllAccounts:
                                string Bankid= Utilities.GetStringInput("Enter the  Bank Id  :  ", true);
                                BankService.ViewAllAccounts(Bankid);
                                break;
                            case Accountmenu.Exit:
                                user = null;
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
