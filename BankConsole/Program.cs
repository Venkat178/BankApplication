using BankApplication.Concerns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BankApplication.Services;

namespace BankApplication
{
    class Program
    {
        static void Main(string[] args )
        {
            var host = CreateHostBuilder(args).Build();
            BankApplication bankapp = new BankApplication();
            bankapp.Initialize(host);
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<IAccountHolderService, AccountHolderService>();
                    services.AddTransient<IAccountService, AccountService>();
                    services.AddTransient<IBankService, BankService>();
                    services.AddTransient<IEmployeeService, EmployeeService>();
                });
        }
    }
}

