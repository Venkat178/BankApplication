using System;
using BankApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    class BankStaff
    {
        public void UpdateDetailsOfCustomer(string userid)
        {
            int userfoundflag = 0;
            foreach(var i in Bank.BankAccounts)
            {
                if (i.Id == userid)
                {
                    userfoundflag = 1;
                    BankMenu b = new BankMenu();
                    b.BankStaffMenu();
                    BankStaffMenu choice = (BankStaffMenu)Enum.Parse(typeof(BankStaffMenu), Console.ReadLine(), true);
                    switch (choice)
                    {
                        case BankStaffMenu.UpdateName:
                            Console.Write("Enter the Correct Name   :    ");
                            i.Name=Console.ReadLine();
                            break;
                        case BankStaffMenu.UpdateGender:
                            Console.Write("Enter the Correct Gender   :    ");
                            i.Gender = (GenderType)Enum.Parse(typeof(GenderType), Console.ReadLine(), true); 
                            break;
                        case BankStaffMenu.UpdatePhoneNumber:
                            Console.Write("Enter the Correct PhoneNumber   :    ");
                            i.PhoneNumber = Console.ReadLine();
                            break;
                        case BankStaffMenu.UpdateAddress:
                            Console.Write("Enter the Correct Address   :    ");
                            i.Address = Console.ReadLine();
                            break;
                    }
                }
            }
            try
            {
                if (userfoundflag == 0)
                {
                    throw new UserNotFoundException("Sorry,User do not found with this Id .");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }

    class BankMenu
    {
        public void BankStaffMenu()
        {
            Console.WriteLine(" UpdateName .\n UpdateAddress .\n UpdatePhoneNumber .\n UpdateGender");
        }
    }
}
