using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ChangePassword
    {
        /// <summary>
        /// Old password string for verification
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }

        /// <summary>
        /// New password string for change
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// String for confirm new password
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }


    public class ResetPassword
    {
        /// <summary>
        /// Guid for user
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

        /// <summary>
        /// Guid for Question
        /// </summary>
        [Required]
        public Guid QuestionGuid { get; set; }

        /// <summary>
        /// Answer for security question
        /// </summary>
        [Required]
        public string Answer { get; set; }

        /// <summary>
        /// New password string for change
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        //[RegularExpression(@".{10,}/, /[a-z]+/, /[0-9]+/, /[A-Z]+/, /[#?!@$%^&*-_]", ErrorMessage = "Characters are not allowed.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// String for confirm new password
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ChangePasswordExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ChangePassword()
            {
                Password = "New password example",
                OldPassword = "Old password example",
                ConfirmPassword = "Confirm password example",
            };
        }
    }

    public class ResetPasswordExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ResetPassword()
            {
                Answer = "Your answer example",
                Password = "Old password example",
                ConfirmPassword = "Confirm password example",
                Guid = Guid.NewGuid(),
                QuestionGuid = Guid.NewGuid(),
            };
        }
    }
}
