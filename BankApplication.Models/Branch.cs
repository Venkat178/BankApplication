﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Models
{
    public class Branch
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string IFSCCode { get; set; }

        public string Address { get; set; }

        public bool IsMainBranch { get; set; }
    }
}
