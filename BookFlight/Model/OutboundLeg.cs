using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookFlight.Model
{
    public class OutboundLeg
    {
        public List<int> CarrierIds { get; set; }
        public string OriginId { get; set; }

        public string DestinationId { get; set; }

        public string DepartureDate { get; set; }
    }
}