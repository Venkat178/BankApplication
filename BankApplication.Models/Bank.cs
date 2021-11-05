using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
	public class Bank
	{
		public string Id { get; set; }
		public string BranchName { get; set; }
		public string IFSCCode { get; set; }
		public string BankName { get; set; }
		public string CurrencyCode { get; set; }
		public Admin Admin { get; set; }

        public List<Employee> Employees = new List<Employee>();

        public List<BankAccount> BankAccounts = new List<BankAccount>();



	}

}
