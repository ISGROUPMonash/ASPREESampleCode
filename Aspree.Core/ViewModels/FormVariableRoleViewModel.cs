using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormVariableRoleViewModel
    {
        /// <summary>
        /// Id of FormVariable
        /// </summary>
        public int FormVariableId { get; set; }
        /// <summary>
        /// Guid of Role
        /// </summary>
        public System.Guid RoleGuidId { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// CanCreate
        /// </summary>
        public bool CanCreate { get; set; }
        /// <summary>
        /// CanView
        /// </summary>
        public bool CanView { get; set; }
        /// <summary>
        /// CanEdit
        /// </summary>
        public bool CanEdit { get; set; }
        /// <summary>
        /// CanDelete
        /// </summary>
        public bool CanDelete { get; set; }
    }
}
