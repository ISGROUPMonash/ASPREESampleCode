using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class LookupDataOptionGroupViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public Guid LookupDataId { get; set; }
        public System.Guid Guid { get; set; }
    }

    public class NewLookupDataOptionGroupViewModel
    {
        [Required]
        public string Label { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid LookupDataId { get; set; }
    }

    public class EditLookupDataOptionGroupViewModel
    {
        [Required]
        public string Label { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid LookupDataId { get; set; }
        //[Required]
        //public System.Guid Guid { get; set; }
    }
}
