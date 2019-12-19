using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ForgotPasswordViewModel
    {
        public string Username { get; set; }
    }
    public class CheckSecurityQuestionAnswerViewModels
    {
        /// <summary>
        /// Guid of User
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Guid of Security Question
        /// </summary>
        public Guid QuestionGuid { get; set; }
        /// <summary>
        /// Answer of security question
        /// </summary>
        public string Answer { get; set; }
    }

    public class ForgotPasswordViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
           return  new ForgotPasswordViewModel
            {
               Username="Example User Name"
            };
          
        }
    }

    public class CheckSecurityQuestionAnswerViewModelsExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new CheckSecurityQuestionAnswerViewModels
            {
                UserId=Guid.NewGuid(),
                QuestionGuid = Guid.NewGuid(),
                Answer ="Example Answer"
            };

        }
    }

}
