using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class EntityViewModel
    {
        public EntityViewModel()
        {
            EntityVariableList = new List<EntityVariableViewModel>();
        }

        /// <summary>
        /// Id of entity
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Guid of entity type
        /// </summary>
        public Guid EntityTypeId { get; set; }
        /// <summary>
        /// Guid of entitysub type
        /// </summary>
        public Nullable<Guid> EntitySubTypeId { get; set; }
        /// <summary>
        /// Name of entity
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Entity id of parent entity
        /// </summary>
        public Nullable<Guid> ParentEntityId { get; set; }
        /// <summary>
        /// Status id of entity
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Guid of enbtity creator
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Date of entity creation.
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Guid of entity modifire
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Date of Modification
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Guid of DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of Deactivated
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid of model
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// List of DroppedVariablesList
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public List<Guid> DroppedVariablesList { get; set; }
        /// <summary>
        /// Guid of tenant
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// List of entity variables
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public List<EntityVariableViewModel> EntityVariableList { get; set; }
        /// <summary>
        /// Name of entity type
        /// </summary>
        public string EntityTypeName { get; set; }
        /// <summary>
        /// Name of entity subtype
        /// </summary>
        public string EntitySubtypeName { get; set; }
        /// <summary>
        /// Id of an Entity
        /// </summary>
        public string EntID { get; set; }

        /// <summary>
        /// Form Guid
        /// </summary>
        public Guid FormGuid { get; set; }
    }

    public class NewEntityViewModel
    {
        [Required]
        public Guid EntityTypeId { get; set; }
        public Nullable<Guid> EntitySubTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public Nullable<Guid> ParentEntityId { get; set; }
        public int Status { get; set; }

        public List<Guid> DroppedVariablesList { get; set; }

        public Guid TenantId { get; set; }
    }

    public class EditEntityViewModel
    {
        [Required]
        public Guid EntityTypeId { get; set; }
        public Nullable<Guid> EntitySubTypeId { get; set; }
        [Required]
        public string Name { get; set; }
        public Nullable<Guid> ParentEntityId { get; set; }
        public int Status { get; set; }
        public System.Guid Guid { get; set; }

        public List<Guid> DroppedVariablesList { get; set; }
        public Guid TenantId { get; set; }
    }

    public class GetAllEntityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<EntityViewModel>()
          {
           new EntityViewModel
            {

                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Entity Name Example",
                EntID = "1234",
                EntityTypeId = Guid.NewGuid(),
                EntityTypeName = Core.Enum.EntityTypes.Person.ToString(),
                EntitySubTypeId = Guid.NewGuid(),
                EntitySubtypeName = "Medical Practitioner/Allied Health",

                ParentEntityId = null,
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = null,
                DeactivatedBy = null,
                DroppedVariablesList = null,
                EntityVariableList = null,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                Status = (int)Core.Enum.Status.Active,
                TenantId = Guid.NewGuid(),
            }
        };
        }
    }
    public class EntityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EntityViewModel
            {

                Id = 1,
                Guid = Guid.NewGuid(),
                Name = "Entity Name Example",
                EntID = "1234",
                EntityTypeId = Guid.NewGuid(),
                EntityTypeName = Core.Enum.EntityTypes.Person.ToString(),
                EntitySubTypeId = Guid.NewGuid(),
                EntitySubtypeName = "Medical Practitioner/Allied Health",

                ParentEntityId = null,
                CreatedBy =Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = null,
                DeactivatedBy = null,
                DroppedVariablesList = null,
                EntityVariableList = null,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                Status = (int)Core.Enum.Status.Active,
                TenantId = Guid.NewGuid(),
            };
        }
    }
}
