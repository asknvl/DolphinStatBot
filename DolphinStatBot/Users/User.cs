using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinStatBot.Users
{
    public class User
    {
        public uint id { get; set; }

        string _login;
        public string login {
            get => _login;
            set
            {
                if (value == null)
                    _login = "";
                else 
                    _login = value;
            }
        }

        string _display_name;
        public string display_name
        {
            get => _display_name;
            set
            {
                if (value == null)
                    _display_name = "";
                else
                    _display_name = value;
            }
        }
        public int role_id { get; set; }        
        public int? teamlead_id { get; set; }
        public string tags { get; set; }
        public int status { get; set; }
        public int accounts_count { get; set; }
    }
}
