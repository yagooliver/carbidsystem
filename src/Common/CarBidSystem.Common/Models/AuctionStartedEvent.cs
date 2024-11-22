using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Common.Models
{
    public class AuctionStartedEvent
    {
        public int AuctionId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int State { get; set; }
    }
}
