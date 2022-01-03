using System.Collections.Generic;
namespace BankApplication.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            BranchList = new List<Branch>();
            AccountHolderList = new List<AccountHolder>();
            EmployeeList = new List<Employee>();
            TransactionList = new List<Transaction>();
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<Branch> BranchList { get; set; }
        public List<AccountHolder> AccountHolderList { get; set; }
        public List<Employee> EmployeeList { get; set; }
        public List<Transaction> TransactionList { get; set; }
    }

}
