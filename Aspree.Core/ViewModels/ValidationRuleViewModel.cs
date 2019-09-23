using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ValidationRuleViewModel
    {

        public int Id { get; set; }
        /// <summary>
        /// Type of Validation Rule
        /// </summary>
        public string RuleType { get; set; }
        /// <summary>
        /// MinRange
        /// </summary>
        public Nullable<double> MinRange { get; set; }
        /// <summary>
        /// MaxRange
        /// </summary>
        public Nullable<double> MaxRange { get; set; }
        /// <summary>
        /// Id of RegEx
        /// </summary>
        public Nullable<int> RegExId { get; set; }
        /// <summary>
        /// ErrorMessage
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// RegEx
        /// </summary>
        public string RegEx { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
    }

    public class NewValidationRuleViewModel
    {
        [Required]
        public string RuleType { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public Nullable<Guid> RegExId { get; set; }
        [Required]
        public string ErrorMessage { get; set; }
    }

    public class EditValidationRuleViewModel
    {
        [Required]
        public string RuleType { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public Nullable<Guid> RegExId { get; set; }
        [Required]
        public string ErrorMessage { get; set; }
        //[Required]
        //public System.Guid Guid { get; set; }
    }
}
