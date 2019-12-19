using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ApiRequestViewModel
    {
        public ApiRequestViewModel()
        {
            CreatedDate = DateTime.UtcNow;
            NewGuid = Guid.NewGuid();
        }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid NewGuid { get; set; }
    }
   

}
