using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
	public class Bank
	{
		public Bank()
        {
			this.CurrencyCodes = new List<CurrencyCode>();

			this.Branches = new List<Branch>();

			this.Employees = new List<Employee>();

			this.AccountHolders = new List<AccountHolder>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public int IMPSChargesforSameBank { get; set; }
		public int RTGSChargesforSameBank { get; set; }
		public int IMPSChargesforDifferentBank { get; set; }
		public int RTGSChargesforDifferentBank { get; set; }
		public List<CurrencyCode> CurrencyCodes { get; set; }
		public List<Branch> Branches { get; set; }
        public List<Employee> Employees { get; set; }
        public List<AccountHolder> AccountHolders { get; set; }
	}

    public class CurrencyCode
    {
		public int Id { get; set; }

		public string Code { get; set; }

		public double ExchangeRate { get; set; }

		public bool IsDefault { get; set; }
    }

    public class Branch
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public string IFSCCode { get; set; }

		public string Address { get; set; }

		public bool IsMainBranch { get; set; }
    }
}
