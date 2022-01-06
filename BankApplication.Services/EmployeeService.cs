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

        public APIResponse<Employee> CreateEmployee(Employee employee)
        {
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(b => b.Id == employee.BranchId);
                if (branch != null)
                {
                    if (BankAppDbctx.Employees.Any(p => p.Name == employee.Name))
                    {
                        return new APIResponse<Employee>() { IsSuccess = false, Message = "Account already exists!" };
                    }
                    employee.Type = UserType.Employee;
                    employee.EmployeeId = Utilities.GetEmployeeId(employee.Name);//employee.Name.Substring(0, 3) + DateTime.Now.ToString("yyyyMMddHHmmss");
                    BankAppDbctx.Employees.Add(employee);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Employee>() { IsSuccess = true, Message = employee.EmployeeId };
                }
            }
            catch (Exception)
            {
                return new APIResponse<Employee>() { IsSuccess = false, Message = "Error while creating employee!" };
            }

            return new APIResponse<Employee>() { IsSuccess = false, Message = "Branch not found" };
        }

        public APIResponse<Employee> UpdateEmployee(Employee employee)
        {
            try
            {
                var oldemployee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employee.Id);
                if (oldemployee != null)
                {
                    oldemployee.Name = string.IsNullOrEmpty(employee.Name) ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber == default(long) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = string.IsNullOrEmpty(employee.Address) ? oldemployee.Address : employee.Address;
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Employee>() { IsSuccess = true, Message = "Successfully updated" };
                }
            }
            catch (Exception) 
            {
                return new APIResponse<Employee>() { IsSuccess = false, Message = "Error occured while updating employee. Try again" };
            }

            return new APIResponse<Employee>() { IsSuccess = false, Message = "Employee not found" };
        }

        public APIResponse<Employee> DeleteEmployee(int employeeid)
        {
            try
            {
                Employee employee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employeeid);
                if (employee != null)
                {
                    BankAppDbctx.Employees.Remove(employee);
                    BankAppDbctx.SaveChanges();

                    return new APIResponse<Employee>() { IsSuccess = true, Message = "Successfully deleted" };
                }
            }
            catch (Exception) 
            {
                return new APIResponse<Employee>() { IsSuccess = false, Message = "Error occured while deleting" };
            }

            return new APIResponse<Employee>() { IsSuccess = false, Message = "Employee not found" };
        }

        public Employee GetEmployee(int employeeid)
        {
            try
            {
                Employee employee = BankAppDbctx.Employees.FirstOrDefault(employee => employee.Id == employeeid && employee.Type == UserType.Employee);
                return employee;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public APIResponse<Employee> GetEmployeesByBank(int bankid)
        {
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    return new APIResponse<Employee> { list = BankAppDbctx.Employees.Where(b => b.BankId == bankid).ToList(), IsSuccess = true };
                }

                return new APIResponse<Employee> { Message = "Bank not found", IsSuccess = false };
            }
            catch(Exception)
            {
                return new APIResponse<Employee> { Message = "Error occured", IsSuccess = false };
            }
        }

        public bool IsExitEmployee(int employeeid)
        {
            try
            {
                return BankAppDbctx.Employees.Any(p => p.Id == employeeid);
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
