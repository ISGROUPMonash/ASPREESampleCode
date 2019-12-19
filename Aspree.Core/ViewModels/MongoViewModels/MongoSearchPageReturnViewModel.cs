using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels.MongoViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MongoSearchPageReturnViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Guid, string> _ViewMongoActivityList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<FormsMongo> _ViewMongoFormsList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string _ViewProjectName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid _ViewProjectGuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string _ViewProjectUserRole { get; set; }

        /// <summary>
        /// Recruitment Start Date of Project
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RecruitmentStartDate { get; set; }
        /// <summary>
        /// Recruitment End Date of Project
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RecruitmentEndDate { get; set; }
        public bool? ProjectEthicsApproval { get; set; }

    }
}
