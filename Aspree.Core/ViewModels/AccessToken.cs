using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string name { get; set; }
        public string roles { get; set; }
        public Guid guid { get; set; }
        public Guid tenantId { get; set; }
        public string email { get; set; }
    }

}
