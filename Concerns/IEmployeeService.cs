using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IEmployeeService
    {
        public APIResponse CreateEmployee(Employee employee,int branchid);
        public APIResponse UpdateEmployee(Employee employee);
        public APIResponse DeleteEmployee(int employeeid);
        public Employee GetEmployee(int employeeid);
        public APIResponse ViewAllEmployees(int bankid);
        public bool IsExitEmployee(int employeeid);
    }
}
