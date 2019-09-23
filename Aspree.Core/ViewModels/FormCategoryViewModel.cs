using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormCategoryViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of Form Category
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Form Category created by
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Form Category created date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Form category modified by
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        ///  Form category Modified Date
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Form category deactivated date
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// list of subcategory
        /// </summary>
        public IList<SubCategoryViewModel> Forms { get; set; }
        /// <summary>
        /// Tenat Id
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// IsDefaultFormCategory
        /// </summary>
        public int IsDefaultFormCategory { get; set; }
    }

    
}
