using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Core.Enum;
using Aspree.Data.MongoDB;
using Aspree.Core.ViewModels.MongoViewModels;


namespace Aspree.Provider.Provider
{
    public class FormProvider : IFormProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly IVariableCategoryProvider _VariableCategoryProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly IFormCategoryProvider _FormCategoryProvider;
        private readonly IEntityTypeProvider _EntityTypeProvider;
        private readonly MongoDBContext _mongoDBContext;

        public FormProvider(AspreeEntities dbContext, IUserLoginProvider userLoginProvider, IVariableCategoryProvider variableCategoryProvider, IRoleProvider roleProvider, IFormCategoryProvider formCategoryProvider, IEntityTypeProvider entityTypeProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = dbContext;
            this._userLoginProvider = userLoginProvider;

            this._VariableCategoryProvider = variableCategoryProvider;
            this._RoleProvider = roleProvider;
            this._FormCategoryProvider = formCategoryProvider;
            this._EntityTypeProvider = entityTypeProvider;
            this._mongoDBContext = mongoDBContext;
        }

        public FormViewModel Create(FormViewModel model)
        {
            var project = this.dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);
            if (dbContext.Forms.Any(et => et.FormTitle.ToLower() == model.FormTitle.ToLower() && et.ProjectId == project.Id && et.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Form already exists.");
            }

            if (model.Variables == null || model.Variables.Count == 0)
            {
                throw new Core.BadRequestException("Please select atleast 1 varialbe on the form.");
            }

            if (model.Variables.Any(c => c.FormVariableRoles == null || c.FormVariableRoles.Count == 0))
            {
                //throw new Core.BadRequestException("Please select variable roles for form.");
            }

            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var formCategory = this.dbContext.FormCategories.FirstOrDefault(et => et.Guid == model.FormCategoryId);
            var tenant = this.dbContext.Tenants.FirstOrDefault(et => et.Guid == model.TenantId);
            var formStatus = this.dbContext.FormStatus.FirstOrDefault(et => et.Status == "Draft");


            var form = new Form()
            {
                Guid = Guid.NewGuid(),
                FormTitle = model.FormTitle,
                FormCategoryId = formCategory != null ? formCategory.Id : (int?)null,
                FormStatusId = formStatus.Id,
                IsTemplate = model.IsTemplate,
                IsPublished = model.IsPublished,
                PreviousVersion = model.PreviousVersion,
                Version = model.Version,
                ApprovedBy = model.ApprovedBy,
                ApprovedDate = model.ApprovedDate,
                FormState = (int)Core.Enum.FormStateTypes.Draft,
                ProjectId = project != null ? project.Id : (int?)null,
                TenantId = tenant.Id,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Custom,
            };

            dbContext.Forms.Add(form);

            SaveChanges();
            var varIds = model.Variables.Select(v => v.VariableId).ToList();
            var roleIds = model.Variables.SelectMany(v => v.FormVariableRoles).ToList();

            var variables = dbContext.Variables.Where(c => varIds.Contains(c.Guid)).ToList();
            var roles = dbContext.Roles.Where(c => roleIds.Contains(c.Guid)).ToList();
            var entityTypes = dbContext.EntityTypes.Where(c => model.EntityTypes.Contains(c.Guid)).ToList();
            int VariableOrderNo = 0;
            foreach (var variable in model.Variables)
            {
                VariableOrderNo++;
                var variableDb = variables.FirstOrDefault(v => v.Guid == variable.VariableId);
                var dependentVariableid = dbContext.Variables.FirstOrDefault(v => v.Guid == variable.DependentVariableId);
                variable.IsBlank = dependentVariableid != null ? variable.IsBlank == true ? true : (bool?)null : (bool?)null;
                var formVariable = new FormVariable()
                {
                    Guid = Guid.NewGuid(),
                    VariableId = variableDb.Id,
                    FormId = form.Id,
                    ValidationRuleType = variable.ValidationRuleType,
                    HelpText = variable.HelpText,
                    IsRequired = variable.IsRequired,
                    MaxRange = variable.MaxRange,
                    MinRange = variable.MinRange,
                    RegEx = variable.RegEx,
                    DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    ValidationMessage = variable.ValidationMessage,
                    ResponseOption = variable.ResponseOption,
                    QuestionText = variable.QuestionText,
                    VariableOrderNo = VariableOrderNo,
                    IsBlank = variable.IsBlank,
                };

                dbContext.FormVariables.Add(formVariable);

                SaveChanges();

                foreach (var formRole in variable.formVariableRoleViewModel)
                {
                    var roleDb = roles.FirstOrDefault(v => v.Guid == formRole.RoleGuidId);
                    dbContext.FormVariableRoles.Add(new FormVariableRole()
                    {
                        Guid = Guid.NewGuid(),
                        RoleId = roleDb.Id,
                        FormVariableId = formVariable.Id,
                        CanCreate = formRole.CanCreate,
                        CanView = formRole.CanView,
                        CanEdit = formRole.CanEdit,
                        CanDelete = formRole.CanDelete,
                    });
                }

                SaveChanges();
            }

            foreach (var entityType in entityTypes)
            {
                this.dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    EntityTypeId = entityType.Id
                });
            }
            SaveChanges();
            return GetByGuid(form.Guid);
        }

        public FormViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var form = dbContext.Forms.FirstOrDefault(fs => fs.Guid == guid);
            if (form != null)
            {
                form.DeactivatedBy = deactivatedBy.Id;
                form.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(form);
            }
            return null;
        }

        public FormViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var form = dbContext.Forms.FirstOrDefault(fs => fs.Id == id);
            if (form != null)
            {
                form.DeactivatedBy = deactivatedBy.Id;
                form.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(form);
            }

            return null;
        }

        public IEnumerable<FormViewModel> GetAll()
        {
            return dbContext.Forms
             .Select(ToModel)
             .ToList();
        }

        public IEnumerable<FormViewModel> GetAll(Guid tenantId)
        {
            return dbContext.Forms
             .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId)
             .Select(ToModel)
             .ToList();
        }

        public FormViewModel GetByGuid(Guid guid)
        {
            var form = dbContext.Forms
              .FirstOrDefault(fs => fs.Guid == guid);

            if (form != null)
                return ToModel(form);

            return null;
        }

        public FormViewModel GetUSerFormByGuid(Guid guid, Guid logginuserId, Guid projectid)
        {
            var form = dbContext.Forms
              .FirstOrDefault(fs => fs.Guid == guid);

            if (form != null)
                return ToNewModel(form, logginuserId, projectid);

            return null;
        }

        public FormViewModel GetById(int id)
        {
            var form = dbContext.Forms
             .FirstOrDefault(fs => fs.Id == id);

            if (form != null)
                return ToModel(form);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public FormViewModel ToModel(Form entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).OrderBy(c => c.Id).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,
                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToVariableViewModel(c.Variable),
                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,
                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,
                    QuestionText = c.QuestionText,
                    IsBlank = c.IsBlank,
                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                IsDefaultForm = entity.IsDefaultForm,
            };
        }

        public FormViewModel ToNewModel(Form entity, Guid? loggedUserGuid = null, Guid? Projectid = null)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var userRole = "";
            if (loggedUserGuid.HasValue && Projectid.HasValue)
            {
                userRole = _userLoginProvider.GetUserRoleByProjectId(loggedUserGuid.Value, Projectid.Value);
            }
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            
            int count = 0;
            try
            {
                var mongoEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().Where(x => x.FormGuid == entity.Guid && x.DateDeactivated == null);
                if (mongoEntities != null)
                {
                    count = mongoEntities.Count();
                }
                count = count != 0 ? count : entity.FormDataEntries.Count;
            }
            catch (Exception ee)
            { }

            var isInDepployActivities = false;
            foreach (var item in entity.ActivityForms)
            {
                if (item.Activity.ActivityStatusId == (int)ActivityStatusTypes.Active)
                {
                    isInDepployActivities = true;
                    break;
                }
                else
                {
                    isInDepployActivities = false;
                }
            }
            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                FormIsInDeployeActiviti = isInDepployActivities,
                IsFormContaindData = count > 0,
                UserTypeRole = userRole,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).OrderBy(cc => cc.Id).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,
                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToVariableViewModel(c.Variable),
                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,
                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,
                    QuestionText = c.QuestionText,
                    IsBlank = c.IsBlank,
                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                IsDefaultForm = entity.IsDefaultForm,
            };
        }

        public FormViewModel Update(FormViewModel model)
        {
            if (dbContext.Forms.Any(et => et.FormTitle.ToLower() == model.FormTitle.ToLower()
            && et.Guid != model.Guid && et.DateDeactivated == null && et.FormDataEntry.Guid == model.ProjectId))
            {
                throw new Core.AlreadyExistsException("Form already exists.");
            }

            if (model.Variables == null || model.Variables.Count == 0)
            {
                throw new Core.BadRequestException("Please select atleast 1 varialbe on the form.");
            }

            if (model.Variables.Where(x => x.IsDefaultVariableType != (int)Core.Enum.DefaultVariableType.Heading).Any(c => c.FormVariableRoles == null || c.FormVariableRoles.Count == 0))
            {
                //throw new Core.BadRequestException("Please select variable roles for form.");
            }

            var form = dbContext.Forms
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            var project = this.dbContext.Projects.FirstOrDefault(x => x.Guid == model.ProjectId);

            var formCategory = this.dbContext.FormCategories.FirstOrDefault(et => et.Guid == model.FormCategoryId);

            if (form != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                form.ModifiedDate = DateTime.UtcNow;
                form.ModifiedBy = modifiedBy.Id;
                form.FormTitle = model.FormTitle;
                form.ApprovedBy = model.ApprovedBy;
                form.ApprovedDate = model.ApprovedDate;
                form.FormCategoryId = formCategory.Id;
                form.PreviousVersion = model.PreviousVersion;
                form.Version = model.Version;
                form.IsPublished = model.IsPublished;

                if (model.IsPublished)
                {
                    form.FormStatusId = dbContext.FormStatus.FirstOrDefault(x => x.Status == "Published").Id;
                    form.FormState = dbContext.FormStatus.FirstOrDefault(x => x.Status == "Published").Id;
                }

                dbContext.FormVariableRoles.RemoveRange(form.FormVariables.SelectMany(c => c.FormVariableRoles).ToList());
                dbContext.FormVariables.RemoveRange(form.FormVariables.Select(c => c).ToList());
                dbContext.FormEntityTypes.RemoveRange(form.FormEntityTypes.Select(c => c).ToList());

                SaveChanges();

                var varIds = model.Variables.Select(v => v.VariableId).ToList();
                var roleIds = model.Variables.SelectMany(v => v.FormVariableRoles).ToList();

                var variables = dbContext.Variables.Where(c => varIds.Contains(c.Guid)).ToList();
                var roles = dbContext.Roles.Where(c => roleIds.Contains(c.Guid)).ToList();
                var entityTypes = dbContext.EntityTypes.Where(c => model.EntityTypes.Contains(c.Guid)).ToList();

                int VariableOrderNo = 0;
                foreach (var variable in model.Variables)
                {
                    VariableOrderNo++;
                    var variableDb = variables.FirstOrDefault(v => v.Guid == variable.VariableId);
                    var dependentVariableId = dbContext.Variables.FirstOrDefault(v => v.Guid == variable.DependentVariableId);
                    variable.IsBlank = dependentVariableId != null ? variable.IsBlank == true ? true : (bool?)null : (bool?)null;
                    var formVariable = new FormVariable()
                    {
                        Guid = Guid.NewGuid(),
                        VariableId = variableDb.Id,
                        FormId = form.Id,
                        ValidationRuleType = variable.ValidationRuleType,
                        HelpText = variable.HelpText,
                        IsRequired = variable.IsRequired,
                        MaxRange = variable.MaxRange,
                        MinRange = variable.MinRange,
                        RegEx = variable.RegEx,
                        DependentVariableId = dependentVariableId != null ? dependentVariableId.Id : (int?)null,
                        ValidationMessage = variable.ValidationMessage,
                        ResponseOption = variable.ResponseOption,
                        QuestionText = variable.QuestionText,
                        VariableOrderNo = VariableOrderNo,
                        IsBlank = variable.IsBlank,
                    };

                    dbContext.FormVariables.Add(formVariable);

                    SaveChanges();

                    foreach (var formRole in variable.formVariableRoleViewModel)
                    {
                        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole.RoleGuidId);

                        if (formRole.CanEdit && formRole.CanCreate && !formRole.CanView)
                        {
                            formRole.CanView = true;
                        }

                        dbContext.FormVariableRoles.Add(new FormVariableRole()
                        {
                            Guid = Guid.NewGuid(),
                            RoleId = roleDb.Id,
                            FormVariableId = formVariable.Id,
                            CanCreate = formRole.CanCreate,
                            CanView = formRole.CanView,
                            CanEdit = formRole.CanEdit,
                            CanDelete = formRole.CanDelete,
                        });
                    }

                    SaveChanges();
                }

                foreach (var entityType in entityTypes)
                {
                    this.dbContext.FormEntityTypes.Add(new FormEntityType()
                    {
                        Guid = Guid.NewGuid(),
                        FormId = form.Id,
                        EntityTypeId = entityType.Id
                    });
                }
                SaveChanges();

                return GetByGuid(model.Guid);
            }

            return null;
        }

        public VariableViewModel ToVariableViewModel(Variable model)
        {
            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == model.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == model.Id).Select(ToVariableValidationRuleViewModel).ToList();

            List<Guid> usedInFormsList = new List<Guid>();
            try
            {
                usedInFormsList = dbContext.FormVariables.Where(x => x.VariableId == model.Id).Select(x => x.Form.Guid).ToList();
            }
            catch (Exception ex) { }

            return new VariableViewModel()
            {
                Guid = model.Guid,
                VariableName = model.VariableName,
                Id = model.Id,
                CanCollectMultiple = model.CanCollectMultiple,
                HelpText = model.HelpText,
                IsApproved = model.IsApproved,
                IsRequired = model.IsRequired,
                IsSoftRange = model.IsSoftRange,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                Question = model.Question,
                RegEx = model.RegEx,
                RequiredMessage = model.RequiredMessage,
                ValidationMessage = model.ValidationMessage,
                ValueDescription = model.ValueDescription,
                Values = model.Values.Split('|').ToList(),
                VariableLabel = model.VariableLabel,
                ModifiedDate = model.ModifiedDate,
                DateDeactivated = model.DateDeactivated,
                VariableRoles = model.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = model.VariableType.Type,
                Comment = model.Comment,
                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,
                VariableValueDescription = model.VariableValueDescription != null ? model.VariableValueDescription.Split('|').ToList() : null,

                CanFutureDate = model.CanFutureDate,
                VariableUsedInFormsList = usedInFormsList,
            };
        }

        public VariableValidationRuleViewModel ToVariableValidationRuleViewModel(VariableValidationRule data)
        {
            return new VariableValidationRuleViewModel()
            {
                Id = data.Id,
                Guid = data.Guid,
                LimitType = data.LimitType,
                Max = data.Max,
                Min = data.Min,
                RegEx = data.RegEx,
                ValidationId = data.ValidationId,
                ValidationMessage = data.ValidationMessage,
                VariableId = data.VariableId,
                ValidationName = data.ValidationRule != null ? data.ValidationRule.RuleType : "Custom",
            };
        }

        public ProjectBuilderFormsViewModel GetProjectBuilderForms(Guid tenantId, Guid LoggedInUserId, Guid projectId)
        {
            #region check login
            string userRole = _userLoginProvider.GetUserRoleByProjectId(LoggedInUserId, projectId);
            string[] allowedRole = new string[] {
                Core.Enum.RoleTypes.System_Admin.ToString().Replace("_"," ")
                , Core.Enum.RoleTypes.Project_Admin.ToString().Replace("_"," ")
            };
            if (!allowedRole.Contains(userRole))
            {
                throw new Core.UnauthorizedException("Authorization has been denied for this request.");
            }
            #endregion            

            ProjectBuilderFormsViewModel projectBuilderFormsViewModel = new ProjectBuilderFormsViewModel();
            projectBuilderFormsViewModel.VariableCategories = _VariableCategoryProvider.GetAll(tenantId);
            projectBuilderFormsViewModel.Roles = _RoleProvider.GetAll(tenantId);
            projectBuilderFormsViewModel.FormCategory = _FormCategoryProvider.GetAll();
            projectBuilderFormsViewModel.EntityTypes = _EntityTypeProvider.GetAll();
            return projectBuilderFormsViewModel;
        }


        public IEnumerable<FormViewModel> GetAllDefaultForms(Guid tenantId)
        {
            return dbContext.Forms
             .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId && v.ProjectId == null)
             .Select(ToModel)
             .ToList();
        }

        public IEnumerable<FormViewModel> GetProjectDefaultForms(Guid tenantId, Guid projectId)
        {
            return dbContext.Forms
             .Where(v => v.FormDataEntry.Guid == projectId && v.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default)
             .Select(ToModel)
             .ToList();
        }


        public IEnumerable<FormViewModel> GetFormsByGuidList(List<Guid> guids)
        {
            return dbContext.Forms
                 .Where(v => guids.Contains(v.Guid)).Select(ToGetFormsByGuidListModel).ToList();
        }
        public FormViewModel ToGetFormsByGuidListModel(Form entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,
                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToGetFormsByGuidListVariableViewModel(c.Variable, entity.Id),

                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,

                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),
                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),


                IsDefaultForm = entity.IsDefaultForm,
            };
        }
        public VariableViewModel ToGetFormsByGuidListVariableViewModel(Variable model, int formId)
        {
            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == model.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == model.Id).Select(ToVariableValidationRuleViewModel).ToList();
            var formDataEntryVariables = this.dbContext.FormDataEntryVariables.FirstOrDefault(x => x.VariableId == model.Id && x.FormDataEntry.FormId == formId);
            return new VariableViewModel()
            {
                Guid = model.Guid,
                VariableName = model.VariableName,
                Id = model.Id,
                CanCollectMultiple = model.CanCollectMultiple,
                HelpText = model.HelpText,
                IsApproved = model.IsApproved,
                IsRequired = model.IsRequired,
                IsSoftRange = model.IsSoftRange,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                Question = model.Question,
                RegEx = model.RegEx,
                RequiredMessage = model.RequiredMessage,
                ValidationMessage = model.ValidationMessage,

                ValueDescription = model.ValueDescription,
                Values = model.Values.Split('|').ToList(),

                VariableLabel = model.VariableLabel,
                ModifiedDate = model.ModifiedDate,
                DateDeactivated = model.DateDeactivated,
                VariableRoles = model.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = model.VariableType.Type,
                Comment = model.Comment,

                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,
                VariableValueDescription = model.VariableValueDescription != null ? model.VariableValueDescription.Split('|').ToList() : null,
                VariableSelectedValues = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : "",
                CanFutureDate = model.CanFutureDate,
            };
        }


        public FormViewModel GetActivityFormBySearchedEntity(int entId, Guid formId, Guid activityId, int summarypageActivityId)
        {
            Form frm = dbContext.Forms.FirstOrDefault(x => x.Guid == formId);

            bool isNewForm = true;
            var isCustomEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.ParentEntityNumber == entId && x.Form.Guid == formId && x.SubjectId == summarypageActivityId);
            if (frm != null)
            {
                if (frm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default)
                {
                    isCustomEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.ParentEntityNumber == entId && x.Form.FormTitle == frm.FormTitle && x.SubjectId == summarypageActivityId);
                }
            }

            if (isCustomEntity != null)
            {
                entId = isCustomEntity.EntityNumber != null ? (int)isCustomEntity.EntityNumber : entId;
                isNewForm = false;
            }
            else
            {
                isCustomEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entId && x.Form.Guid == formId && x.SubjectId == summarypageActivityId);
                if (frm != null)
                {
                    if (frm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default)
                    {
                        isCustomEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entId && x.Form.FormTitle == frm.FormTitle && x.SubjectId == summarypageActivityId);
                    }
                }

                isNewForm = false;
            }
            var form = dbContext.Forms
                .FirstOrDefault(fs => fs.Guid == formId);

            if (form != null)
                return ToActivityFormModel(form, entId, isNewForm, summarypageActivityId);

            return null;
        }
        public FormViewModel ToActivityFormModel(Form entity, int entId, bool isNewForm, int summarypageActivityId)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;
            var formfataentryguid = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entId && x.Form.FormTitle == entity.FormTitle && x.SubjectId == summarypageActivityId);

            string ModifiedByString = string.Empty, ModifiedDateString = string.Empty;
            if (entity.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                formfataentryguid = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == entId && x.SubjectId == summarypageActivityId);
            }


            #region modified by details
            if (formfataentryguid != null)
            {
                try
                {
                    ModifiedDateString = formfataentryguid.ModifiedDate != null ? formfataentryguid.ModifiedDate?.ToLocalTime().ToString("dd-MMM-yyyy HH:mm:ss") : formfataentryguid.CreatedDate.ToLocalTime().ToString("dd-MMM-yyyy HH:mm:ss");
                    int modifierId = formfataentryguid.ModifiedBy != null ? (int)formfataentryguid.ModifiedBy : formfataentryguid.CreatedBy;
                    var formmodifiedBy = _userLoginProvider.GetById(modifierId);
                    ModifiedByString = formmodifiedBy != null ? formmodifiedBy.FirstName + " " + formmodifiedBy.LastName : string.Empty;
                }
                catch (Exception formmodifier)
                { }
            }
            #endregion

            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.OrderBy(x => x.VariableOrderNo).Where(x => x.DateDeactivated == null).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,

                    DependentVariableId = c.DependentVariableId != null ? dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault() : (Guid?)null,//.DependentVariableId,
                                                                                                                                                                                           //                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToActivityFormVariableModel(c.Variable, entity.Id, entId, summarypageActivityId),

                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,

                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),

                    QuestionText = c.QuestionText,
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,

                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),


                IsDefaultForm = entity.IsDefaultForm,
                IsNewForm = isNewForm,
                FormDataEntryGuid = formfataentryguid != null ? formfataentryguid.Guid : (Guid?)null,

                ModifiedByString = ModifiedByString,
                ModifiedDateString = ModifiedDateString,
            };
        }
        public VariableViewModel ToActivityFormVariableModel(Variable model, int formId, int selectedValue, int summarypageActivityId)
        {
            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == model.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == model.Id).Select(ToVariableValidationRuleViewModel).ToList();

            var FormDataEntryVariables = this.dbContext.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID" && x.SelectedValues == selectedValue.ToString());

            int formdataentryid = 0;
            try
            {
                formdataentryid = FormDataEntryVariables.FirstOrDefault(x => x.SelectedValues == selectedValue.ToString() && x.Variable.VariableName == "EntID").FormDataEntryId;
            }
            catch (Exception e)
            {


            }

            if (model.VariableName == DefaultsVariables.AuthenticationMethod.ToString())
            {
                var authTypeId = dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                model.Values = string.Join("|", authTypeId.Select(x => x.id).ToList());
                model.VariableValueDescription = string.Join("|", authTypeId.Select(x => x.name).ToList());
            }
            if (model.VariableName == DefaultsVariables.ProRole.ToString())
            {
                var roles = dbContext.Roles.Where(x => x.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")
                        && x.DateDeactivated == null).OrderByDescending(x => x.Id).Select(x => new { id = x.Guid.ToString(), name = x.Name }).ToList();

                model.Values = string.Join("|", roles.Select(x => x.id).ToList());
                model.VariableValueDescription = string.Join("|", roles.Select(x => x.name).ToList());
            }

            var formDataEntryVariables = this.dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == formdataentryid && x.VariableId == model.Id);

            var formDataEntry = this.dbContext.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "EntID" && x.SelectedValues == selectedValue.ToString());

            var variableEntityType = this.dbContext.VariableEntityTypes.FirstOrDefault(x => x.VariableId == model.Id);

            Guid? usernameGuid = null;
            if (model.VariableName == "Username")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : null;
                try
                {
                    var username = dbContext.UserLogins.FirstOrDefault(x => x.UserName == email && x.Id == formDataEntry.FormDataEntry.ThisUserId);
                    if (username != null)
                    {
                        usernameGuid = username.Guid;
                    }
                }
                catch (Exception ex) { }
            }

            if (model.VariableName == "Email")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : null;
                try
                {
                    var username = dbContext.UserLogins.FirstOrDefault(x => x.Email == email && x.Id == formDataEntry.FormDataEntry.ThisUserId);
                    if (username != null)
                    {
                        usernameGuid = username.Guid;
                    }
                }
                catch (Exception ex) { }
            }

            List<LinkedProjectGroupViewModel> linkedProject = new List<LinkedProjectGroupViewModel>();
            if (model.VariableType.Type == "LKUP")
            {
                var entType = model.VariableEntityTypes.FirstOrDefault();
                int entTypeid = entType != null ? entType.EntityTypeId : 0;
                var entTypes = dbContext.EntityTypes.FirstOrDefault(x => x.Id == entTypeid);

                int? entsubtype = null;
                string entsubtypeID = string.Empty;
                if (model.VariableEntityTypes.Count == 1)
                {
                    entsubtype = entType != null ? entType.EntitySubTypeId : (int?)null;
                }

                string entTypeName = entTypes != null ? entTypes.Name : string.Empty;
                if (entTypeName == "Person") { entTypeName = "Person Registration"; }
                else if (entTypeName == "Project") { entTypeName = "Project Registration"; }
                else if (entTypeName == "Participant") { entTypeName = "Participant Registration"; }
                else if (entTypeName == "Place/Group") { entTypeName = "Place/Group Registration"; }

                List<string> entityValues = new List<string>();
                List<string> entityText = new List<string>();

                var getAllformWithentTypeid = dbContext.FormEntityTypes.Where(x => x.EntityTypeId == entTypeid).Select(x => x.FormId).ToList();

                var formDataEntryEntityTypesList = dbContext.FormDataEntries.Where(t => getAllformWithentTypeid.Contains((int)t.FormId)).ToList();

                if (formDataEntryEntityTypesList.Count() > 0)
                {
                    foreach (var proj in formDataEntryEntityTypesList)
                    {
                        var text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                        var valuse = proj.Guid;

                        if (entsubtype != null)
                        {
                            text = null;
                            if (entTypeName == "Person Registration")
                            {
                                var persuntype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "PerSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (persuntype == entSubTyp)
                                    {
                                        var text1 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                                        var text2 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                        text = text1 + " " + text2;
                                    }
                                }
                            }
                            else if (entTypeName == "Project Registration")
                            {
                                var projectsubtype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (projectsubtype == entSubTyp)
                                    {
                                        text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                    }
                                }
                            }
                        }
                        if (text != null)
                        {
                            if (model.VariableName == DefaultsVariables.LnkPro.ToString())
                            {
                                var grpName = proj.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString());
                                int projGroup = grpName != null ? !string.IsNullOrEmpty(grpName.SelectedValues) ? Convert.ToInt32(grpName.SelectedValues) : 0 : 0;
                                projGroup = projGroup == 0 ? (int)ProjectSubTypeEnum.Other : projGroup;
                                var projectgroupname = Enum.GetName(typeof(ProjectSubTypeEnum), projGroup);

                                linkedProject.Add(new LinkedProjectGroupViewModel()
                                {
                                    GroupName = !string.IsNullOrEmpty(projectgroupname) ? projectgroupname.Replace("_", " ") : "",
                                    ProjectId = valuse.ToString(),
                                    ProjectName = text,
                                });
                            }

                            entityValues.Add(valuse.ToString());
                            entityText.Add(text);
                        }
                    }
                    model.Values = entityValues != null ? string.Join("|", entityValues) : string.Empty;
                    model.VariableValueDescription = entityText != null ? string.Join("|", entityText) : string.Empty;
                }
            }

            return new VariableViewModel()
            {
                Guid = model.Guid,
                VariableName = model.VariableName,
                Id = model.Id,
                CanCollectMultiple = model.CanCollectMultiple,
                HelpText = model.HelpText,
                IsApproved = model.IsApproved,
                IsRequired = model.IsRequired,
                IsSoftRange = model.IsSoftRange,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                Question = model.Question,
                RegEx = model.RegEx,
                RequiredMessage = model.RequiredMessage,
                ValidationMessage = model.ValidationMessage,
                ValueDescription = model.ValueDescription,
                Values = model.Values.Split('|').ToList(),
                VariableLabel = model.VariableLabel,
                ModifiedDate = model.ModifiedDate,
                DateDeactivated = model.DateDeactivated,
                VariableRoles = model.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = model.VariableType.Type,
                Comment = model.Comment,

                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,
                VariableValueDescription = model.VariableValueDescription != null ? model.VariableValueDescription.Split('|').ToList() : null,
                VariableSelectedValues = formDataEntryVariables != null ? formDataEntryVariables.SelectedValues : "",
                FormDataEntryGuid = formDataEntry != null ? formDataEntry.FormDataEntry.Guid : (Guid?)null,

                LookupEntityType = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Guid : (Guid?)null : (Guid?)null,
                LookupEntitySubtype = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Guid : (Guid?)null : (Guid?)null,

                LookupEntityTypeName = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Name : null : null,
                LookupEntitySubtypeName = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Name : null : null,
                CanFutureDate = model.CanFutureDate,
                UserNameVariableGuid = usernameGuid,
                LinkedProjectListWithGroupList = model.VariableName == DefaultsVariables.LnkPro.ToString() ? linkedProject : null,
            };
        }

        public IEnumerable<FormViewModel> GetAllForms(Guid tenantId, Guid projectId)
        {
            List<FormViewModel> formList = new List<FormViewModel>();
            IEnumerable<Form> forms = dbContext.Forms
             .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId && v.ProjectId.HasValue && v.FormDataEntry.Guid == projectId);
            forms.ToList().ForEach(item =>
            {
                var _thisForm = ToFormModel(item, projectId);
                _thisForm.FormUsedInActivityList = new List<Guid>();
                var canMultiple = _thisForm.Variables.Where(x => x.variableViewModel.CanCollectMultiple == false).ToList();
                if (canMultiple.Count() > 0)
                {
                    _thisForm.FormUsedInActivityList = dbContext.ActivityForms.Where(x => x.FormId == _thisForm.Id).Select(x => x.Activity.Guid).ToList();
                    _thisForm.UsedVariablesNameList = canMultiple.Select(x => x.VariableName).ToList();
                }
                formList.Add(_thisForm);
            });
            return formList;
        }
        public FormViewModel ToFormModel(Form entity, Guid projectId)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,
                    DependentVariableId = dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault(),//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = ToVariableModel(c.Variable, projectId),
                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,
                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,
                    QuestionText = c.QuestionText,
                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                IsDefaultForm = entity.IsDefaultForm,
            };
        }
        public VariableViewModel ToVariableModel(Variable entity, Guid projectId)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var dependentVarId = this.dbContext.Variables.FirstOrDefault(et => et.Id == entity.DependentVariableId);
            var validationRule = this.dbContext.ValidationRules.FirstOrDefault(et => et.Id == entity.ValidationRuleId);
            var variableCategory = this.dbContext.VariableCategories.FirstOrDefault(et => et.Id == entity.VariableCategoryId);
            var variableType = this.dbContext.VariableTypes.FirstOrDefault(et => et.Id == entity.VariableTypeId);

            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == entity.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == entity.Id).Select(ToVariableValidationRuleViewModel).ToList();
            var variableEntityType = this.dbContext.VariableEntityTypes.FirstOrDefault(x => x.VariableId == entity.Id);
            var entitysubtype = this.dbContext.VariableEntityTypes.Where(x => x.VariableId == entity.Id).ToList();

            Guid? LookupEntitySubtype;
            string LookupEntitySubtypeName;
            if (entitysubtype.Count > 1)
            {
                LookupEntitySubtype = new Guid();
                LookupEntitySubtypeName = "All";
            }
            else if (entitysubtype.Count == 1)
            {
                LookupEntitySubtype = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Guid : (Guid?)null : (Guid?)null;
                LookupEntitySubtypeName = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Name : null : null;
            }
            else
            {
                LookupEntitySubtype = new Guid();
                LookupEntitySubtypeName = null;
            }
            List<Guid> usedInFormsList = new List<Guid>();
            try
            {
                usedInFormsList = dbContext.FormVariables.Where(x => x.VariableId == entity.Id && x.Form.FormDataEntry.Guid == projectId).Select(x => x.Form.Guid).ToList();
            }
            catch (Exception ex) { }
            return new VariableViewModel()
            {
                Guid = entity.Guid,
                VariableName = entity.VariableName,
                Id = entity.Id,
                CanCollectMultiple = entity.CanCollectMultiple,
                DependentVariableId = entity.DependentVariableId.HasValue ? dependentVarId.Guid : (Guid?)null,
                HelpText = entity.HelpText,
                IsApproved = entity.IsApproved,
                IsRequired = entity.IsRequired,
                IsSoftRange = entity.IsSoftRange,
                MaxRange = entity.MaxRange,
                MinRange = entity.MinRange,
                Question = entity.Question,
                RegEx = entity.RegEx,
                RequiredMessage = entity.RequiredMessage,
                ValidationMessage = entity.ValidationMessage,
                ValidationRuleId = validationRule != null ? validationRule.Guid : (Guid?)null,
                ValueDescription = entity.ValueDescription,
                Values = entity.Values.Split('|').ToList(),
                VariableCategoryId = variableCategory != null ? variableCategory.Guid : (Guid?)null,
                VariableLabel = entity.VariableLabel,
                VariableTypeId = variableType != null ? variableType.Guid : Guid.Empty,
                CreatedBy = createdBy.Guid,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                DateDeactivated = entity.DateDeactivated,
                VariableRoles = entity.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = variableType.Type,
                Comment = entity.Comment,
                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,

                VariableValueDescription = entity.VariableValueDescription != null ? entity.VariableValueDescription.Split('|').ToList() : null,
                IsDefaultVariable = entity.IsDefaultVariable,

                LookupEntityType = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Guid : (Guid?)null : (Guid?)null,
                LookupEntitySubtype = LookupEntitySubtype,

                LookupEntityTypeName = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Name : null : null,
                LookupEntitySubtypeName = LookupEntitySubtypeName,
                CanFutureDate = entity.CanFutureDate,

                VariableUsedInFormsList = usedInFormsList,
            };
        }

        public FormViewModel TestEnvironment_GetActivityFormBySearchedEntity(int entId, Guid formId, Guid activityId)
        {
            var isCustomEntity = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.ParentEntityNumber == entId && x.Form.Guid == formId);
            if (isCustomEntity != null)
            {
                entId = isCustomEntity.EntId != 0 ? (int)isCustomEntity.EntId : entId;
            }

            var form = dbContext.Forms
                .FirstOrDefault(fs => fs.Guid == formId);

            if (form != null)
                return TestEnvironment_ToActivityFormModel(form, entId);

            return null;
        }
        public FormViewModel TestEnvironment_ToActivityFormModel(Form entity, int entId)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var project = entity.ProjectId.HasValue ? dbContext.Projects.FirstOrDefault(p => p.Id == entity.ProjectId) : null;

            var formfataentryguid = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == entId && x.Form.Id == entity.Id);
            return new FormViewModel()
            {
                Id = entity.Id,
                Guid = entity.Guid,
                ApprovedBy = entity.ApprovedBy,
                ApprovedDate = entity.ApprovedDate,
                Version = entity.Version,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                FormCategoryId = entity.FormCategory.Guid,
                FormState = entity.FormState,
                FormStatusId = entity.FormStatu.Guid,
                FormTitle = entity.FormTitle,
                IsPublished = entity.IsPublished,
                IsTemplate = entity.IsTemplate,
                PreviousVersion = entity.PreviousVersion,
                ProjectId = project != null ? project.Guid : (Guid?)null,
                TenantId = entity.Tenant.Guid,
                Variables = entity.FormVariables.Where(x => x.DateDeactivated == null).Select(c => new FormVariableViewModel()
                {
                    VariableId = c.Variable.Guid,
                    VariableName = c.Variable.VariableName,
                    FormVariableRoles = c.FormVariableRoles.Select(fv => fv.Role.Guid).ToList(),
                    ValidationRuleType = c.ValidationRuleType,
                    HelpText = c.HelpText,
                    IsRequired = c.IsRequired,
                    MaxRange = c.MaxRange,
                    MinRange = c.MinRange,
                    RegEx = c.RegEx,
                    DependentVariableId = c.DependentVariableId != null ? dbContext.Variables.Where(x => x.Id == c.DependentVariableId).Select(s => s.Guid).FirstOrDefault() : (Guid?)null,//.DependentVariableId,
                    ResponseOption = c.ResponseOption,
                    ValidationMessage = c.ValidationMessage,
                    VariableType = c.Variable.VariableType.Type,
                    variableViewModel = TestEnvironment_ToActivityFormVariableModel(c.Variable, entity.Id, entId),
                    IsSearchVisible = c.IsSearchVisible,
                    SearchPageOrder = c.SearchPageOrder,

                    formVariableRoleViewModel = c.FormVariableRoles.Select(f => new FormVariableRoleViewModel()
                    {
                        FormVariableId = f.FormVariableId,
                        RoleGuidId = f.Role.Guid,
                        Guid = f.Guid,
                        CanCreate = f.CanCreate,
                        CanView = f.CanView,
                        CanEdit = f.CanEdit,
                        CanDelete = f.CanDelete,

                    }).ToList(),

                    QuestionText = c.QuestionText,
                    IsDefaultVariableType = c.Variable.IsDefaultVariable,

                }).ToList(),
                EntityTypes = entity.FormEntityTypes.Select(c => c.EntityType.Guid).ToList(),
                IsDefaultForm = entity.IsDefaultForm,
                FormDataEntryGuid = formfataentryguid != null ? formfataentryguid.Guid : (Guid?)null,
            };
        }
        public VariableViewModel TestEnvironment_ToActivityFormVariableModel(Variable model, int formId, int selectedValue)
        {
            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == model.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == model.Id).Select(ToVariableValidationRuleViewModel).ToList();
            if (model.VariableName == "AuthenticationMethod")
            {
                var authTypeId = dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                model.Values = string.Join("|", authTypeId.Select(x => x.id).ToList());
                model.VariableValueDescription = string.Join("|", authTypeId.Select(x => x.name).ToList());
            }

            ProjectBuilderJSONEntityValue formDataEntryVariablesJson = this.dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == selectedValue);
            var deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(formDataEntryVariablesJson.ProjectBuilderJSONValues);
            var formDataEntryVariables = deserialized.JsonFormDataEntryVariableList.FirstOrDefault(x => x.VariableId == model.Id);

            var variableEntityType = this.dbContext.VariableEntityTypes.FirstOrDefault(x => x.VariableId == model.Id);
            Guid? usernameGuid = null;

            #region authenticate-type
            if (model.VariableName == "Username")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValue : null;
            }

            if (model.VariableName == "Email")
            {
                var email = formDataEntryVariables != null ? formDataEntryVariables.SelectedValue : null;
            }
            #endregion

            if (model.VariableType.Type == "LKUP")
            {
                #region entity type drop-down
                var entType = model.VariableEntityTypes.FirstOrDefault();
                int entTypeid = entType != null ? entType.EntityTypeId : 0;
                var entTypes = dbContext.EntityTypes.FirstOrDefault(x => x.Id == entTypeid);
                int? entsubtype = null;
                string entsubtypeID = string.Empty;
                if (model.VariableEntityTypes.Count == 1)
                {
                    entsubtype = entType != null ? entType.EntitySubTypeId : (int?)null;
                }

                string entTypeName = entTypes != null ? entTypes.Name : string.Empty;
                if (entTypeName == "Person") { entTypeName = "Person Registration"; }
                else if (entTypeName == "Project") { entTypeName = "Project Registration"; }
                else if (entTypeName == "Participant") { entTypeName = "Participant Registration"; }
                else if (entTypeName == "Place/Group") { entTypeName = "Place/Group Registration"; }


                List<string> entityValues = new List<string>();
                List<string> entityText = new List<string>();

                var getAllformWithentTypeid = dbContext.FormEntityTypes.Where(x => x.EntityTypeId == entTypeid).Select(x => x.FormId).ToList();
                var formDataEntryEntityTypesList = dbContext.FormDataEntries.Where(t => getAllformWithentTypeid.Contains((int)t.FormId)).ToList();
                if (formDataEntryEntityTypesList.Count() > 0)
                {
                    foreach (var proj in formDataEntryEntityTypesList)
                    {
                        var text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                        var valuse = proj.Guid;

                        if (entsubtype != null)
                        {
                            text = null;
                            if (entTypeName == "Person Registration")
                            {
                                var persuntype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "PerSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (persuntype == entSubTyp)
                                    {
                                        var text1 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                                        var text2 = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                        text = text1 + " " + text2;
                                    }
                                }
                            }
                            else if (entTypeName == "Project Registration")
                            {
                                var projectsubtype = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                                var entSubTyp = entsubtype != null ? entsubtype.ToString() : string.Empty;
                                if (!string.IsNullOrEmpty(entSubTyp))
                                {
                                    if (projectsubtype == entSubTyp)
                                    {
                                        text = proj.FormDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                                    }
                                }
                            }
                        }
                        if (text != null)
                        {
                            entityValues.Add(valuse.ToString());
                            entityText.Add(text);
                        }
                    }
                    model.Values = entityValues != null ? string.Join("|", entityValues) : string.Empty;
                    model.VariableValueDescription = entityText != null ? string.Join("|", entityText) : string.Empty;
                }
                if (entTypeName == "Project Registration")
                {
                    List<string> entityValues1 = new List<string>();
                    List<string> entityText1 = new List<string>();

                    var projects = dbContext.ProjectBuilderJSONs.ToList();
                    foreach (var proj in projects)
                    {
                        var jsonDes = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(proj.ProjectBuilderJSONData);

                        entityValues1.Add(jsonDes.ProjectGuid.ToString());
                        entityText1.Add(jsonDes.ProjectName);
                    }
                    model.Values = entityValues1 != null ? string.Join("|", entityValues1) : string.Empty;
                    model.VariableValueDescription = entityText1 != null ? string.Join("|", entityText1) : string.Empty;
                }
                #endregion entity type drop-down end
            }

            return new VariableViewModel()
            {
                Guid = model.Guid,
                VariableName = model.VariableName,
                Id = model.Id,
                CanCollectMultiple = model.CanCollectMultiple,
                HelpText = model.HelpText,
                IsApproved = model.IsApproved,
                IsRequired = model.IsRequired,
                IsSoftRange = model.IsSoftRange,
                MaxRange = model.MaxRange,
                MinRange = model.MinRange,
                Question = model.Question,
                RegEx = model.RegEx,
                RequiredMessage = model.RequiredMessage,
                ValidationMessage = model.ValidationMessage,

                ValueDescription = model.ValueDescription,
                Values = model.Values.Split('|').ToList(),

                VariableLabel = model.VariableLabel,
                ModifiedDate = model.ModifiedDate,
                DateDeactivated = model.DateDeactivated,
                VariableRoles = model.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = model.VariableType.Type,
                Comment = model.Comment,

                ValidationRuleIds = variableValidationRuleGuidIds,
                variableValidationRuleViewModel = listOfRule,
                VariableValueDescription = model.VariableValueDescription != null ? model.VariableValueDescription.Split('|').ToList() : null,
                VariableSelectedValues = formDataEntryVariables != null ? formDataEntryVariables.SelectedValue : "",

                LookupEntityType = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Guid : (Guid?)null : (Guid?)null,
                LookupEntitySubtype = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Guid : (Guid?)null : (Guid?)null,

                LookupEntityTypeName = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Name : null : null,
                LookupEntitySubtypeName = variableEntityType != null ? variableEntityType.EntitySubType != null ? variableEntityType.EntitySubType.Name : null : null,
                CanFutureDate = model.CanFutureDate,
                UserNameVariableGuid = usernameGuid,
            };
        }

    }
}
