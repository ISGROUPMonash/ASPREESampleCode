using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EntityTypeViewModel
    {
        /// <summary>
        /// Id of entity type
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of Entity Type
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Guid of Entity Type
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Guid of tenant
        /// </summary>
        public Guid? TenantId { get; set; }
    }

    public class NewEntityTypeViewModel
    {
        /// <summary>
        /// Name of Entity Type
        /// </summary>
        [Required]
        public string Name { get; set; }
    }

    public class EditEntityTypeViewModel
    {
        /// <summary>
        /// Name of Entity Type
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
