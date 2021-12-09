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
