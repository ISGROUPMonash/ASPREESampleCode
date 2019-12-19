using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EntityFormDataVariableViewModel
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int VariableId { get; set; }
        public string SelectedValues { get; set; }
        public Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public System.DateTime? ModifiedDate { get; set; }
        public int? DeactivatedBy { get; set; }
        public System.DateTime? DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        public int EntityTypeId { get; set; }
        public int? EntitySubTypeId { get; set; }
        public string Json { get; set; }

        public Guid? VariableGuid { get; set; }
        public Guid? EntityGuid { get; set; }
        
        [Required(ErrorMessage = "Required EntityTypeGuid")]
        [Display(Name = "Variable Name")]
        public Guid? EntityTypeGuid { get; set; }

        //[Required(ErrorMessage = "Required EntitySubTypeGuid")]
        //[Display(Name = "Variable Name")]
        public Nullable<Guid> EntitySubTypeGuid { get; set; }

        [Required(ErrorMessage = "Required EntityName")]
        [Display(Name = "Variable Name")]
        public string EntityName { get; set; }

        public string EntityTypeName { get; set; }
        public string EntitySubTypeName { get; set; }

    }

    public class EntityFormDataVariableJson
    {
        public EntityFormDataVariableJson()
        {
            Json = new List<JsonObject>();
        }
        public Guid? Guid { get; set; }
        public Guid? EntityTypeGuid { get; set; }
        public Guid? EntitySubTypeGuid { get; set; }
        public string EntityName { get; set; }
        public Guid? TenantId { get; set; }
        public List< JsonObject> Json { get; set; }

        
    }
    public class JsonObject
    {
        public string Question { get; set; }
        public dynamic Answer { get; set; }
        public Guid? VariableId { get; set; }
        public string VariableType { get; set; }
    }
}
