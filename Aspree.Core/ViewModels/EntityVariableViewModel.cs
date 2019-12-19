using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EntityVariableViewModel
    {
        public EntityVariableViewModel()
        {
            EntityVariableList = new List<EntityVariableViewModel>();
        }
        public List<EntityVariableViewModel> EntityVariableList { get; set; }
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int VariableId { get; set; }
        public System.Guid Guid { get; set; }
        public Guid EntityVariableGuid { get; set; }


        //variable table details
        public int IdVariableTable { get; set; }

        public string VariableName { get; set; }

        public string VariableLabel { get; set; }
        public string Question { get; set; }
        public List<string> Values { get; set; }
        public string ValueDescription { get; set; }
        public string HelpText { get; set; }
        public int VariableTypeId { get; set; }
        public string ValidationMessage { get; set; }
        public string RequiredMessage { get; set; }
        public double? MinRange { get; set; }
        public double? MaxRange { get; set; }
        public string RegEx { get; set; }
        public bool? IsSoftRange { get; set; }
        public int? ValidationRuleId { get; set; }
        public int? DependentVariableId { get; set; }
        public bool IsRequired { get; set; }
        public bool CanCollectMultiple { get; set; }
        public int? VariableCategoryId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        //public string CreatedBy { get; set; }
        //public string CreatedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public string ModifiedDate { get; set; }
        //public string DeactivatedBy { get; set; }
        //public string DateDeactivated { get; set; }
        public Guid VariableGuid { get; set; }
        public int? TenantId { get; set; }
        public string VariableType { get; set; }
        public List<string> VariableValueDescription { get; set; }
    }
}
