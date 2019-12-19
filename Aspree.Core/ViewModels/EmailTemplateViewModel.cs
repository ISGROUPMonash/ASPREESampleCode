using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aspree.Core.ViewModels
{
    public class EmailTemplateViewModel
    {
        public int Id { get; set; }
        public Guid PushEmailEventID { get; set; }
        [Display(Name = "Subject Content")]
        [Required]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Template Content")]
        [AllowHtml]
        public string MailBody { get; set; }
        //[RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "Please select the event type.")]
        //public int NotificationEventId { get; set; }
        [Display(Name = "Keywords")]
        public string EMailKeywords { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
     
        public Nullable<Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
       
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        public System.DateTime DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        //public PushEmailEventViewModel NotificationEventTypeById { get; set; }
        public List<PushEmailEventViewModel> EventList { get; set; }
        //[Display(Name = "Event Types")]
        public string EventName { get; set; }
        
        

    }



    public class EmailTemplateSearchViewModel
    {
        public int PushEmailEventID { get; set; }
        [Display(Name = "Template Content")]
        public string MailBody { get; set; }
        //[RegularExpression(@"^[1-9]+[0-9]*$", ErrorMessage = "Please select the event type.")]
        public byte NotificationEventId { get; set; }
        [Display(Name = "Keywords")]
        public string EMailKeywords { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Subject Content")]
        public string Subject { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public System.DateTime DateDeactivated { get; set; }
        public PushEmailEventViewModel NotificationEventTypeById { get; set; }
        public List<PushEmailEventViewModel> NotificationEventTypes { get; set; }
        [Display(Name = "Event Types")]
        public string EventName { get; set; }

        public int TotalRecords { get; set; }

    }

    public class EditEmailTemplateViewModel
    {
        [Required]
        public System.Guid Guid { get; set; }
        public string MailBody { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }

        public System.DateTime ModifiedDate { get; set; }
  

    }

   


}


