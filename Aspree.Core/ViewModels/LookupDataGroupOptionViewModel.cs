using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class LookupDataGroupOptionViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public Guid LookupDataOptionGroupId { get; set; }
        public System.Guid Guid { get; set; }

    }

    public class NewLookupDataGroupOptionViewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid LookupDataOptionGroupId { get; set; }
    }

    public class EditLookupDataGroupOptionViewModel
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public Guid LookupDataOptionGroupId { get; set; }
        //[Required]
        //public System.Guid Guid { get; set; }
    }
}
