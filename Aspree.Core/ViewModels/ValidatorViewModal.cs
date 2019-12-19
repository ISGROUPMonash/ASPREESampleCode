using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ValidatorViewModal
    {
        public Guid? Guid { get; set; }
        [Required]
        public string Property { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
