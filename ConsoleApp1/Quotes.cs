using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsoleApp1
{
    public class Quotes
    {
        public string QuoteId { get; set; }
        public string MinPrice { get; set; }
        public string Direct { get; set; }
        public string QuoteDateTime { get; set; }
        public OutboundLeg Outboudleg {get;set;}
    }
}