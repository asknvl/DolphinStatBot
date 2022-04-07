using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.Users
{
    public interface IUser
    {
        public uint id { get; set; }
        public string login { get; set; }
        public string display_name { get; set; }
        public int role_id { get; set; }
        public int? teamlead_id { get; set; }
        public string tags { get; set; }
        public int status { get; set; }
        public int accounts_count { get; set; }

    }
}
