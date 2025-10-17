using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSApp.Data
{
    public class User
    {
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string UserType { get; set; } = "";
        public string Address { get; set; } = "";
    }
}