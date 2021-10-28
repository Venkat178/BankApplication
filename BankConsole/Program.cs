using System;
using BankApplication.Models;
using BankApplication.Services;

namespace BankConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exitFlag = true;
            while (exitFlag)
            {
                Menu m = new Menu();
                m.MainMenu();   
                try
                {
                    Console.Write("Enter the Choice  :  ");
                    EnumMainmenu choice = (EnumMainmenu)Enum.Parse(typeof(EnumMainmenu), Console.ReadLine(), true);
                    switch (choice)
                    {
                        case EnumMainmenu.Register:
                            BankServices bm = new BankServices();
                            Console.Write("Enter the Name   :   ");
                            string HolderName = Console.ReadLine();
                            Console.Write("Enter the Branch Name   :   ");
                            string branchname = Console.ReadLine();
                            Console.Write("Enter the Phone Number to Registered With   :   ");
                            string phnnumber = Console.ReadLine();
                            Console.Write("Enter the Address   :   ");
                            string addrs = Console.ReadLine();
                            try
                            {
                                GenderType gnd = (GenderType)Enum.Parse(typeof(GenderType), Console.ReadLine(), true);
                                Console.Write("Enter the Password   :   ");
                                string pass = Console.ReadLine();
                                Console.WriteLine("Re-Enter the Password");
                                while (pass != Console.ReadLine())
                                    Console.WriteLine("Password not matched!");
                                try
                                {
                                    bm.CreateAccount(HolderName, pass, branchname, phnnumber, addrs, gnd);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                Console.WriteLine("Your are one of the Member of Tech Bank.Great to See You.Your Account Created Successfully");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                            
                            break;
                        case EnumMainmenu.Login:
                            BankAccount b = new BankAccount();
                            Console.Write("Enter the  userId   :   ");
                            string uid = Console.ReadLine();
                            int userfoundflag = 0;
                            foreach (var i in Bank.BankAccounts)
                            {
                                if (i.Id == uid)
                                {
                                    userfoundflag = 1;
                                    break;
                                }
                            }
                            if(userfoundflag==0)
                            {
                                try
                                {
                                    throw new UserNotFoundException("Sorry,User do not found with this Id .");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                
                            }
                            Console.Write("Enter the  password   :   ");
                            string pwd = Console.ReadLine();
                            BankAccount user = b.Login(uid, pwd);
                            if (user != null)
                            {
                                bool exitFlag1 = true;
                                while (exitFlag1)
                                {
                                    m.LoginMenu();
                                    Console.Write("Enter the Choice   :   ");
                                    EnumLoginmenu ch = (EnumLoginmenu)Enum.Parse(typeof(EnumLoginmenu), Console.ReadLine(), true);
                                    switch (ch)
                                    {
                                        case EnumLoginmenu.Deposit:
                                            Console.Write("Enter the Amount");
                                            double amt = Convert.ToInt32(Console.ReadLine());
                                            b.Deposit(uid, amt);
                                            break;
                                        case EnumLoginmenu.Withdraw:
                                            try
                                            {
                                                Console.Write("Enter the Amount");
                                                double amt1 = Convert.ToInt32(Console.ReadLine());
                                                b.Withdraw(uid, amt1);
                                            }
                                            catch(Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                            }
                                            break;
                                        case EnumLoginmenu.Transfer:
                                            Console.Write("Enter the Reciever's BankId   :   ");
                                            string recuid = Console.ReadLine();
                                            Console.Write("Enter the Amount");
                                            double amt2 = Convert.ToInt32(Console.ReadLine());
                                            b.Transfer(uid, recuid, amt2);
                                            break;
                                        case EnumLoginmenu.ViewBalance:
                                            b.ViewBalance(uid);
                                            break;
                                        case EnumLoginmenu.ViewTransactions:
                                            b.ViewTransactions(uid);
                                            break;
                                        case EnumLoginmenu.Logout:
                                            b.logout();
                                            exitFlag1 = false;
                                            break;
                                        default:
                                            Console.WriteLine("Enter a valid Choice");
                                            break;
                                    }
                                }
                            }
                            break;
                        case EnumMainmenu.Exit:
                            exitFlag = false;
                            Console.WriteLine("Have A Great Day with Tech Bank.");
                            break;
                        default:
                            Console.WriteLine("Enter valid Choice");
                            break;

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }

        }
    }

    class Menu
    {
        public void MainMenu()
        {
            Console.WriteLine(" Welcome to Tech Bank .\n Create Account .\n View Id .\n Login .\n Exit .");
        }

        public void LoginMenu()
        {
            Console.WriteLine(" Welcome to Tech Bank .\n Deposit .\n Withdraw .\n Transfer .\n View Balance .\n View Transactions .\n Logout .");
        }
    }
}

