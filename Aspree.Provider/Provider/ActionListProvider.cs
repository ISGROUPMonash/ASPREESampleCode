using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data.MongoDB;
using Aspree.Core.ViewModels.MongoViewModels;
using MongoDB.Driver.Builders;
using Aspree.Data;
using Aspree.Core.Enum;
using MongoDB.Driver;

namespace Aspree.Provider.Provider
{
    public class ActionListProvider : IActionListProvider
    {
        private readonly MongoDBContext _mongoDBContext;
        private readonly AspreeEntities _dbContext;
        public ActionListProvider(MongoDBContext mongoDBContext, AspreeEntities dbContext)
        {
            this._mongoDBContext = mongoDBContext;
            this._dbContext = dbContext;
        }

        public IQueryable<ActionListViewModel> GetAll(ActionListSearchParameters searchModel)
        {
            List<ActionListViewModel> resultList = new List<ActionListViewModel>();
            IQueryable<ActionListViewModel> query = resultList.AsQueryable();

            string roleName = string.Empty;

            bool isSystemAdmin = false;
            bool isProjectAdmin = false;

            ProjectStaffMemberRole projectStaffMemberRole = _dbContext.ProjectStaffMemberRoles.Where(x => x.UserLogin.Guid == searchModel.LoggedInUserGuid && x.FormDataEntry.Guid == searchModel.projectId).OrderByDescending(c => c.Id).FirstOrDefault();
            if (projectStaffMemberRole != null)
            {
                roleName = projectStaffMemberRole.Role.Name;
                if (projectStaffMemberRole.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    isSystemAdmin = true;
                }
                if (projectStaffMemberRole.Role.Name == RoleTypes.Project_Admin.ToString().Replace("_", " "))
                {
                    isProjectAdmin = true;
                }
            }
            else
            {
                UserLogin loggedInUser = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == searchModel.LoggedInUserGuid);
                var roles = loggedInUser.UserRoles.Select(x => x.Role.Name).ToList();
                if (roles.Contains(Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")))
                {
                    isSystemAdmin = true;
                };
                roleName = roles.FirstOrDefault();
            }

            List<ProjectDeployViewModel> project = new List<ProjectDeployViewModel>();
            if (isSystemAdmin)
            {

                var result = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable().OrderByDescending(x => x.ProjectDeployDate)
                .GroupBy(g => g.ProjectGuid).Select(x => x.FirstOrDefault());

                project = result.ToList();

            }
            else
            {
                var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, searchModel.projectId);
                var result = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                project.Add(result);
            }
            resultList = ActionListOfProjects(project, searchModel.LoggedInUserGuid, isSystemAdmin, roleName);
            query = resultList.AsQueryable();

            if (isSystemAdmin)
            {
                query = query.Concat(GettAllDraftVariables(searchModel.LoggedInUserGuid));
            }

            int totleRecordCount = query != null ? query.Count() : 0;

            if (searchModel.Activity != null)
            {
                query = query.Where(x => x.ActivityName == searchModel.Activity).OrderByDescending(x => x.CreatedDate);
            }
            if (searchModel.EntityNumber != null)
            {
                query = query.Where(x => x.EntityNumber.Contains(searchModel.EntityNumber)).OrderByDescending(x => x.CreatedDate);
            }

            if (searchModel.EntityType != null)
            {
                searchModel.EntityType = searchModel.EntityType.ToLower();
                query = query.Where(x => x.EntityTypeName.ToLower().Contains(searchModel.EntityType)).OrderByDescending(x => x.CreatedDate);
            }
            if (searchModel.Form != null)
            {
                query = query.Where(x => x.FormName == searchModel.Form).OrderByDescending(x => x.CreatedDate);
            }
            if (searchModel.FormStatus != null)
            {
                searchModel.FormStatus = searchModel.FormStatus.ToLower();
                query = query.Where(x => x.FormStatusName.ToLower().Contains(searchModel.FormStatus)).OrderByDescending(x => x.CreatedDate);
            }
            query.ToList().ForEach(x => x.filteredCount = query.Count());
            query.ToList().ForEach(x => x.totleRecordCount = totleRecordCount);
            return query.OrderByDescending(x => x.CreatedDate).Skip(searchModel.Start).Take(searchModel.Length);
        }

        public List<ActionListViewModel> ActionListOfProjects(List<ProjectDeployViewModel> projectList, Guid loggedInUserGuid, bool isSystemAdmin, string roleName)
        {
            List<ActionListViewModel> resultList = new List<ActionListViewModel>();
            int projectCount = 0;
            if (projectList != null)
            {
                while (projectCount < projectList.Count())
                {
                    if (projectList[projectCount] != null)
                    {
                        List<SummaryPageActivityViewModel> summaryPageActivityForms = null;
                        string[] rolesArray = new string[]
                        {
                        RoleTypes.System_Admin.ToString().Replace("_"," ")
                        , RoleTypes.Project_Admin.ToString().Replace("_"," ")
                        , RoleTypes.Data_Entry_Supervisor.ToString().Replace("_"," ")
                        };

                        if (rolesArray.Contains(roleName))
                        {
                            summaryPageActivityForms = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().Where(x => x.ProjectGuid == projectList[projectCount].ProjectGuid && x.DateDeactivated == null).ToList();
                        }
                        else
                        {
                            var conditionUserRole = Query<SummaryPageActivityViewModel>.EQ(q => q.ActivityCompletedByGuid, loggedInUserGuid);
                            summaryPageActivityForms = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(conditionUserRole).AsQueryable().Where(x => x.ProjectGuid == projectList[projectCount].ProjectGuid && x.DateDeactivated == null).ToList();
                        }

                        IQueryable<FormDataEntryMongo> dataEntryEntity = null;
                        if (!isSystemAdmin)
                        {
                            IMongoQuery conditionDataEntryEntity = Query.And(Query<FormDataEntryMongo>.EQ(q => q.ProjectGuid, projectList[projectCount].ProjectGuid)
                                , Query<FormDataEntryMongo>.EQ(q => q.DateDeactivated, null));

                            dataEntryEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionDataEntryEntity).AsQueryable();

                        }

                        ActionListViewModel model = new ActionListViewModel();
                        int activityCount = 0;
                        if (summaryPageActivityForms != null)
                        {
                            while (activityCount < summaryPageActivityForms.Count())
                            {
                                ActivitiesMongo projectActivity = null;
                                if (summaryPageActivityForms[activityCount].ProjectVersion != projectList[projectCount].ProjectInternalVersion)
                                {
                                    var conditionOtherVersion = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectList[projectCount].ProjectGuid), Query<ProjectDeployViewModel>.EQ(r => r.ProjectInternalVersion, summaryPageActivityForms[activityCount].ProjectVersion));
                                    var projectOtherVersion = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionOtherVersion).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                                    if (projectOtherVersion != null)
                                    {
                                        projectActivity = projectOtherVersion.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid);
                                    }
                                    else
                                    {
                                        projectActivity = projectList[projectCount].ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid);
                                    }
                                }
                                else
                                {
                                    projectActivity = projectList[projectCount].ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid);
                                }
                                if (projectActivity == null)
                                {
                                    #region For older version created activity
                                    if (
                                                            summaryPageActivityForms[activityCount].ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                                                            || summaryPageActivityForms[activityCount].ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                                                            || summaryPageActivityForms[activityCount].ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                                                            || summaryPageActivityForms[activityCount].ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                                                            || summaryPageActivityForms[activityCount].ActivityName == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
                                    {
                                        projectActivity = projectList[projectCount].ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == summaryPageActivityForms[activityCount].ActivityName);
                                    }
                                    else
                                    {
                                        IMongoQuery condition = Query.And(Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectList[projectCount].ProjectGuid)
                                              , Query<ProjectDeployViewModel>.EQ(q => q.ProjectInternalVersion, summaryPageActivityForms[activityCount].ProjectVersion));

                                        ProjectDeployViewModel result = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                                        projectActivity = result.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid);
                                    }
                                    #endregion
                                }

                                #region Prevent end user to access Project Registration and Project Linkage
                                try
                                {
                                    if (!new List<string>() { RoleTypes.System_Admin.ToString().Replace("_", " "), RoleTypes.Project_Admin.ToString().Replace("_", " ") }.Contains(roleName))
                                    {
                                        if (projectActivity.ActivityEntityTypeName == EntityTypes.Project.ToString())
                                        {
                                            continue;
                                        }

                                        if (projectActivity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage)
                                            || projectActivity.ActivityName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                                        {
                                            continue;
                                        }
                                    }
                                }
                                catch (Exception exRole) { }
                                #endregion


                                int formCount = 0;
                                var formCollection = summaryPageActivityForms[activityCount].SummaryPageActivityFormsList.Where(x => x.FormStatusId != (int)Core.Enum.FormStatusTypes.Published).ToList();
                                if (formCollection != null)
                                {
                                    while (formCount < formCollection.Count())
                                    {
                                        #region ASPMONASH-395/398
                                        if (roleName == RoleTypes.Data_Entry_Supervisor.ToString().Replace("_", " ")
                                            || roleName == RoleTypes.Project_Admin.ToString().Replace("_", " "))
                                        {
                                            if (summaryPageActivityForms[activityCount].ActivityCompletedByGuid == loggedInUserGuid)
                                            {
                                            }
                                            else
                                            {
                                                if (formCollection[formCount].FormStatusId == (int)FormStatusTypes.Not_entered || formCollection[formCount].FormStatusId == (int)FormStatusTypes.Draft || formCollection[formCount].FormStatusId == 0)
                                                {
                                                    continue;
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Form view and edit permission restriction
                                        FormDataEntryMongo currentEntityDataEntry = null;
                                        if (dataEntryEntity != null)
                                        {
                                            if (formCollection[formCount].FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                                                  || formCollection[formCount].FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                                                  || formCollection[formCount].FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                                                  || formCollection[formCount].FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                                                  )
                                            {
                                                currentEntityDataEntry = dataEntryEntity.FirstOrDefault(c => c.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid && c.FormGuid == formCollection[formCount].FormGuid && c.EntityNumber == summaryPageActivityForms[activityCount].PersonEntityId);
                                            }
                                            else
                                            {
                                                currentEntityDataEntry = dataEntryEntity.FirstOrDefault(c => c.ActivityGuid == summaryPageActivityForms[activityCount].ActivityGuid && c.FormGuid == formCollection[formCount].FormGuid && c.ParentEntityNumber == summaryPageActivityForms[activityCount].PersonEntityId);
                                            }
                                        }




                                        int count = 0;
                                        int totalVariables = 0;
                                        int countEdit = 0;

                                        int remainingDataEntryCount = 0;
                                        try
                                        {

                                            FormsMongo frm = projectActivity.FormsListMongo.FirstOrDefault(x => x.FormGuid == formCollection[formCount].FormGuid);
                                            if (frm != null)
                                            {
                                                IQueryable<VariablesMongo> variables = frm.VariablesListMongo.AsQueryable();

                                                totalVariables = variables.Count();

                                                variables.ToList().ForEach(v =>
                                                {
                                                    var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && x.CanView == true).FirstOrDefault();//&& x.CanView==true
                                                    if (variable == null)
                                                    {
                                                        count++;
                                                    }
                                                });


                                                if (formCollection[formCount].FormStatusName == FormStatusTypes.Submitted_for_review.ToString()
                                                    && roleName != RoleTypes.System_Admin.ToString().Replace("_", " "))
                                                {
                                                    variables.ToList().ForEach(v =>
                                                    {
                                                        string[] skipVar = new string[] { "OtherText", "Heading" };
                                                        if (!skipVar.Contains(v.VariableName))
                                                        {
                                                            if (v.DependentVariableId != null)
                                                            {

                                                                bool chkDependentVariablePermission = variables.Where(x => x.VariableId == v.DependentVariableId).Any(x => x.VariableRoleListMongo.Any(c => c.RoleName == roleName && (c.CanEdit == true || c.CanCreate == true)));
                                                                if (chkDependentVariablePermission)
                                                                {
                                                                    var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && (x.CanEdit == true || x.CanCreate == true)).FirstOrDefault();
                                                                    if (variable == null)
                                                                    {
                                                                        countEdit++;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (currentEntityDataEntry != null)
                                                                        {

                                                                            FormDataEntryVariableMongo curntVarDataEntry = currentEntityDataEntry.formDataEntryVariableMongoList.Where(x => x.VariableId == v.VariableId).FirstOrDefault();
                                                                            if (curntVarDataEntry != null)
                                                                            {
                                                                                if (string.IsNullOrEmpty(curntVarDataEntry.SelectedValues))
                                                                                {
                                                                                    remainingDataEntryCount++;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var variable = v.VariableRoleListMongo.Where(x => x.RoleName == roleName && (x.CanEdit == true || x.CanCreate == true)).FirstOrDefault();
                                                                if (variable == null)
                                                                {

                                                                    countEdit++;
                                                                }
                                                                else
                                                                {
                                                                    if (currentEntityDataEntry != null)
                                                                    {
                                                                        FormDataEntryVariableMongo curntVarDataEntry = currentEntityDataEntry.formDataEntryVariableMongoList.Where(x => x.VariableId == v.VariableId).FirstOrDefault();
                                                                        if (curntVarDataEntry != null)
                                                                        {
                                                                            if (string.IsNullOrEmpty(curntVarDataEntry.SelectedValues))
                                                                            {
                                                                                remainingDataEntryCount++;
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                            if (count > 0)
                                            {
                                                if (totalVariables == count)
                                                    continue;
                                            }

                                            if (countEdit > 0 && remainingDataEntryCount == 0)
                                            {
                                                continue;
                                            }
                                        }
                                        catch (Exception exc) { }
                                        #endregion

                                        model = new ActionListViewModel();

                                        model.ActivityGuid = summaryPageActivityForms[activityCount].ActivityGuid;
                                        model.ActivityId = summaryPageActivityForms[activityCount].ActivityId;
                                        model.ActivityName = summaryPageActivityForms[activityCount].ActivityName;

                                        model.EntityNumber = summaryPageActivityForms[activityCount].PersonEntityId.ToString("D7");

                                        model.EntityTypeGuid = Guid.Empty;
                                        model.EntityTypeId = 0;
                                        model.EntityTypeName = projectActivity != null ? projectActivity.ActivityEntityTypeName : string.Empty;

                                        model.FormGuid = formCollection[formCount].FormGuid;
                                        model.FormId = formCollection[formCount].FormId;
                                        model.FormName = formCollection[formCount].FormTitle;
                                        model.FormStatusId = formCollection[formCount].FormStatusId;

                                        model.FormStatusName = formCollection[formCount].FormStatusId == (int)Core.Enum.FormStatusTypes.Submitted_for_review ? "For Review" :
                                            formCollection[formCount].FormStatusId == (int)Core.Enum.FormStatusTypes.Not_entered ? "Not Entered" :
                                            formCollection[formCount].FormStatusId == (int)Core.Enum.FormStatusTypes.Draft ? formCollection[formCount].FormStatusName :
                                            formCollection[formCount].FormStatusName;

                                        model.CreatedDate = summaryPageActivityForms[activityCount].CreatedDate;

                                        model.SummaryPageActivityId = Convert.ToString(summaryPageActivityForms[activityCount].Id);
                                        model.EntityProjectGuid = summaryPageActivityForms[activityCount].ProjectGuid;
                                        model.ProjectVersion = summaryPageActivityForms[activityCount].ProjectVersion;
                                        resultList.Add(model);

                                        formCount++;
                                    }
                                }
                                activityCount++;
                            }
                        }
                    }
                    projectCount++;
                }
            }
            return resultList;
        }

        public int CountAllActionListRecord(Guid projectId, Guid loggedInUserGuid)
        {
            #region new-code
            List<ActionListViewModel> resultList = new List<ActionListViewModel>();
            IQueryable<ActionListViewModel> query = resultList.AsQueryable();
            bool isSystemAdmin = false;
            bool isProjectAdmin = false;

            string roleName = string.Empty;

            ProjectStaffMemberRole projectStaffMemberRole = _dbContext.ProjectStaffMemberRoles.Where(x => x.UserLogin.Guid == loggedInUserGuid && x.FormDataEntry.Guid == projectId).OrderByDescending(c => c.Id).FirstOrDefault();
            if (projectStaffMemberRole != null)
            {
                roleName = projectStaffMemberRole.Role.Name;
                if (projectStaffMemberRole.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    isSystemAdmin = true;
                }
                if (projectStaffMemberRole.Role.Name == RoleTypes.Project_Admin.ToString().Replace("_", " "))
                {
                    isProjectAdmin = true;
                }
            }
            else
            {
                UserLogin loggedInUser = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserGuid);
                var roles = loggedInUser.UserRoles.Select(x => x.Role.Name).ToList();
                if (roles.Contains(Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")))
                {
                    isSystemAdmin = true;
                };

                roleName = roles.FirstOrDefault();
            }
            List<ProjectDeployViewModel> project = new List<ProjectDeployViewModel>();
            if (isSystemAdmin)
            {
                var result = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").FindAll().AsQueryable().OrderByDescending(x => x.ProjectDeployDate)
                                .GroupBy(g => g.ProjectGuid).Select(x => x.FirstOrDefault());
                project = result.ToList();
            }
            else
            {
                var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var result = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                project.Add(result);
            }
            resultList = ActionListOfProjects(project, loggedInUserGuid, isSystemAdmin, roleName);
            query = resultList.AsQueryable();
            if (isSystemAdmin)
            {
                query = query.Concat(GettAllDraftVariables(loggedInUserGuid));
            }
            return query.Count();
            #endregion
        }

        public IQueryable<ActionListViewModel> GetAllActionListActivities(Guid projectId, Guid loggedInUserGuid)
        {
            #region role check
            bool isSystemAdmin = false;
            bool isProjectAdmin = false;
            ProjectStaffMemberRole projectStaffMemberRole = _dbContext.ProjectStaffMemberRoles.Where(x => x.UserLogin.Guid == loggedInUserGuid && x.FormDataEntry.Guid == projectId).OrderByDescending(c => c.Id).FirstOrDefault();
            if (projectStaffMemberRole != null)
            {
                if (projectStaffMemberRole.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " "))
                {
                    isSystemAdmin = true;
                }
                if (projectStaffMemberRole.Role.Name == RoleTypes.Project_Admin.ToString().Replace("_", " "))
                {
                    isProjectAdmin = true;
                }
            }
            else
            {
                UserLogin loggedInUser = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUserGuid);
                var roles = loggedInUser.UserRoles.Select(x => x.Role.Name).ToList();
                if (roles.Contains(Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")))
                {
                    isSystemAdmin = true;
                };
            }
            #endregion

            List<SummaryPageActivityViewModel> summaryPageActivityForms = null;

            if (isSystemAdmin)
            {
                summaryPageActivityForms = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().Where(x => x.DateDeactivated == null).ToList();
            }
            else
            {
                summaryPageActivityForms = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindAll().AsQueryable().Where(x => x.ProjectGuid == projectId && x.DateDeactivated == null).ToList();
            }
                IQueryable<SummaryPageActivityViewModel> summaryPageActivityFormsSQL = _dbContext.Activities.Where(x => x.FormDataEntry.Guid == projectId && x.IsDefaultActivity == (int)DefaultActivityType.Default).Select(v => new SummaryPageActivityViewModel()
                {
                    ActivityGuid = v.Guid,
                    ActivityId = v.Id,
                    ActivityName = v.ActivityName,
                    SummaryPageActivityFormsList = v.ActivityForms.Select(fr => new SummaryPageActivityForms()
                    {
                        FormGuid = fr.Form.Guid,
                        FormId = fr.FormId,
                        FormTitle = fr.Form.FormTitle,
                    }).ToList(),
                });

                var lst1 = summaryPageActivityForms.ToList();
                var lst2 = summaryPageActivityFormsSQL.ToList();

                summaryPageActivityForms = lst1.Concat(lst2).ToList();

            List<ActionListViewModel> resultList = new List<ActionListViewModel>();
            ActionListViewModel model = new ActionListViewModel();
            int activityCount = 0;
            if (summaryPageActivityForms != null)
            {
                while (activityCount < summaryPageActivityForms.Count())
                {
                    var frmList = summaryPageActivityForms[activityCount].SummaryPageActivityFormsList.Where(x => x.FormStatusId != (int)Core.Enum.FormStatusTypes.Published).ToList();
                    int formCount = 0;
                    if (frmList != null)
                    {
                        while (formCount < frmList.Count())
                        {
                            model = new ActionListViewModel();
                            model.ActivityGuid = summaryPageActivityForms[activityCount].ActivityGuid;
                            model.ActivityId = summaryPageActivityForms[activityCount].ActivityId;
                            model.ActivityName = summaryPageActivityForms[activityCount].ActivityName;
                            model.EntityNumber = summaryPageActivityForms[activityCount].PersonEntityId.ToString("D7");
                            model.EntityTypeGuid = Guid.Empty;
                            model.EntityTypeId = 0;
                            model.EntityTypeName = string.Empty;
                            model.FormGuid = frmList[formCount].FormGuid;
                            model.FormId = frmList[formCount].FormId;
                            model.FormName = frmList[formCount].FormTitle;
                            model.FormStatusId = frmList[formCount].FormStatusId;
                            model.FormStatusName = frmList[formCount].FormStatusId == (int)Core.Enum.FormStatusTypes.Submitted_for_review ? "For Review" : "Draft";
                            model.CreatedDate = summaryPageActivityForms[activityCount].CreatedDate;
                            resultList.Add(model);
                            formCount++;
                        }
                    }
                    activityCount++;
                }
            }
            IQueryable<ActionListViewModel> query = resultList.AsQueryable();
            if (isSystemAdmin)
            {
                query = query.Concat(GettAllDraftVariables(loggedInUserGuid));
            }
            return query.OrderByDescending(x => x.ActivityName).ThenBy(x => x.FormName);
        }
        public IQueryable<ActionListViewModel> GettAllDraftVariables(Guid loggedInUserGuid)
        {
            List<ActionListViewModel> resultList = new List<ActionListViewModel>();
            IQueryable<Variable> allDraftVariables = _dbContext.Variables.Where(x => x.IsApproved == false);
            ActionListViewModel model = new ActionListViewModel();
            allDraftVariables.ToList().ForEach(v =>
            {
                model = new ActionListViewModel();
                model.ActivityGuid = Guid.Empty;
                model.ActivityId = v.Id;
                model.ActivityName = "Variable";
                model.EntityNumber = v.Id.ToString();
                model.EntityTypeGuid = Guid.Empty;
                model.EntityTypeId = (int)Core.Enum.EntityTypes.Project;
                model.EntityTypeName = Core.Enum.EntityTypes.Project.ToString();
                model.FormGuid = v.Guid;
                model.FormId = v.Id;
                model.FormName = v.VariableName;
                model.FormStatusId = !v.IsApproved ? (int)Core.Enum.FormStatusTypes.Submitted_for_review : (int)Core.Enum.FormStatusTypes.Published;
                model.FormStatusName = !v.IsApproved ? "For Review" : "Draft";
                model.CreatedDate = v.CreatedDate;
                resultList.Add(model);
            });
            return resultList.AsQueryable();
        }
    }
}