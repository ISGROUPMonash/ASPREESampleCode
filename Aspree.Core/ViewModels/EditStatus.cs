using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EditStatus
    {
        //[Required]
        //public Guid Guid { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
