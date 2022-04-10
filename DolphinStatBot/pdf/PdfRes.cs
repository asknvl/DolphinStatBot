using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.pdf
{
    public class PdfRes
    {
        public string Date { get; set; }
        public string Time { get; set; }                 
        public MemoryStream PdfStream { get; set; }
    }
}
