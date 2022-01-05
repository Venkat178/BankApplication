using BankApplication.Models;

namespace BankApplication.Concerns
{
    public interface IEmployeeService
    {
        public APIResponse<Employee> CreateEmployee(Employee employee);
        public APIResponse<Employee> UpdateEmployee(Employee employee);
        public APIResponse<Employee> DeleteEmployee(int employeeid);
        public Employee GetEmployee(int employeeid);
        public APIResponse<Employee> ViewAllEmployees(int bankid);
        public bool IsExitEmployee(int employeeid);
    }
}
