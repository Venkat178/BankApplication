using System;
using System.Collections.Generic;
using System.Linq;
using BankApplication.Models;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Services
{
    public class AccountServices:EmployeeServices
    {
        public Admin AdminLogin(string userid,Admin AdminObj)
        {
            Admin admin= null;
            foreach (var i in BankDatabase.Banks)
            {
                if (i.Admin.Id == userid)
                {
                    //string password1 = Utilities.GetStringInput("Enter the Password  :  ", true);
                    //if (i.Admin.Password == password1)
                    //{
                        admin = i.Admin;
                        //Console.WriteLine("hello");
                        //this.AccountMenu(loginuserid);
                    //}
                    break;
                }
            }
            return admin;
        }

        public void UpdateEmployeeName(string userid, string HolderName)
        {
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    i.Name = HolderName;
                }
            }
        }

        public void UpdateEmployeePhoneNumber(string userid, string phonenumber)
        {
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    i.PhoneNumber = phonenumber;
                }
            }
        }

        public void UpdateEmployeeGender(string userid, GenderType gender)
        {
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    i.Gender = gender;
                }
            }
        }

        public void UpdateEmployeeAddress(string userid, string address)
        {
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    i.Address = address;
                }
            }
        }

        public void DeleteEmployeeAccount(string userid)
        {
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Id == userid)
                {
                    BankDatabase.Employees.Remove(i);
                }
            }
        }

        public string returnEmployeeId(string HolderName)
        {
            string id = string.Empty;
            foreach (var i in BankDatabase.Employees)
            {
                if (i.Name == HolderName)
                {
                    id = i.Id;
                }
            }
            return id;
        }

        //public void 
    }
}
