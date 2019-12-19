using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Core.ViewModels.MongoViewModels;
using MongoDB.Driver.Builders;

namespace Aspree.Provider.Provider
{
    public class ReportProvider : IReportProvider
    {
        private readonly MongoDBContext _mongoDBContext;
        private readonly AspreeEntities _dbContext;
        public ReportProvider(MongoDBContext mongoDBContext, AspreeEntities dbContext)
        {
            this._mongoDBContext = mongoDBContext;
            this._dbContext = dbContext;
        }

        public ReportViewModel GetAll(Guid loggedInUserId, string ProjectName, string ActivityName = null, string FormName = null)
        {
            IQueryable<FormDataEntryMongo> mongoEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable();

            #region filters
            if (!string.IsNullOrEmpty(ProjectName))
            {
                string projName = !string.IsNullOrEmpty(ProjectName) ? ProjectName.ToLower() : string.Empty;
                mongoEntities = mongoEntities.Where(x => projName.Equals(x.ProjectName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(ActivityName))
            {
                mongoEntities = mongoEntities.Where(x => ActivityName.ToLower().Equals(x.ActivityName, StringComparison.InvariantCultureIgnoreCase));
            }
            if (!string.IsNullOrEmpty(FormName))
            {
                mongoEntities = mongoEntities.Where(x => FormName.ToLower().Equals(x.FormTitle, StringComparison.InvariantCultureIgnoreCase));
            }
            #endregion

            var projectSQL = _dbContext.FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == ProjectName && x.Variable.VariableName == Core.Enum.DefaultsVariables.Name.ToString());
            var projectGuid = projectSQL != null ? projectSQL.FormDataEntry.Guid : Guid.Empty;
            var conditionProject = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectGuid);
            var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProject).SetSortOrder("ProjectDeployDate").OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

            #region roles
            var projectStatus = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectGuid);
            string loggedInUserRole = string.Empty;
            if (projectStatus != null)
            {
                var projectStaffMemberRoles = new LoginUserProvider().GetProjectStaffMembers(projectGuid);
                if (projectStaffMemberRoles != null)
                {
                    var projectRole = projectStaffMemberRoles.FirstOrDefault(x => x.StaffGuid == loggedInUserId);
                    var userInfo = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserId);
                    string role = string.Empty;
                    try { role = userInfo != null ? (userInfo.UserRoles.FirstOrDefault() != null ? userInfo.UserRoles.Select(x => x.Role.Name).FirstOrDefault() : string.Empty) : string.Empty; } catch (Exception exc) { Console.WriteLine(exc); }
                    loggedInUserRole = (projectRole != null ? projectRole.Role : role);
                }
            }
            #endregion

            ReportViewModel reportViewModel = new ReportViewModel();
            reportViewModel.metadata = new metadata();
            reportViewModel.metadata.next = string.Empty;
            reportViewModel.metadata.previous = string.Empty;

            reportViewModel.results = new List<results>();
            results res = new results();

            foreach (var item in mongoEntities)
            {
                try
                {
                    res = new results();

                    res.id = Convert.ToInt64(item.ParentEntityNumber == null ? item.EntityNumber : item.ParentEntityNumber);

                    if (res.id == 5236)
                    {

                    }

                    res.activities = new List<Report_ActivityReportViewModel>();
                    Report_ActivityReportViewModel activityReport = new Report_ActivityReportViewModel();

                    var activityGroup = mongoEntities.Where(x => x.EntityNumber == res.id || x.ParentEntityNumber == res.id).AsQueryable();

                    foreach (var ResultEntity in activityGroup)
                    {
                        try
                        {
                            activityReport = new Report_ActivityReportViewModel();
                            activityReport.name = ResultEntity.ActivityName;
                            activityReport.id = ResultEntity.ActivityGuid;
                            activityReport.timestamp = ResultEntity.CreatedDate;
                            activityReport.createdById = ResultEntity.CreatedByGuid;

                            activityReport.forms = new List<Report_FormReportViewModel>();

                            List<Report_FormReportViewModel> formReportList = new List<Report_FormReportViewModel>();
                            Report_FormReportViewModel formReport = new Report_FormReportViewModel();

                            var formGroup = mongoEntities.Where(x => x.ActivityGuid == ResultEntity.ActivityGuid && (x.EntityNumber == res.id || x.ParentEntityNumber == res.id)).AsQueryable();

                            foreach (var form in formGroup)
                            {
                                var projectActivity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == form.ActivityGuid);
                                var projectform = projectActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == form.FormGuid);
                                formReport = new Report_FormReportViewModel();
                                formReport.id = form.FormGuid;
                                formReport.name = form.FormTitle;
                                formReport.submittedById = form.CreatedByGuid;
                                formReport.timestamp = form.CreatedDate;
                                formReport.version = form.ProjectVersion;
                                formReport.variables = new List<Report_VariableReportViewModel>();
                                formReport.variables = ToReport_VariableReportViewModel(form.formDataEntryVariableMongoList, projectform, loggedInUserRole);
                                formReportList.Add(formReport);
                            }
                            activityReport.forms = formReportList;
                            res.activities.Add(activityReport);
                        }
                        catch (Exception exc) { res = null; }
                    }

                    if (res != null)
                        reportViewModel.results.Add(res);
                }
                catch (Exception exce) { }
            }
            return reportViewModel;
        }
        public List<Report_ActivityReportViewModel> activities(FormDataEntryMongo frm)
        {
            Report_ActivityReportViewModel m = new Report_ActivityReportViewModel();
            List<Report_ActivityReportViewModel> md = new List<Report_ActivityReportViewModel>();
            md.Add(m);
            return md;
        }
        public List<Report_VariableReportViewModel> ToReport_VariableReportViewModel(List<FormDataEntryVariableMongo> variables, FormsMongo form, string loggedInUserRoleName)
        {
            List<Report_VariableReportViewModel> allVariables = new List<Core.ViewModels.Report_VariableReportViewModel>();
            Report_VariableReportViewModel variable = new Core.ViewModels.Report_VariableReportViewModel();

            variables.ForEach(result =>
            {
                var localVar = form.VariablesListMongo.FirstOrDefault(x => x.VariableGuid == result.VariableGuid);
                if (localVar != null)
                {
                    var canView = localVar.VariableRoleListMongo.FirstOrDefault(x => x.CanView == true && x.RoleName == loggedInUserRoleName);
                    if (canView != null)
                    {

                        #region selected value to Text
                        switch (localVar.VariableTypeName)
                        {
                            case "Dropdown":
                                int dropdownIndex = localVar.Values.IndexOf(result.SelectedValues);
                                result.SelectedValues = dropdownIndex >= 0 ? localVar.ValueDescription.ElementAt(dropdownIndex) : result.SelectedValues;
                                break;
                            case "Checkbox":
                                int checkboxIndex = localVar.Values.IndexOf(result.SelectedValues);
                                result.SelectedValues = checkboxIndex >= 0 ? localVar.ValueDescription.ElementAt(checkboxIndex) : result.SelectedValues;
                                break;
                            case "LKUP":
                                break;
                            default:
                                break;
                        }
                        #endregion
                        variable = new Core.ViewModels.Report_VariableReportViewModel();
                        variable.name = result.VariableName;
                        variable.value = result.SelectedValues;
                        variable.type = localVar.VariableTypeName;
                        allVariables.Add(variable);
                    }
                }
            });

            return allVariables;
        }
        public ReportViewModel ToModel(FormDataEntry entity)
        {
            throw new NotImplementedException();
        }
    }
}
