using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class CheckListViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
    }

    public class NewCheckListViewModel
    {
        [Required]
        public string Title { get; set; }
    }

    public class EditCheckListViewModel
    {
        //public Guid Guid { get; set; }
        [Required]
        public string Title { get; set; }
    }
}
