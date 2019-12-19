using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class LookupDataViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public Guid VariableTypeId { get; set; }
        public Nullable<int> LookupDataType { get; set; }
        public Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public Guid Guid { get; set; }
    }

    public class NewLookupDataViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public Guid VariableTypeId { get; set; }
        public Nullable<int> LookupDataType { get; set; }
    }

    public class EditLookupDataViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public Guid VariableTypeId { get; set; }
        public Nullable<int> LookupDataType { get; set; }
        //[Required]
        //public Guid Guid { get; set; }
    }
}
