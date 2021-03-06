﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class VariableCategoryViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of Category
        /// </summary>
        [Required]
        public string CategoryName { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Variable Category Created By
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// CreatedDate of Variable category 
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Variable category modified by
        /// </summary>
        public Guid? ModifiedBy { get; set; }
        /// <summary>
        /// ModifiedDate of Variable category
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// Variable category DeactivatedBy
        /// </summary>
        public Guid? DeactivatedBy { get; set; }
        /// <summary>
        /// DeactivatedDate of Variable category
        /// </summary>
        public DateTime? DeactivatedDate { get; set; }
        /// <summary>
        /// GUID of Tenat
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// List of Variables
        /// </summary>
        public IList<SubCategoryViewModel> Variables { get; set; }
        /// <summary>
        /// IsDefaultVariableCategory
        /// </summary>
        public int IsDefaultVariableCategory { get; set; }
    }

}
