using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.Stat
{
    public class Statistics : IStatistics
    {        
        public double spend { get; set; }     
        public uint results { get; set; }        
        public double cpa { get; set; }
        public double total { get; set; }
    }
}
