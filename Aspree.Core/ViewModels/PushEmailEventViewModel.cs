using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
   public class PushEmailEventViewModel
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public System.Nullable<bool> IsEmailEvent { get; set; }
        public string DisplayName { get; set; }

        public Guid Guid { get; set; }
    }
}
