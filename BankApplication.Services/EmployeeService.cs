using System;
using System.Linq;
using BankApplication.Models;
using BankApplication.Concerns;

namespace BankApplication.Services
{
    public class EmployeeService : IEmployeeService
    {
        public BankApplicationDbContext BankAppDbctx;

        public EmployeeService()
        {
            BankAppDbctx = new BankApplicationDbContext();
        }

        public APIResponse CreateEmployee(Employee employee,int branchid)
        {
            Branch branch = BankAppDbctx.Branches.FirstOrDefault(b=>b.Id == branchid);
            if(branch != null)
            {
                try
                {
                    if (BankAppDbctx.Employees.Any(p => p.Name == employee.Name) == true)
                    {
                        return new APIResponse() { IsSuccess = false, Message = "Account already exists!" };
                    }
                    employee.Type = UserType.Employee;
                    employee.EmployeeId = employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                    BankAppDbctx.Employees.Add(employee);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = employee.EmployeeId };
                }
                catch (Exception)
                {
                    return new APIResponse() { IsSuccess = false, Message = "Error while creating employee!" };
                }
            }
            return new APIResponse() { IsSuccess = false, Message = "Branch not found" };
        }

        public APIResponse UpdateEmployee(Employee employee)
        {
            try
            {
                var oldemployee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employee.Id);
                if (oldemployee != null)
                {
                    oldemployee.Name = employee.Name == string.Empty ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber == default(long) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = employee.Address == string.Empty ? oldemployee.Address : employee.Address;
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully updated" };
                }
            }
            catch (Exception) 
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while updating employee.Try again" };
            }
            return new APIResponse() { IsSuccess = false, Message = "Employee not found" };
        }

        public APIResponse DeleteEmployee(int employeeid)
        {
            try
            {
                Employee employee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employeeid);
                if (employee != null)
                {
                    BankAppDbctx.Employees.Remove(employee);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch (Exception) { }
            return new APIResponse() { IsSuccess = false, Message = "Employee not found" };
        }

        

        public APIResponse DeleteAccountHolderAccount(int userid)
        {
            try
            {
                AccountHolder accountholder = BankAppDbctx.AccountHolders.FirstOrDefault(account => account.Id == userid);
                if (accountholder != null)
                {
                    BankAppDbctx.AccountHolders.Remove(accountholder);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully deleted" };
                }
                else
                {
                    return new APIResponse() { IsSuccess = false, Message = "Account Holder not found" };
                }
            }
            catch (Exception)
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while deleting account holder.Try again" };
            }
            
        }

        public APIResponse RevertTransaction(int transId)
        {
            try
            {
                Transaction transaction = BankAppDbctx.Transactions.FirstOrDefault(trans => trans.Id == transId);
                if (transaction != null)
                {
                    AccountHolder senderAccountHolder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.SrcAccId);
                    AccountHolder receiverAccountHolder = BankAppDbctx.AccountHolders.FirstOrDefault(AccountHolder => AccountHolder.Id == transaction.DestAccId);
                    senderAccountHolder.Balance += transaction.Amount;
                    receiverAccountHolder.Balance -= transaction.Amount;
                    BankAppDbctx.Transactions.Add(transaction);
                    BankAppDbctx.SaveChanges();
                    return new APIResponse() { IsSuccess = true, Message = "Successfully transfered" };
                }
                else
                {
                    Console.WriteLine("Transaction not found");
                    return new APIResponse() { IsSuccess = false, Message = "Transaction not found" };
                }
            }
            catch(Exception)
            {
                return new APIResponse() { IsSuccess = false, Message = "Error occured while transfering.Try again" };
            }
            
        }

        public Employee GetEmployee(int employeeid)
        {
            Employee employee = BankAppDbctx.Employees.FirstOrDefault(employee => employee.Id == employeeid && employee.Type == UserType.Employee);
            return employee;
        }

        public APIResponse ViewAllEmployees(int bankid)
        {
            Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
            if (bank != null)
            {
                return new APIResponse { EmployeeList = BankAppDbctx.Employees.Where(b => b.BankId == bankid).ToList(), IsSuccess = true };
            }
            return new APIResponse { Message = "Bank not found", IsSuccess = false };
        }

        public bool IsExitEmployee(int employeeid)
        {
            return BankAppDbctx.Employees.Any(p => p.Id == employeeid);
        }
    }
}
