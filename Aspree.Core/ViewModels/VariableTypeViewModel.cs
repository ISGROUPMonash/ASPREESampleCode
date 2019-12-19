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
    public class VariableTypeViewModel 
    {
        public VariableTypeViewModel()
        {
            this.Status = 1;
        }
        /// <summary>
        /// Id of Variable
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Type of variable
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Status of variable
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Guid of variable
        /// </summary>
        public System.Guid Guid { get; set; }
    }

    public class NewVariableType 
    {
        [Required]
         public string Type { get; set; }
    }

    public class EditVariableType
    {
        //[Required]
        //public Guid Guid { get; set; }

        [Required]
        public string Type { get; set; }
    }

    public class GetAllVariableTypeViewModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<VariableTypeViewModel>()
            {
                new VariableTypeViewModel
                {
                    Id = 1,
                   Type = "Text Box",
                  Status = 1,
                  Guid = Guid.NewGuid(),
                }
            };
        }
 }
    public class VariableTypeViewModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new VariableTypeViewModel()
            {
                Id = 1,
                Type = "Text Box",
                Status = 1,
                Guid = Guid.NewGuid(),
            };
        }    
    }
    public class NewVariableTypeExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewVariableType()
            {
                Type = "Text Box",                
            };

        }
    }

    public class EditVariableTypeExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditVariableType()
            {

                Type = "Text Box",

            };

        }
    }
}


