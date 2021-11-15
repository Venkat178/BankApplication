using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApplication.Models
{
    public class WrongPasswordException:Exception
    {
        public WrongPasswordException(string msg):base(msg)
        {

        }
    }
}
