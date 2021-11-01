﻿using System;
using System.Collections.Generic;

namespace BankApplication.Services
{
    class ExchangeRate
    {
        Dictionary<string, double> ExchangeRateList = new Dictionary<string, double>(){
                {"INR", 1.0},
                {"AFN", 0.87},
                {"AMD", 0.15 },
                { "AZN"  , 43.28 } ,
                { "BDT",   0.86} ,
                { "KHR",   0.018} ,
                { "CNY",   11.40} ,
                { "GEL",   23.58},
                { "HKD",   9.46},
                { "IDR",   0.0052},
                { "JPY",   0.67},
                { "KZT",   0.17},
                { "LAK",   0.0077},
                { "MYR",   17.74},
                { "KRW",   0.063},
                { "LKR",   0.37},
                { "MVR",   4.76},
                { "PKR",   0.44},
                { "PHP",   1.47},
                { "THB",   2.25},
                { "TWD",   2.66},
                { "VND",   0.0032},
                { "BHD",   195.22},
                { "AED",   20.03},
                { "SAR",   19.62},
                { "OMR",   191.10},
                { "ILS",   22.96},
                { "IQD",    0.050},
                { "IRR",   0.0017},
                { "JOD",   103.78},
                { "KWD",   244.69},
                { "LBP",   0.049},
                { "QAR",   20.21},
                { "LYD",   16.28},
                { "BAM",   44.50},
                { "EUR",   86.99},
                { "BGN",   44.48},
                { "CZK",   3.42},
                { "HRK",   11.62},
                { "DKK",   11.70},
                { "HUF",   0.25},
                { "ISK",   0.58},
                { "MDL",   4.21},
                { "MKD",    1.41},
                { "NOK",   8.49},
                { "PLN",   19.14},
                { "RON",   17.59},
                { "RSD",   0.74},
                { "RUB",   1.01},
                { "NOK",   8.49},
                { "SEK",   8.55},
                { "CHF",   80.17},
                { "TRY",   6.71},
                { "UAH",   2.75},
                { "GBP",   101.92},
                { "ARS",   0.75},
                { "BOB",   10.66},
                { "BRL",   13.94},
                { "CLP",   0.093},
                { "COP",   0.019},
                { "PEN",   17.95},
                { "PYG",   0.011},
                { "UYU",   1.72},
                { "VEF",   0.000000000184327},
                { "USD",   73.58},
                { "CAD",   58.15},
                { "AWG",   40.86},
                { "BBD",   36.45},
                { "BMD",   73.58},
                { "BSD",   73.56},
                { "DOP",   1.29},
                { "JMD",   0.49},
                { "MXN",   3.69},

                { "ZAR",   5.20},
                { "EGP",   4.68},
                { "GHS",   12.32},
                { "GMD",   1.44},
                { "KES",   0.67},
                { "MAD",   8.23},
                { "MUR",   1.74},
                { "NAD",   4.49},
                { "NGN",   0.18},
                { "SCR",   5.77},
                { "TND",   26.37},
                { "UGX",   0.021},
                { "XAF",   0.13},

                { "AUD",   54.27},
                { "FJD",   35.46},
                { "NZD",   52.38},
                { "XPF",    0.72}
                                   };

        public double ExchangeRateCoverter(double amt, string currencycode)
        {
            return amt * ExchangeRateList[currencycode];
        }

    }
}

