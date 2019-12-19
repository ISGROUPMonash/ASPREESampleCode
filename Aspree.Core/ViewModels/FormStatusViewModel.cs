using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormStatusViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public System.Guid Guid { get; set; }
    }
}
