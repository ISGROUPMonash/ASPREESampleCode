using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SiteViewModel
    {
        [IgnoreDataMember]

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid CreatedBy { get; set; }
        public Nullable<Guid> CountyId { get; set; }
        public Nullable<Guid> StateId { get; set; }
        public Nullable<Guid> CityId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public string GPSLocations { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<Guid> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
    }


    public class NewSiteViewModel
    {
        [Required]
        public string Name { get; set; }
        public Nullable<Guid> CountyId { get; set; }
        public Nullable<Guid> StateId { get; set; }
        public Nullable<Guid> CityId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public string GPSLocations { get; set; }
    }

    public class EditSiteViewModel
    {
        [Required]
        public string Name { get; set; }
        public Nullable<Guid> CountyId { get; set; }
        public Nullable<Guid> StateId { get; set; }
        public Nullable<Guid> CityId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Suburb { get; set; }
        public string PostCode { get; set; }
        public string GPSLocations { get; set; }
        //public System.Guid Guid { get; set; }
    }
}
