using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessMonitorUI.Models
{
    public class AuthorizationData
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
