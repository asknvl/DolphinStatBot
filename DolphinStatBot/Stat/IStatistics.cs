using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.Stat
{
    public interface IStatistics
    {
        double spend { get; set; }
        uint results { get; set; }
        double cpa { get; set; }
    }
}
