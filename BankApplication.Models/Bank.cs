﻿using System;
using System.Collections.Generic;

namespace BankApplication.Models
{
	public class Bank
	{
		public string Id { get; set; }
		public string BranchName { get; set; }
		public string IFSCCode { get; set; }
		public string BankName { get; set; }

		public static List<BankAccount> BankAccounts = new List<BankAccount>();


	}

}
