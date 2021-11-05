using System;
using System.Collections.Generic;

namespace BankApplication.Services
{
    class ExchangeRate
    {
        Dictionary<string, double> ExchangeRateList = new Dictionary<string, double>() { };
        public double ExchangeRateCoverter(double amt, string currencycode)
        {
            return amt * ExchangeRateList[currencycode];
        }

    }
}

