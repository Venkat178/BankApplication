using System;

namespace BankApplication.Models
{
    public class UserNotFoundException:Exception
    {
        public UserNotFoundException(string msg):base(msg)
        {

        }
    }
}
