using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Core.Enum;
using MongoDB.Driver.Builders;
using Aspree.Core.ViewModels.MongoViewModels;

namespace Aspree.Provider.Provider
{
    public class ProjectProvider : IProjectProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IPrivilegeProvider _privilegeProvider;
        private readonly MongoDBContext _mongoDBContext;
        public ProjectProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, IPrivilegeProvider privilegeProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._privilegeProvider = privilegeProvider;
            this._mongoDBContext = mongoDBContext;
        }

        public ProjectViewModel Create(ProjectViewModel model)
        {
            if (dbContext.Projects.Any(et => et.ProjectName.ToLower() == model.ProjectName.ToLower()))
            {
                throw new Core.AlreadyExistsException("Project already exists.");
            }
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var tenant = dbContext.Tenants.FirstOrDefault(t => t.Guid == model.TenantId);
            var projectuser = dbContext.UserLogins.FirstOrDefault(p => p.Guid == model.ProjectUserId);
            var project = new Project()
            {
                Guid = Guid.NewGuid(),
                ProjectName = model.ProjectName,
                CheckListID = model.CheckListID,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                EndDate = model.EndDate,
                PreviousProjectId = model.PreviousProjectId,
                ProjectStatusId = model.ProjectStatusId,
                ProjectUrl = model.ProjectUrl,
                StartDate = model.StartDate,
                State = model.State,
                Version = model.Version,
                TenantId = tenant != null ? tenant.Id : 0
            };

            dbContext.Projects.Add(project);
            foreach (var projUser in model.ProjectStaffMembersRoles)
            {
                var user = dbContext.UserLogins.FirstOrDefault(v => v.Guid == projUser.UserGuid);
                var role = dbContext.Roles.FirstOrDefault(v => v.Guid == projUser.RoleGuid);
                var projectstaffRoles = new ProjectStaffMemberRole()
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = project.Id,
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedBy = createdBy.Id,
                    StaffCreatedDate = DateTime.UtcNow,
                };
                dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);
            }
            SaveChanges();

            _privilegeProvider.CreateDefaultFormsForProject(project.Id, createdBy.Id, tenant != null ? tenant.Id : 0);
            _privilegeProvider.CreateDefaultActivitiesForProject(project.Id, createdBy.Id, tenant != null ? tenant.Id : 0);

            return ToModel(project);
        }

        public ProjectViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var project = dbContext.Projects.FirstOrDefault(fs => fs.Guid == guid);
            if (project != null)
            {
                project.DeactivatedBy = deactivatedBy.Id;
                project.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(project);
            }

            return null;
        }

        public ProjectViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var project = dbContext.Projects.FirstOrDefault(fs => fs.Id == id);
            if (project != null)
            {
                project.DeactivatedBy = deactivatedBy.Id;
                project.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(project);
            }

            return null;
        }

        public IEnumerable<ProjectViewModel> GetAll()
        {
            return dbContext.Projects.Where(x => x.DateDeactivated == null)
               .Select(ToModel)
               .ToList();
        }

        public IEnumerable<ProjectViewModel> GetAll(Guid tenantId)
        {
            return dbContext.Projects
               .Where(p => p.TenantId.HasValue && p.Tenant.Guid == tenantId && p.DateDeactivated == null)
               .Select(ToModel)
               .ToList();
        }

        public ProjectViewModel GetByGuid(Guid guid)
        {
            var project = dbContext.Projects
                .FirstOrDefault(fs => fs.Guid == guid);


            if (project != null)
                return ToModel(project);

            return null;
        }

        public ProjectViewModel GetById(int id)
        {
            var project = dbContext.Projects
               .FirstOrDefault(fs => fs.Id == id);

            if (project != null)
                return ToModel(project);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public ProjectViewModel ToModel(Project entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            return new ProjectViewModel()
            {
                Guid = entity.Guid,
                ProjectName = entity.ProjectName,
                CheckListID = entity.CheckListID,
                CreatedBy = createdBy.Guid,
                CreatedDate = DateTime.UtcNow,
                EndDate = entity.EndDate,
                PreviousProjectId = entity.PreviousProjectId,
                ProjectStatusId = entity.ProjectStatusId,
                ProjectUrl = entity.ProjectUrl,
                StartDate = entity.StartDate,
                State = entity.State,
                Version = entity.Version,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                DateDeactivated = entity.DateDeactivated,
                Id = entity.Id,
                ProjectUsers = entity.ProjectStaffMembers.Select(u => _userLoginProvider.ToModel(u.UserLogin)).ToList(),
            };
        }

        public ProjectViewModel Update(ProjectViewModel model)
        {
            if (dbContext.Projects.Any(et => et.ProjectName.ToLower() == model.ProjectName.ToLower()
            && et.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Project already exists.");
            }

            var project = dbContext.Projects
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            var projectuser = dbContext.UserLogins.FirstOrDefault(p => p.Guid == model.ProjectUserId);

            if (project != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);

                project.ProjectName = model.ProjectName;
                project.CheckListID = model.CheckListID;
                project.ModifiedBy = modifiedBy.Id;
                project.ModifiedDate = DateTime.UtcNow;
                project.EndDate = model.EndDate;
                project.PreviousProjectId = model.PreviousProjectId;
                project.ProjectUrl = model.ProjectUrl;
                project.StartDate = model.StartDate;
                IEnumerable<ProjectStaffMemberRole> list = dbContext.ProjectStaffMemberRoles.Where(x => x.ProjectId == project.Id).ToList();
                // Use Remove Range function to delete all records at once
                dbContext.ProjectStaffMemberRoles.RemoveRange(list);
                foreach (var projUser in model.ProjectStaffMembersRoles)
                {
                    var user = dbContext.UserLogins.FirstOrDefault(v => v.Guid == projUser.UserGuid);
                    var role = dbContext.Roles.FirstOrDefault(v => v.Guid == projUser.RoleGuid);
                    var projectstaffRoles = new ProjectStaffMemberRole()
                    {
                        Guid = Guid.NewGuid(),
                        ProjectId = project.Id,
                        UserId = user.Id,
                        RoleId = role.Id,
                        CreatedBy = modifiedBy.Id,
                        StaffCreatedDate = projUser.StaffCreatedDate == null ? DateTime.UtcNow : projUser.StaffCreatedDate,
                    };
                    dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);
                }
                SaveChanges();
                return ToModel(project);
            }

            return null;
        }

        public IEnumerable<ProjectViewModel> GetAllProjectUserByRole(Guid projectGuid, Guid roleGuid)
        {
            var project = dbContext.Projects.Where(c => c.Guid == projectGuid).Select(ToModel).ToList();
            return project;
        }

        public ProjectViewModel PublishProject(ProjectViewModel model)
        {
            if (dbContext.Projects.Any(et => et.Guid == model.Guid && et.ProjectStatusId == (int)Core.Enum.ProjectStatusTypes.Published))
            {
                throw new Core.AlreadyExistsException("Project already published.");
            }
            var project = dbContext.Projects
              .FirstOrDefault(fs => fs.Guid == model.Guid);
            if (project != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                project.ProjectStatusId = model.ProjectStatusId;
                project.State = model.State;
                project.ModifiedBy = modifiedBy.Id;
                project.ModifiedDate = DateTime.UtcNow;
                project.ProjectUrl = model.ProjectUrl;
            }
            return ToModel(project);
        }

        public FormDataEntryProjectsViewModel GetProjectByGuid_New(Guid guid)
        {
            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == guid);
            if (project != null)
                return ToProjectModel(project);
            return null;
        }
        public FormDataEntryProjectsViewModel ToProjectModel(FormDataEntry entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var formDataEntryVariables = dbContext.FormDataEntryVariables.Where(et => et.FormDataEntryId == entity.Id);

            string Name = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();

            string ProjectSubtype_model = string.Empty;
            var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
            {
                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ProSType");
                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                for (int i = 0; i < variableValues.Count; i++)
                {
                    if (variableValues[i] == ProjectSubtype)
                    {
                        ProjectSubtype_model = variableValuesDesc[i];
                    }
                }
            }
            string ConfData_model = string.Empty;
            var ConfData = formDataEntryVariables.Where(x => x.Variable.VariableName == "ConfData").Select(x => x.SelectedValues).FirstOrDefault();
            {
                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ConfData");
                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                for (int i = 0; i < variableValues.Count; i++)
                {
                    if (variableValues[i] == ConfData)
                    {
                        ConfData_model = variableValuesDesc[i];
                    }
                }
            }

            string CnstModel_model = string.Empty;
            var CnstModel = formDataEntryVariables.Where(x => x.Variable.VariableName == "CnstModel").Select(x => x.SelectedValues).FirstOrDefault();
            {
                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "CnstModel");
                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                for (int i = 0; i < variableValues.Count; i++)
                {
                    if (variableValues[i] == CnstModel)
                    {
                        CnstModel_model = variableValuesDesc[i];
                    }
                }
            }
            string Ethics_model = string.Empty;
            var Ethics = formDataEntryVariables.Where(x => x.Variable.VariableName == "Ethics").Select(x => x.SelectedValues).FirstOrDefault();
            {
                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "Ethics");
                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                for (int i = 0; i < variableValues.Count; i++)
                {
                    if (variableValues[i] == Ethics)
                    {
                        Ethics_model = variableValuesDesc[i];
                    }
                }
            }
            string DataStore_model = string.Empty;
            var DataStore = formDataEntryVariables.Where(x => x.Variable.VariableName == "DataStore").Select(x => x.SelectedValues).FirstOrDefault();
            {
                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "DataStore");
                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                for (int i = 0; i < variableValues.Count; i++)
                {
                    if (variableValues[i] == DataStore)
                    {
                        DataStore_model = variableValuesDesc[i];
                    }
                }
            }
            string ProDt_model = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProDt").Select(x => x.SelectedValues).FirstOrDefault();

            DateTime? RecruitmentStartDate = null;
            DateTime? RecruitmentEndDate = null;
            var recStartDate = entity.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
            var recEndDate = entity.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());
            try { RecruitmentStartDate = recStartDate != null ? Convert.ToDateTime(recStartDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }
            try { RecruitmentEndDate = recEndDate != null ? Convert.ToDateTime(recEndDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }

            return new FormDataEntryProjectsViewModel()
            {
                Guid = entity.Guid,
                ProjectName = Name,
                CreatedBy = createdBy.Guid,
                CreatedDate = DateTime.UtcNow,
                Id = entity.Id,
                ProjectStaffMembersRoles = entity.ProjectStaffMemberRoles.Select(c => new NewProjectStaffMemberRoleViewModel()
                {
                    Id = c.Id,
                    ProjectGuid = c.FormDataEntry.Guid,
                    UserGuid = c.UserLogin != null ? c.UserLogin.Guid : Guid.Empty,
                    RoleGuid = c.Role.Guid,
                    ProjectUserName = c.UserLogin != null ? c.UserLogin.FirstName + " " + c.UserLogin.LastName : "",
                    ProjectUserRoleName = c.Role.Name,
                    StaffCreatedDate = c.StaffCreatedDate,
                    StaffCreatedDateString = c.StaffCreatedDate != null ? c.StaffCreatedDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : "",
                    Guid = c.Guid,
                }).ToList(),
                RecruitmentStartDate= RecruitmentStartDate,
                RecruitmentEndDate=RecruitmentEndDate,
            };
        }

        public IEnumerable<ProjectViewModel> GetAllProjectByUserId(Guid userId)
        {
            var user = dbContext.UserLogins.FirstOrDefault(x => x.Guid == userId);
            var role = user != null ? user.UserRoles.Select(x => x.Role) : null;
            var rolename = role != null ? role.Select(x => x.Name) : new List<string>();
            if (rolename.Contains(Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")))
            {
                return this.dbContext.ProjectStaffMemberRoles.ToList().GroupBy(g => g.ProjectId).Select(x => x.First()).Select(ToProjectViewModel).ToList();
            }
            return this.dbContext.ProjectStaffMemberRoles
                .Where(u => u.UserLogin.Guid == userId)
                .OrderByDescending(d => d.Id)
                .Select(ToProjectViewModel)
                .ToList();
        }

        public ProjectViewModel ToProjectViewModel(ProjectStaffMemberRole project)
        {
            var projName = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.ProjectId && x.Variable.VariableName == Core.Enum.DefaultsVariables.Name.ToString());
            return new ProjectViewModel()
            {
                ProjectName = projName != null ? projName.SelectedValues : string.Empty,
                Guid = project.FormDataEntry.Guid,
                RoleGuid = project.Role.Guid,
                RoleId = project.RoleId,
            };
        }

        public ProjectBasicDetailsViewModel ProjectBasicDetail(Guid guid)
        {
            ProjectBasicDetailsViewModel model = new ProjectBasicDetailsViewModel();
            FormDataEntry formDataEntry = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == guid);
            if (formDataEntry != null)
            {
                if (formDataEntry.ProjectDeployStatus == (int)ProjectStatusTypes.Published)
                {
                    long? id = formDataEntry.EntityNumber;
                    var condition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, id);
                    var projectDetails = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();
                    if (projectDetails != null)
                    {
                        model.Name = projectDetails.formDataEntryVariableMongoList.Where(x => x.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectDisplayName = projectDetails.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectDisplayName").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectColor = projectDetails.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectColor").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectLogo = projectDetails.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectLogo").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectDisplayNameTextColour = projectDetails.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProjectDisplayNameTextColour").Select(x => x.SelectedValues).FirstOrDefault();
                        model.Guid = guid;
                        return model;
                    }
                    else
                    {
                        model.Name = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectDisplayName = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectDisplayName").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectColor = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectColor").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectLogo = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectLogo").Select(x => x.SelectedValues).FirstOrDefault();
                        model.ProjectDisplayNameTextColour = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectDisplayNameTextColour").Select(x => x.SelectedValues).FirstOrDefault();
                        model.Id = formDataEntry.Id;
                        model.Guid = guid;
                        return model;
                    }
                }
                else
                {
                    model.Name = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectDisplayName = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectDisplayName").Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectColor = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectColor").Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectLogo = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectLogo").Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectDisplayNameTextColour = formDataEntry.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProjectDisplayNameTextColour").Select(x => x.SelectedValues).FirstOrDefault();
                    model.Id = formDataEntry.Id;
                    model.Guid = guid;
                    return model;
                }
            }
            return null;
        }
    }
}