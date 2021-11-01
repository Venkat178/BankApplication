using System;
using BankApplication.Models;
using BankApplication.Services;
using System.Collections.Generic;

namespace BankApplicationConsole
{
    class BankApplication
    {
        public BankServices BankService;
        public Bank bank;
        public BankStaff bankstaff;

        public BankApplication()
        {
            this.BankService = new BankServices();
            this.bank = new Bank();
            this.bankstaff = new BankStaff();
            this.MainMenu();
        }

        public void MainMenu()
        {
            bool exitFlag = true;
            while(exitFlag)
            {
                Console.WriteLine("0. SetUpBank \n1. Register \n2. Login \n3. Exit ");
                try
                {
                    Console.WriteLine("Please select your option");
                    Mainmenu choice = (Mainmenu)Enum.Parse(typeof(Mainmenu), Console.ReadLine(), true);
                    switch (choice)
                    {
                        case Mainmenu.SetUpBank:
                            try
                            {
                                bank.BankName = Utilities.GetStringInput("Enter the Bank name   :   ", true);
                                bank.BranchName = Utilities.GetStringInput("Enter the Branch Name   :   ", true);
                                bank.IFSCCode = Utilities.GetStringInput("Enter the IFSC Code   :   ", true);
                                bank.Id = bank.BankName + DateTime.Now.ToString("yyyyMMddHHmmss");
                                bank.CurrencyCode = "INR";
                                bool isSuccess = this.BankService.SetUpBank(bank);
                                if (isSuccess)
                                {
                                    Console.WriteLine("Bank is Successfuly Created");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        case Mainmenu.Register:
                            try
                            {
                                BankAccount bankaccount = new BankAccount();
                                bankaccount.Name = Utilities.GetStringInput("Enter the Your Name   :   ", true);
                                bankaccount.Id = bankaccount.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                                Console.WriteLine("Our Branches are   :   ");
                                BankService.ViewAllBankBranches();

                                bankaccount.BranchName = Utilities.GetStringInput("Enter the Branch Name from the above   :   ", true);
                                foreach (var i in BankDatabase.Banks)
                                {
                                    if (i.BranchName == bankaccount.BranchName)
                                    {
                                        bankaccount.BankId = i.Id;
                                    }
                                }
                                if (bank.Id != bankaccount.BankId)
                                {
                                    Console.WriteLine("This Bank do not have a branch in where you selected.");
                                    break;
                                }
                                bankaccount.PhoneNumber = Utilities.GetStringInput("Enter the Phone number   :   ", true);
                                bankaccount.Address = Utilities.GetStringInput("Enter the Your Address   :   ", true);
                                bankaccount.Gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                string userid = BankService.Register(bank, bankaccount);
                                Console.WriteLine("Your User Id   :   " + userid);

                                string password = Utilities.GetStringInput("Enter the password   :   ", true);
                                string recheckpassowrd = Utilities.GetStringInput("Re-Enter the password   :   ", true);
                                while (password != recheckpassowrd)
                                    Console.WriteLine("Password does not matched! Please try Again");
                                bank.BankAccounts.Add(bankaccount);
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            break;
                        case Mainmenu.Login:

                            int userfoundflag1 = 0;
                            string userid1 = Utilities.GetStringInput("Enter the user Id   :   ", true);
                            foreach(var i in bank.Employees)
                            {
                                if (i.Id==userid1)
                                {
                                    userfoundflag1 = 1;
                                }
                            }
                            if (userfoundflag1==1)
                            {
                                this.BankStaff(userid1);
                            }
                            this.UserMenu(userid1);
                            break;
                        case Mainmenu.Exit:
                            exitFlag = false;
                            Console.WriteLine("Have A Great Day with Tech Bank.");
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
            }
        }

        public void UserMenu(string uid)
        {
            try
            {
                string password = Utilities.GetStringInput("Enter the Password   :   ", true);
                BankAccount user = BankService.Login(bank, uid, password);
                if (user!=null)
                {
                    bool exitFlag1 = true;
                    while (exitFlag1)
                    {
                        Console.WriteLine("0. Deposit \n1. Withdraw \n2. Transfer \n3. View Balance \n4. View Transactions \n5. Logout .");

                        Console.Write("Please select your option   :   ");
                        Usermenu ch = (Usermenu)Enum.Parse(typeof(Usermenu), Console.ReadLine(), true);
                        switch (ch)
                        {
                            case Usermenu.Deposit:
                                double amt = Convert.ToDouble(Utilities.GetStringInput("Enter the Amount to Deposit  ", true));
                                BankService.Deposit(bank, uid, amt);
                                break;
                            case Usermenu.Withdraw:
                                try
                                {
                                    Console.Write("Enter the Amount");
                                    double amt1 = Convert.ToDouble(Console.ReadLine());
                                    BankService.Withdraw(bank, uid, amt1);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                break;
                            case Usermenu.Transfer:
                                Console.Write("Enter the Reciever's BankId   :   ");
                                string recuid = Console.ReadLine();
                                Console.Write("Enter the Amount");
                                double amt2 = Convert.ToDouble(Console.ReadLine());
                                BankService.Transfer(bank, uid, recuid, amt2);
                                break;
                            case Usermenu.ViewBalance:
                                BankService.ViewBalance(bank, uid);
                                break;
                            case Usermenu.ViewTransactions:
                                BankService.ViewTransactions(bank, uid);
                                break;
                            case Usermenu.Logout:
                                //BankService.logout();
                                exitFlag1 = false;
                                break;
                            default:
                                Console.WriteLine("Your Option is Not Valid");
                                break;
                        }
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

        public void BankStaff(string userid)
        {
            try
            {
                string password = Utilities.GetStringInput("Enter the Password   :   ", true);
                BankAccount user = BankService.Login(bank, userid, password);
                if (user!=null)
                {
                    bool exitFlag = true;
                    while (exitFlag)
                    {
                        Console.WriteLine("0. UpdateName \n1. UpdateAddress \n2. UpdatePhoneNumber \n3. UpdateGender \n4. RevertTransaction\n5. DeleteAccount\n6.Exit");
                        Console.Write("Please select your option   :   ");
                        Bankstaff choice2 = (Bankstaff)Enum.Parse(typeof(Bankstaff), Console.ReadLine(), true);
                        switch (choice2)
                        {
                            case Bankstaff.UpdateName:
                                string name = Utilities.GetStringInput("Enter the Name to Update  ", true);
                                bankstaff.UpdateName(bank, userid, name);
                                break;
                            case Bankstaff.UpdatePhoneNumber:
                                string phonenumber = Utilities.GetStringInput("Enter the Phone Number to Update  ", true);
                                bankstaff.UpdateName(bank, userid, phonenumber);
                                break;
                            case Bankstaff.UpdateGender:
                                GenderType gender = (GenderType)Enum.Parse(typeof(GenderType), Utilities.GetStringInput("Enter the Your Gender   :   ", true), true);
                                bankstaff.UpdateGender(bank, userid, gender);
                                break;
                            case Bankstaff.UpdateAddress:
                                string address = Utilities.GetStringInput("Enter the Address to Update  ", true);
                                bankstaff.UpdateName(bank, userid, address);
                                break;
                            case Bankstaff.RevertTransaction:
                                string transid = Utilities.GetStringInput("Enter the TransactionID  ", true);
                                bankstaff.revertTransaction(bank, transid);
                                break;
                            case Bankstaff.DeleteAccount:
                                bankstaff.DeleteAccount(bank, userid);
                                break;
                            case Bankstaff.Exit:
                                exitFlag = false;
                                break;
                            default:
                                Console.WriteLine("Your Option is Not Valid");
                                break;
                        }
                    }
                }
                else
                {
                    throw new UserNotFoundException("Sorry,User do not found with this Id .");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
