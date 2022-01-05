using System.Collections.Generic;

namespace BankApplication.Models
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            list = new List<T>();
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<T> list { get; set; }
    }

}
