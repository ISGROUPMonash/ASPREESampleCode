//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aspree.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExternalLink
    {
        public int Id { get; set; }
        public System.Guid Guid { get; set; }
        public string Url { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime ExpiredDate { get; set; }
        public Nullable<System.Guid> UserGuid { get; set; }
    }
}