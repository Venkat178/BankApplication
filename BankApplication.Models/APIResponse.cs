using System.Collections.Generic;

namespace BankApplication.Models
{
    public class APIResponse<T>
    {
        public APIResponse()
        {
            this.ListData = new List<T>();
            IsSuccess = false;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<T> ListData { get; set; }
        public T Data { get; set; }
    }

}
