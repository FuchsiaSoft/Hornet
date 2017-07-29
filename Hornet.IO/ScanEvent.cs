using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO
{
    public class ScanEvent
    {
        public ScanEventType EventType { get; set; }
        public DateTime EventTimeStamp { get; set; }
    }
}
