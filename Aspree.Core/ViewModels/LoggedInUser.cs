using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class LoggedInUser
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public Guid TenantId { get; set; }

        public string ProjectDisplayName { get; set; }
        public Guid ProjectGuid { get; set; }
        public string ProjectDisplayNameTextColour { get; set; }
    }
}
