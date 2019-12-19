using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using System.Globalization;

namespace Aspree.Provider.Provider
{
    public class DashboardProvider : IDashboardProvider
    {
        private readonly AspreeEntities _dbContext;
        public DashboardProvider(AspreeEntities dbContext)
        {
            this._dbContext = dbContext;

        }
        public DashboardStatus GetDashboardStatus(DashboardFilter filter)
        {
            string[] formats = { "MM-dd-yyyy", "dd-MM-yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy",  "yyyy/MM/dd" };
            var start = DateTime.ParseExact(filter.Start, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None);
            var end = DateTime.ParseExact(filter.End, formats, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None).AddDays(1).AddSeconds(-1);
            if (end < start) {
               throw new Core.AlreadyExistsException("End date can not be less then start date.");
            }
            return new DashboardStatus {
                ActiveRoles = this._dbContext.Roles.Count(r => !r.DateDeactivated.HasValue && r.CreatedDate >= start && r.CreatedDate <= end),
                ActiveUser = this._dbContext.UserLogins.Count(r => !r.DateDeactivated.HasValue && r.CreatedDate >= start && r.CreatedDate <= end),
                RoleCount = this._dbContext.Roles.Count(r => r.CreatedDate >= start && r.CreatedDate <= end),
                UserCount = this._dbContext.UserLogins.Count(r => r.CreatedDate >= start && r.CreatedDate <= end)
            };
        }
    }
}