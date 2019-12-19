using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class RegExRuleViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        [Required]
        public string RegExName { get; set; }
        [Required]
        public string RegEx { get; set; }
        public string Description { get; set; }
        public Nullable<System.Guid> Guid { get; set; }
        public Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
    }

    public class NewRegExRuleViewModel
    {
        [Required]
        public string RegExName { get; set; }
        [Required]
        public string RegEx { get; set; }
        public string Description { get; set; }
    }

    public class EditRegExRuleViewModel
    {
        //[Required]
        //public Guid Guid { get; set; }
        [Required]
        public string RegExName { get; set; }
        [Required]
        public string RegEx { get; set; }
        public string Description { get; set; }
    }
}
