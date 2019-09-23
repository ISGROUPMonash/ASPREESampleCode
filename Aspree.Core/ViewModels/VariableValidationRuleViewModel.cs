using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class VariableValidationRuleViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Id of Variable
        /// </summary>
        public int VariableId { get; set; }
        /// <summary>
        /// Validation Id
        /// </summary>
        public Nullable<int> ValidationId { get; set; }
        /// <summary>
        /// ValidationMessage
        /// </summary>
        public string ValidationMessage { get; set; }
        /// <summary>
        /// RegEx
        /// </summary>
        public string RegEx { get; set; }
        /// <summary>
        /// Min
        /// </summary>
        public Nullable<double> Min { get; set; }
        /// <summary>
        /// Max
        /// </summary>
        public Nullable<double> Max { get; set; }
        /// <summary>
        /// LimitType
        /// </summary>
        public string LimitType { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Name of Validation
        /// </summary>
        public string ValidationName { get; set; }

    }
}
