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
    public class EntitySubTypeViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of Entity Subtype
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Guid of Entity Type
        /// </summary>
        [Required]
        public Guid EntityTypeId { get; set; }
        /// <summary>
        /// Guid of Entity Subtype
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Name of Entity Type
        /// </summary>
        public string EntityTypeName { get; set; }
    }

    public class NewEntitySubTypeViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid EntityTypeId { get; set; }
    }

    public class EditEntitySubTypeViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public Guid EntityTypeId { get; set; }
        //[Required]
        //public System.Guid Guid { get; set; }
    }


    public class GetAllEntitySubTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<EntitySubTypeViewModel>()
            {
                new EntitySubTypeViewModel
                {
                    Id = 1,
                    Guid = Guid.NewGuid(),
                    EntityTypeId = Guid.NewGuid(),
                    EntityTypeName = Core.Enum.EntityTypes.Person.ToString(),
                    Name = "Medical Practitioner/Allied Health",
                }
            };
        }
    }

    public class EntitySubTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
             return new EntitySubTypeViewModel
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                EntityTypeId = Guid.NewGuid(),
                EntityTypeName = Core.Enum.EntityTypes.Person.ToString(),
                Name = "Medical Practitioner/Allied Health",
            };
           
        }
    }

    public class NewEntitySubTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewEntitySubTypeViewModel()
            {
                Name = "Example EntitySubType",
                EntityTypeId = Guid.NewGuid(),
            };
        }
    }

    public class EditEntitySubTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditEntitySubTypeViewModel()
            {
                Name = "Example EntitySubType",
                EntityTypeId = Guid.NewGuid(),
            };
        }
    }
}
