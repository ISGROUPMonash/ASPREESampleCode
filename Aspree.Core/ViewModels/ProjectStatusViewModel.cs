using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ProjectStatusViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public System.Guid Guid { get; set; }
    }
}
