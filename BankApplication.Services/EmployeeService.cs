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
            APIResponse<Employee> apiresponse = new APIResponse<Employee>();
            try
            {
                Branch branch = BankAppDbctx.Branches.FirstOrDefault(b => b.Id == employee.BranchId);
                if (branch != null)
                {
                    employee.Type = UserType.Employee;
                    employee.EmployeeId = Utilities.GenarateEmployeeId(employee.Name);
                    BankAppDbctx.Employees.Add(employee);
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = employee.EmployeeId;
                }
                else
                {
                    apiresponse.Message = "Branch not found";
                }
            }
            catch (Exception)
            {
                return new APIResponse<Employee>() { IsSuccess = false, Message = "Error while creating employee!" };
            }
            return apiresponse;
        }

        public APIResponse<Employee> UpdateEmployee(Employee employee)
        {
            APIResponse<Employee> apiresponse = new APIResponse<Employee>();
            try
            {
                var oldemployee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employee.Id);
                if (oldemployee != null)
                {
                    oldemployee.Name = string.IsNullOrEmpty(employee.Name) ? oldemployee.Name : employee.Name;
                    oldemployee.PhoneNumber = employee.PhoneNumber == default(long) ? oldemployee.PhoneNumber : employee.PhoneNumber;
                    oldemployee.Address = string.IsNullOrEmpty(employee.Address) ? oldemployee.Address : employee.Address;
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = "Successfully updated";
                }
                else
                {
                    apiresponse.Message = "Employee not found";
                }
            }
            catch (Exception) 
            {
                apiresponse.Message = "Error occured while updating employee. Try again";
            }

            return apiresponse;
        }

        public APIResponse<Employee> DeleteEmployee(int employeeid)
        {
            APIResponse<Employee> apiresponse = new APIResponse<Employee>();
            try
            {
                Employee employee = BankAppDbctx.Employees.FirstOrDefault(_ => _.Id == employeeid);
                if (employee != null)
                {
                    BankAppDbctx.Employees.Remove(employee);
                    BankAppDbctx.SaveChanges();
                    apiresponse.IsSuccess = true;
                    apiresponse.Message = "Successfully deleted";
                }
                else
                {
                    apiresponse.Message = "Employee not found";
                }
            }
            catch (Exception) 
            {
                apiresponse.Message = "Error occured while deleting";
            }

            return apiresponse;
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
            APIResponse<Employee> apiresponse = new APIResponse<Employee>();
            try
            {
                Bank bank = BankAppDbctx.Banks.FirstOrDefault(_ => _.Id == bankid);
                if (bank != null)
                {
                    apiresponse.IsSuccess = true;
                    apiresponse.ListData = BankAppDbctx.Employees.Where(b => b.BankId == bankid).ToList();
                }
                else
                {
                    apiresponse.Message = "Bank not found";
                }
                
            }
            catch(Exception)
            {
                apiresponse.Message = "Error occured";
            }
            return apiresponse;
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
