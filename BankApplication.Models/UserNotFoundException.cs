using System;
using System.Runtime.Serialization;

namespace BankApplication.Models
{
    public class UserNotFoundException:Exception
    {
        public UserNotFoundException(string msg):base(msg)
        {

        }
    }
}
