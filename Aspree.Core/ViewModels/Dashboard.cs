using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class DashboardStatus
    {
        public int UserCount { get; set; }
        public int ActiveUser { get; set; }
        public int RoleCount { get; set; }
        public int ActiveRoles { get; set; }
    }

    public class DashboardFilter
    {
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
        
    }
}
