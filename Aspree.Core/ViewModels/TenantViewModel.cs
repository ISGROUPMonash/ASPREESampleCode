using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class TenantViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        public System.Guid Guid { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
    }
}
