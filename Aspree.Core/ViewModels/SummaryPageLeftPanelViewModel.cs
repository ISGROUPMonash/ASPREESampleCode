using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SummaryPageLeftPanelViewModel
    {
        public string Postcode { get; set; }
        public string Fax { get; set; }
        public string StrtNum { get; set; }
        public string StrtNum2 { get; set; }
        public string StrtNme { get; set; }
        public string StrtNme2 { get; set; }

        /// <summary>
        /// Address of entity
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Suburb of entity
        /// </summary>
        public string Suburb { get; set; }
        /// <summary>
        /// State of entity
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Phone no of entity
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email of entity
        /// </summary>
        public string Email { get; set; }

        public string DefaultFormType { get; set; }
    }
}
