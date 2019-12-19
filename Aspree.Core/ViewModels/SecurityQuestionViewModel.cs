using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SecurityQuestionViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        /// <summary>
        /// Security question
        /// </summary>
        [Required]
        public string Question { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
    }


    public class NewSecurityQuestionViewModel
    {
        /// <summary>
        /// Security question
        /// </summary>
        [Required]
        public string Question { get; set; }
    }

    public class EditSecurityQuestionViewModel
    {
        /// <summary>
        /// Security question
        /// </summary>
        [Required]
        public string Question { get; set; }
        //[Required]
        //public System.Guid Guid { get; set; }
    }

    public class UpdateUserSecurityQuestion
    {
        /// <summary>
        /// Guid for Security Question
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

        /// <summary>
        /// Guid for user
        /// </summary>
        [Required]
        public Guid UserGuid { get; set; }

        /// <summary>
        /// Answer of Security Question
        /// </summary>
        [Required]
        public string Answer { get; set; }
    }

    public class UpdateUserSecurityQuestionExample : Swashbuckle.Examples.IExamplesProvider
    {
        public object GetExamples()
        {
            return new UpdateUserSecurityQuestion
            {
                Answer = "Example Answer",
                Guid = Guid.NewGuid(),
                UserGuid = Guid.NewGuid(),
            };
        }
    }
    public class GetAllSecurityQuestionViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel
                {
                    Question = "Example Question",
                    Guid = Guid.NewGuid(),

                }
                

            };

        }
    }

    public class SecurityQuestionViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
          return  new SecurityQuestionViewModel
            {
               Question="Example Question",
                Guid = Guid.NewGuid(),
                
            };
           
        }
    }

    public class NewSecurityQuestionViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewSecurityQuestionViewModel
            {
                Question = "Example Question",
            };
        }
    }

    public class EditSecurityQuestionViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditSecurityQuestionViewModel
            {
                Question = "Example Question",
            };
        }
    }
}
