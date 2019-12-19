using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels.MongoViewModels
{
    /// <summary>
    /// 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class FormDataEntryMongo
    {
        /// <summary>
        /// 
        /// </summary>
        public ObjectId Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FormDataEntryMongo()
        {
            formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int ProjectVersion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ProjectGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int FormId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid FormGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FormTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ActivityGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EntityTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid EntityTypeGuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CreatedById { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid CreatedByGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedByName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? ModifiedDate { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Guid? ModifiedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? ThisUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ThisUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ThisUserGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int64 EntityNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int64? ParentEntityNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FormDataEntryVariableMongo> formDataEntryVariableMongoList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SummaryPageActivityObjId { get; set; }


        /// <summary>
        /// GUID of form data entry deactivator
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of form data entry deactivation
        /// </summary>
        public Nullable<DateTime> DateDeactivated { get; set; }

        /// <summary>
        /// Guid of project used for project linkage
        /// </summary>
        public Guid? ProjectLinkage_LinkedProjectId { get; set; }
        /// <summary>
        /// is active user of project for project linkage
        /// </summary>
        public bool? ProjectLinkage_IsActiveProjectUser { get; set; }
        /// <summary>
        /// Date of project left for project linkage
        /// </summary>
        public DateTime? ProjectLinkage_ProjectLeftDate { get; set; }


        public string ThisUserEmail { get; set; }
        public string ThisUserPhone { get; set; }
        public string ThisUserAddress { get; set; }
        public string ThisUserSuburb { get; set; }
        public string ThisUserState { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class FormDataEntryVariableMongo
    {
        /// <summary>
        /// 
        /// </summary>
        public int VariableId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid VariableGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SelectedValues { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Int64? ParentId { get; set; }

        //public string FileName { get; set; }
    }

}
