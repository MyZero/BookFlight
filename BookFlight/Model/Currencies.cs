using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookFlight.Model
{
    public class Currencies
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public string SymbolOnLeft { get; set; }
        public string SpaceBetweenAmountAndSymbol { get; set; }
        public string RoundingCoefficient { get; set; }
        public string DecimalDigits { get; set; }
    }
}