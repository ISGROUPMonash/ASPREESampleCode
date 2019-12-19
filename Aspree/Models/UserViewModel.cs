using Aspree.Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Models
{
    public class UserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Address { get; set; }
        public Int64 UserId { get; set; }
        public Guid UserGuid { get; set; }
        public List<SelectListItem> Role { get; set; }
        [Required]

        public Guid RoleId { get; set; }

        public Guid TenantId { get; set; }

        public Guid QuestionId { get; set; }
        public string Answer { get; set; }

        //[MaximumFileSizeValidator(1)]
        //[ValidFileTypeValidator(new string[] { "png", "jpg" })]
        //[ImageDimensionValidator(maximumHeight:200, maximumWidth:200)]
        //public HttpPostedFileBase FileProfile { get; set; }
        public string Profile { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public bool IsUserApprovedBySystemAdmin { get; set; }
    }
}