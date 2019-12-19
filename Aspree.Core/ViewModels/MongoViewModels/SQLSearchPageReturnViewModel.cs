using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels.MongoViewModels
{
 public   class SQLSearchPageReturnViewModel
    {
        public List<Core.ViewModels.FormViewModel> _ViewSQLFormsList { get; set; }
        public string _ViewProjectName { get; set; }
        public string _ViewProjectUserRole { get; set; }
        public Guid _ViewProjectGuid { get; set; }


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


    }
}
