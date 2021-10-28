using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Models
{
    public class OutofMoneyException:Exception
    {
        public OutofMoneyException(string msg):base(msg)
        {

        }
    }
}
