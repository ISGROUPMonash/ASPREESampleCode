using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormDataEntryVariableViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id of Variable
        /// </summary>
        public int VariableId { get; set; }
        /// <summary>
        /// SelectedValues(string)
        /// </summary>
        public string SelectedValues { get; set; }
        /// <summary>
        /// SelectedValues(Int)
        /// </summary>
        public Nullable<int> SelectedValues_int { get; set; }
        /// <summary>
        /// SelectedValues(float)
        /// </summary>
        public Nullable<double> SelectedValues_float { get; set; }
        /// <summary>
        /// Id of FormDataEntry
        /// </summary>
        public int FormDataEntryId { get; set; }
        /// <summary>
        /// FormDataEntry createdBy
        /// </summary>
        public int CreatedBy { get; set; }
        /// <summary>
        /// FormDataEntry created date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// FormDataEntry ModifiedBy
        /// </summary>
        public Nullable<int> ModifiedBy { get; set; }
        /// <summary>
        /// ModifiedBy ModifiedDate
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Form data entry DeactivatedBy
        /// </summary>
        public Nullable<int> DeactivatedBy { get; set; }
        /// <summary>
        ///  Deactivation date of Form data entry
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Name of Variable
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// FormId
        /// </summary>
        public int? FormId { get; set; }
        /// <summary>
        /// ParentId
        /// </summary>
        public int? ParentId { get; set; }
        /// <summary>
        /// ActivityId
        /// </summary>
        public int? ActivityId { get; set; }
        /// <summary>
        /// Guid of Form
        /// </summary>
        public Guid? FormGuid { get; set; }
        /// <summary>
        /// ActivityGuid
        /// </summary>
        public Guid? ActivityGuid { get; set; }
        /// <summary>
        /// Status of FormDataEntry
        /// </summary>
        public int FormDataEntryStatus { get; set; }
        /// <summary>
        /// Form Title
        /// </summary>
        public string FormTitle { get; set; }

        /// <summary>
        /// Variable Type Name
        /// </summary>
        public string VariableTypeName { get; set; }

        public string FileName { get; set; }
    }

    public class FormDataEntryVariableViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FormDataEntryVariableViewModel()
            {

                Id = 1,
                VariableId = 2,
                SelectedValues = "Test Values",
                SelectedValues_int = 1,
                SelectedValues_float = 1.0,
                FormDataEntryId = 1,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = 1,
                ModifiedDate = DateTime.UtcNow,
                DeactivatedBy = 1,
                DateDeactivated = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                VariableName = "Test Variable",
                FormId = 1,
                ParentId = 1,
                ActivityId = 1,
                FormGuid = Guid.NewGuid(),
                ActivityGuid = Guid.NewGuid(),
                FormDataEntryStatus = 1,
                FormTitle = "Example Title"

            };
        }
    }
}
