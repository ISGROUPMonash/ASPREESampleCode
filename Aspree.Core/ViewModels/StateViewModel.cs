using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class StateViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
        public Guid Guid { get; set; }
    }

    public class NewStateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid CountryId { get; set; }
    }

    public class EditStateViewModel
    {
        //[Required]
        //public Guid Guid { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid CountryId { get; set; }
    }
}
