using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class NewStatus
    {
        [Required]
        public string Status { get; set; }
    }
}
