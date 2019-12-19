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
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Aspree.Provider.Provider
{
    public class VariableProvider : IVariableProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;

        private readonly IVariableCategoryProvider _VariableCategoryProvider;
        private readonly IValidationRuleProvider _ValidationRuleProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly IVariableTypeProvider _VariableTypeProvider;
        private readonly MongoDBContext _mongoDBContext;

        public VariableProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, IVariableCategoryProvider variableCategoryProvider, IValidationRuleProvider validationRuleProvider, IRoleProvider roleProvider, IVariableTypeProvider variableTypeProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._VariableCategoryProvider = variableCategoryProvider;
            this._ValidationRuleProvider = validationRuleProvider;
            this._RoleProvider = roleProvider;
            this._VariableTypeProvider = variableTypeProvider;
            this._mongoDBContext = mongoDBContext;
        }

        public VariableViewModel Create(VariableViewModel model)
        {
            if (dbContext.Variables.Any(et => et.VariableName.ToLower() == model.VariableName.ToLower() && et.DateDeactivated == null))
            {
                throw new Core.AlreadyExistsException("Variable already exists.");
            }

            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var dependentVarId = this.dbContext.Variables.FirstOrDefault(et => et.Guid == model.DependentVariableId);
            var validationRule = this.dbContext.ValidationRules.FirstOrDefault(et => et.Guid == model.ValidationRuleId);
            var variableCategory = this.dbContext.VariableCategories.FirstOrDefault(et => et.Guid == model.VariableCategoryId);
            var variableType = this.dbContext.VariableTypes.FirstOrDefault(et => et.Guid == model.VariableTypeId);
            var tenant = this.dbContext.Tenants.FirstOrDefault(et => et.Guid == model.TenantId);


            if (variableType.Type == "Dropdown" || variableType.Type == "Checkbox")
            {
                if (model.Values.Count == 0)
                {
                    throw new Core.AlreadyExistsException("Please add value and description.");
                }
            }
            else
            {
                model.Values.Clear();
                model.VariableValueDescription.Clear();
            }
            if (variableType.Type != "LKUP")
            {
                model.LookupEntityType = null;
                model.LookupEntitySubtype = null;
            }
            if (variableType.Type != "Numeric (Integer)" && variableType.Type != "Numeric (Decimal)" && variableType.Type != "Date")
            {
                model.OutsideRangeValidation = null;
                model.MissingValidation = null;
                model.MinRange = 0;
                model.MaxRange = 500;
            }

            var variable = new Variable()
            {
                Guid = Guid.NewGuid(),
                VariableName = model.VariableName,
                CanCollectMultiple = model.CanCollectMultiple,
                DependentVariableId = dependentVarId != null ? dependentVarId.Id : (int?)null,
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
                Comment = model.Comment,
                ValidationRuleId = validationRule != null ? validationRule.Id : (int?)null,
                ValueDescription = model.ValueDescription,
                Values = model.Values != null ? string.Join("|", model.Values) : string.Empty,
                VariableCategoryId = variableCategory != null ? variableCategory.Id : (int?)null,
                VariableLabel = model.VariableLabel,
                VariableTypeId = variableType != null ? variableType.Id : 0,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                TenantId = tenant.Id,
                VariableValueDescription = model.VariableValueDescription != null ? string.Join("|", model.VariableValueDescription) : string.Empty,

                IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Custom,
                CanFutureDate = variableType.Type == "Date" ? model.CanFutureDate : (bool?)null,
            };

            dbContext.Variables.Add(variable);

            SaveChanges();

            foreach (var item in model.Values)
            {
                dbContext.VariableValues.Add(new VariableValue()
                {
                    Guid = Guid.NewGuid(),
                    Text = item,
                    Value = item,
                    VariableId = variable.Id
                });
            }

            var roles = this.dbContext.Roles
                .Where(r => model.VariableRoles.Contains(r.Guid))
                .ToArray();

            foreach (var role in roles)
            {
                dbContext.VariableRoles.Add(new VariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = role.Id,
                    VariableId = variable.Id
                });
            }


            #region add in variable validation  table

            var allValidationRules = this.dbContext.ValidationRules;

            if (variableType.Type == "Numeric (Integer)")
            {
                model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Numeric").Guid);
                if (model.MaxRange != null || model.MinRange != null)
                    model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Range").Guid);
            }
            if (variableType.Type == "Numeric (Decimal)")
            {
                model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Decimal").Guid);
                if (model.MaxRange != null || model.MinRange != null)
                    model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Range").Guid);
            }
            if (variableType.Type == "Date")
            {

                switch (model.DateFormat)
                {
                    case "MMDDYYYY":
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMDDYYYY").Guid);
                        break;
                    case "MMYYYY":
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMYYYY").Guid);
                        break;
                    case "YYYY":
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_YYYY").Guid);
                        break;
                    case "DDMMMYYYY":
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_DDMMMYYYY").Guid);
                        break;
                    case "MMMYYYY":
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMMYYYY").Guid);
                        break;
                    default:
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date").Guid);
                        break;
                }

            }



            IEnumerable<VariableValidationRule> list = dbContext.VariableValidationRules.Where(x => x.VariableId == variable.Id).ToList();
            // Use Remove Range function to delete all records at once
            dbContext.VariableValidationRules.RemoveRange(list);
            foreach (var variableValidationGuid in model.ValidationRuleIds)
            {
                var validationRules = allValidationRules.FirstOrDefault(x => x.Guid == variableValidationGuid);
                string msg = "";
                string RegEx = "";

                var limitType = "";
                if (variableType.Type == "Numeric (Integer)" || variableType.Type == "Numeric (Decimal)")
                {
                    limitType = "Range";
                }

                if (variableValidationGuid == new Guid())
                {
                    msg = model.ValidationMessage;
                    RegEx = model.CustomRegEx;
                }
                else
                {
                    msg = validationRules != null ? validationRules.ErrorMessage : null;
                    RegEx = validationRules != null ? validationRules.RegExRule.RegEx : null;
                }
                dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = limitType,

                    Max = msg == "Range" || msg == "Length" ? (double?)model.MaxRange : validationRules != null ? validationRules.MaxRange != null ? (double?)validationRules.MaxRange : (double?)null : (double?)null,
                    Min = msg == "Range" || msg == "Length" ? (double?)model.MinRange : validationRules != null ? validationRules.MinRange != null ? (double?)validationRules.MinRange : (double?)null : (double?)null,
                   
                    RegEx = RegEx,
                    ValidationId = validationRules != null ? validationRules.Id : (int?)null,
                    ValidationMessage = validationRules != null ? validationRules.RuleType == "Range" ? model.OutsideRangeValidation : msg : "Invalid",
                    VariableId = variable.Id,
                });


            }
            #endregion

            #region for lookup variable entities
             
            if (variableType.Type == "LKUP")
            {
                if (model.LookupEntityType != null)
                {
                    var entityName = this.dbContext.EntityTypes.Where(x => x.Guid == model.LookupEntityType).Select(x => x.Name).FirstOrDefault();

                    var entityType = this.dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.LookupEntityType);

                    if (model.LookupEntitySubtype == new Guid())
                    {
                        var entitySubtype = this.dbContext.EntitySubTypes.Where(x => x.EntityType.Guid == model.LookupEntityType).ToList();
                        foreach (var subtype in entitySubtype)
                        {
                            dbContext.VariableEntityTypes.Add(new VariableEntityType()
                            {
                                Guid = Guid.NewGuid(),
                                EntityTypeId = entityType != null ? entityType.Id : 1,
                                EntitySubTypeId = subtype != null ? subtype.Id : (int?)null,
                                VariableId = variable.Id
                            });
                        }
                        if (entityName == EntityTypesListInDB.Consumer_Group.ToString().Replace("_", " ") && entitySubtype != null)
                        {
                            var firstEntitySubtype = entitySubtype.FirstOrDefault();
                            dbContext.VariableEntityTypes.Add(new VariableEntityType()
                            {
                                Guid = Guid.NewGuid(),
                                EntityTypeId = entityType != null ? entityType.Id : 1,
                                EntitySubTypeId = firstEntitySubtype != null ? firstEntitySubtype.Id : (int?)null,
                                VariableId = variable.Id
                            });
                        }
                    }
                    else
                    {
                        var entitySubtype = this.dbContext.EntitySubTypes.FirstOrDefault(x => x.Guid == model.LookupEntitySubtype);
                        dbContext.VariableEntityTypes.Add(new VariableEntityType()
                        {
                            Guid = Guid.NewGuid(),
                            EntityTypeId = entityType != null ? entityType.Id : 1,
                            EntitySubTypeId = entitySubtype != null ? entitySubtype.Id : (int?)null,
                            VariableId = variable.Id
                        });
                    }
                }
            }
            #endregion


            return ToModel(variable);
        }

        public VariableViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var variable = dbContext.Variables.FirstOrDefault(fs => fs.Guid == guid);
            if (variable != null)
            {
                variable.DeactivatedBy = deactivatedBy.Id;
                variable.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(variable);
            }

            return null;
        }

        public VariableViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var variable = dbContext.Variables.FirstOrDefault(fs => fs.Id == id);
            if (variable != null)
            {
                variable.DeactivatedBy = deactivatedBy.Id;
                variable.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(variable);
            }

            return null;
        }

        public IEnumerable<VariableViewModel> GetAll()
        {
            return dbContext.Variables
             .Select(ToModel)
             .ToList();
        }

        public IEnumerable<VariableViewModel> GetAll(Guid tenantId)
        {
            return dbContext.Variables
             .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId)
             .Select(ToModel)
             .ToList();
        }

        public VariableViewModel GetByGuid(Guid guid)
        {
            var entity = dbContext.Variables
               .FirstOrDefault(fs => fs.Guid == guid);

            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public VariableViewModel GetVariablesByGuid(Guid guid, Guid logginuserId, Guid projectid)
        {
            var Variable = dbContext.Variables
              .FirstOrDefault(fs => fs.Guid == guid);

            if (Variable != null)
                return ToNewModel(Variable, logginuserId, projectid);

            return null;
        }

        public VariableViewModel GetById(int id)
        {
            var entity = dbContext.Variables
              .FirstOrDefault(fs => fs.Id == id);

            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public VariableViewModel ToModel(Variable entity)
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

            if (entity.VariableName == Core.Enum.DefaultsVariables.AuthenticationMethod.ToString())
            {
                List<string> valueList = new List<string>();
                List<string> descriptionList = new List<string>();

                valueList = new List<string>();
                descriptionList = new List<string>();
                var authTypeId = dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).OrderByDescending(x => x.Id).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                valueList = authTypeId.Select(x => x.id).ToList();
                descriptionList = authTypeId.Select(x => x.name).ToList();

                entity.Values = string.Join("|", valueList);
                entity.VariableValueDescription = string.Join("|", descriptionList);
            }

            if (entity.VariableName == Core.Enum.DefaultsVariables.ProRole.ToString())
            {
                List<string> valueList = new List<string>();
                List<string> descriptionList = new List<string>();

                valueList = new List<string>();
                descriptionList = new List<string>();
                var proRoleId = dbContext.Roles.Where(x => x.DateDeactivated == null && x.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")).OrderByDescending(x => x.Id).Select(x => new { id = x.Id, name = x.Name }).ToList();
                valueList = proRoleId.Select(x => x.id.ToString()).ToList();
                descriptionList = proRoleId.Select(x => x.name).ToList();

                entity.Values = string.Join("|", valueList);
                entity.VariableValueDescription = string.Join("|", descriptionList);
            }

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
                usedInFormsList = dbContext.FormVariables.Where(x => x.VariableId == entity.Id).Select(x => x.Form.Guid).ToList();
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

        public VariableViewModel ToNewModel(Variable entity, Guid? loggedUserGuid = null, Guid? Projectid = null)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            var userRole = "";
            if (loggedUserGuid.HasValue && Projectid.HasValue)
            {
                userRole = _userLoginProvider.GetUserRoleByProjectId(loggedUserGuid.Value, Projectid.Value);
            }
            var dependentVarId = this.dbContext.Variables.FirstOrDefault(et => et.Id == entity.DependentVariableId);
            var validationRule = this.dbContext.ValidationRules.FirstOrDefault(et => et.Id == entity.ValidationRuleId);
            var variableCategory = this.dbContext.VariableCategories.FirstOrDefault(et => et.Id == entity.VariableCategoryId);
            var variableType = this.dbContext.VariableTypes.FirstOrDefault(et => et.Id == entity.VariableTypeId);

            var ValidationRuleIds = this.dbContext.VariableValidationRules.Where(v => v.VariableId == entity.Id).Select(x => x.ValidationId).ToList();
            var variableValidationRuleGuidIds = this.dbContext.ValidationRules
                .Where(r => ValidationRuleIds.Contains(r.Id)).Select(x => x.Guid)
                .ToList();


            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == entity.Id).Select(ToVariableValidationRuleViewModel).ToList();

            if (entity.VariableName == Core.Enum.DefaultsVariables.AuthenticationMethod.ToString())
            {
                List<string> valueList = new List<string>();
                List<string> descriptionList = new List<string>();

                valueList = new List<string>();
                descriptionList = new List<string>();
                var authTypeId = dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null).OrderByDescending(x => x.Id).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).ToList();
                valueList = authTypeId.Select(x => x.id).ToList();
                descriptionList = authTypeId.Select(x => x.name).ToList();

                entity.Values = string.Join("|", valueList);
                entity.VariableValueDescription = string.Join("|", descriptionList);
            }

            if (entity.VariableName == Core.Enum.DefaultsVariables.ProRole.ToString())
            {
                List<string> valueList = new List<string>();
                List<string> descriptionList = new List<string>();

                valueList = new List<string>();
                descriptionList = new List<string>();
                var proRoleId = dbContext.Roles.Where(x => x.DateDeactivated == null && x.Name != Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")).OrderByDescending(x => x.Id).Select(x => new { id = x.Id, name = x.Name }).ToList();
                valueList = proRoleId.Select(x => x.id.ToString()).ToList();
                descriptionList = proRoleId.Select(x => x.name).ToList();

                entity.Values = string.Join("|", valueList);
                entity.VariableValueDescription = string.Join("|", descriptionList);
            }

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
                usedInFormsList = dbContext.FormVariables.Where(x => x.VariableId == entity.Id).Select(x => x.Form.Guid).ToList();
            }
            catch (Exception ex) { }


            var allForms = dbContext.Forms.Where(c => usedInFormsList.Contains(c.Guid));

            var activityStatusId = allForms.Any(x => x.ActivityForms.Any(y => y.Activity.ActivityStatusId == (int)ActivityStatusTypes.Active));

            return new VariableViewModel()
            {
                Guid = entity.Guid,
                VariableName = entity.VariableName,
                Id = entity.Id,
                CanCollectMultiple = entity.CanCollectMultiple,
                DependentVariableId = entity.DependentVariableId.HasValue ? dependentVarId.Guid : (Guid?)null,
                HelpText = entity.HelpText,
                IsApproved = entity.IsApproved,
                UserTypeRole = userRole,
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

                ActivityStatusId = activityStatusId ? (int)ActivityStatusTypes.Active : (int)ActivityStatusTypes.Draft,
            };
        }

        public IEnumerable<VariableViewModel> GetAllVariables(Guid tenantId, Guid projectId)
        {
            string[] defaultEntityTypes = new string[]
            {
                EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration),
                EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration),
                EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration ),
                EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration),
            };
            IMongoQuery conditionUserEntitiesCustomForms = Query.Or(
                    Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                    , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    , Query<FormDataEntryMongo>.EQ(q => q.FormTitle, EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                    );

            List<VariableViewModel> variableList = new List<VariableViewModel>();
            List<Variable> variables = dbContext.Variables
             .Where(v => v.TenantId.HasValue && v.Tenant.Guid == tenantId && v.DateDeactivated == null).ToList();
            IQueryable<FormDataEntryMongo> allEntities = null;
            IQueryable<EntityType> dbContext_EntityTypes = null;
            IQueryable<EntitySubType> entitySubtypes = null;
            if (variables != null && variables.Any(cc => cc.VariableType.Type == VariableTypes.LKUP.ToString()))
            {
                allEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionUserEntitiesCustomForms).AsQueryable();

                dbContext_EntityTypes = dbContext.EntityTypes.AsQueryable();
                entitySubtypes = dbContext.EntitySubTypes.AsQueryable();
            }
            int variableCnt = 0;
            if (variables != null)
            {
                while (variableCnt < variables.Count())
                {
                    if (variables[variableCnt] != null)
                    {
                        variableList.Add(ToVariableModel(variables[variableCnt], projectId, allEntities, dbContext_EntityTypes, entitySubtypes));
                    }
                    variableCnt++;
                }
            }
            return variableList;
        }
        public VariableViewModel ToVariableModel(Variable entity
            , Guid projectId
            , IQueryable<FormDataEntryMongo> allEntities
            , IQueryable<EntityType> dbContext_EntityTypes
            , IQueryable<EntitySubType> entitySubtypes
            )
        {
            Guid dependentVarId = this.dbContext.Variables.Where(et => et.Id == entity.DependentVariableId).Select(c => c.Guid).FirstOrDefault();

            var listOfRule = this.dbContext.VariableValidationRules.Where(x => x.VariableId == entity.Id).Select(ToVariableValidationRuleViewModel).ToList();


            var variableEntityType = entity.VariableEntityTypes.FirstOrDefault();
            var entitysubtype = entity.VariableEntityTypes.ToList();

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
                if (!entity.CanCollectMultiple)
                    usedInFormsList = entity.FormVariables.Where(x => x.VariableId == entity.Id && x.Form.FormDataEntry.Guid == projectId).Select(x => x.Form.Guid).ToList();
            }
            catch (Exception ex) { }


            List<String> valueLKUP = new List<string>();
            try
            {
                if (entity.VariableType.Type == VariableTypes.LKUP.ToString() && entity.IsDefaultVariable == (int)Core.Enum.DefaultVariableType.Custom)
                {
                    valueLKUP = GetVariableValues(entity, allEntities, dbContext_EntityTypes, entitySubtypes).Select(x => x.Key).FirstOrDefault();
                }
            }
            catch (Exception ex) { }
            return new VariableViewModel()
            {
                Guid = entity.Guid,
                VariableName = entity.VariableName,
                Id = entity.Id,
                CanCollectMultiple = entity.CanCollectMultiple,
                DependentVariableId = dependentVarId,
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
                ValueDescription = entity.ValueDescription,

                Values = (entity.VariableType.Type == VariableTypes.LKUP.ToString()) ? valueLKUP : entity.Values.Split('|').ToList(),
                VariableValueDescription = (entity.VariableType.Type == VariableTypes.LKUP.ToString()) ? valueLKUP : entity.VariableValueDescription.Split('|').ToList(),

                VariableCategoryId = entity.VariableCategory != null ? entity.VariableCategory.Guid : (Guid?)null,
                VariableLabel = entity.VariableLabel,
                VariableTypeId = entity.VariableType != null ? entity.VariableType.Guid : Guid.Empty,
                ModifiedDate = entity.ModifiedDate,
                DateDeactivated = entity.DateDeactivated,
                VariableRoles = entity.VariableRoles.Select(r => r.Role.Guid).ToList(),
                VariableTypeName = entity.VariableType != null ? entity.VariableType.Type : null,
                Comment = entity.Comment,
                variableValidationRuleViewModel = listOfRule,

                IsDefaultVariable = entity.IsDefaultVariable,
                LookupEntityType = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Guid : (Guid?)null : (Guid?)null,
                LookupEntitySubtype = LookupEntitySubtype,

                LookupEntityTypeName = variableEntityType != null ? variableEntityType.EntityType != null ? variableEntityType.EntityType.Name : null : null,
                LookupEntitySubtypeName = LookupEntitySubtypeName,
                CanFutureDate = entity.CanFutureDate,

                VariableUsedInFormsList = usedInFormsList,
            };
        }
        public VariableViewModel Update(VariableViewModel model)
        {
            if (dbContext.Variables.Any(et => et.VariableName.ToLower() == model.VariableName.ToLower() && et.DateDeactivated == null
            && et.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Variable already exists.");
            }

            if (dbContext.Variables.Any(et => et.Guid == model.Guid
            && et.IsDefaultVariable == (int)Core.Enum.DefaultVariableType.Default))
            {
                throw new Core.AlreadyExistsException("Default variables can not modified.");
            }

            var variable = dbContext.Variables
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            var dependentVarId = this.dbContext.Variables.FirstOrDefault(et => et.Guid == model.DependentVariableId);
            var validationRule = this.dbContext.ValidationRules.FirstOrDefault(et => et.Guid == model.ValidationRuleId);
            var variableCategory = this.dbContext.VariableCategories.FirstOrDefault(et => et.Guid == model.VariableCategoryId);
            var variableType = this.dbContext.VariableTypes.FirstOrDefault(et => et.Guid == model.VariableTypeId);


            if (variableType.Type == "Dropdown" || variableType.Type == "Checkbox")
            {
                if (model.Values.Count == 0)
                {
                    throw new Core.AlreadyExistsException("Please add value and description.");
                }
            }
            else
            {
                model.Values.Clear();
                model.VariableValueDescription.Clear();
            }
            if (variableType.Type != "LKUP")
            {
                model.LookupEntityType = null;
                model.LookupEntitySubtype = null;
            }
            if (variableType.Type != "Numeric (Integer)" && variableType.Type != "Numeric (Decimal)" && variableType.Type != "Date")
            {
                model.OutsideRangeValidation = null;
                model.MissingValidation = null;
                model.MinRange = 0;
                model.MaxRange = 500;
            }
            var tenant = this.dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);
            if (variable != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);
                if (modifiedBy != null)
                {
                    if (modifiedBy.RoleName != Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                    {
                        model.IsApproved = false;
                        model.Comment = string.Empty;
                    }
                }
                variable.VariableName = model.VariableName;
                variable.CanCollectMultiple = model.CanCollectMultiple;
                variable.DependentVariableId = dependentVarId != null ? dependentVarId.Id : (int?)null;
                variable.HelpText = model.HelpText;
                variable.IsApproved = model.IsApproved;
                variable.IsRequired = model.IsRequired;
                variable.IsSoftRange = model.IsSoftRange;
                variable.MaxRange = model.MaxRange;
                variable.MinRange = model.MinRange;
                variable.Question = model.Question;
                variable.RegEx = model.RegEx;
                variable.RequiredMessage = model.RequiredMessage;
                variable.ValidationMessage = model.ValidationMessage;
                variable.ValidationRuleId = validationRule != null ? validationRule.Id : (int?)null;
                variable.ValueDescription = model.ValueDescription;
                variable.Values = string.Join("|", model.Values);
                variable.VariableCategoryId = variableCategory != null ? variableCategory.Id : (int?)null;
                variable.VariableLabel = model.VariableLabel;
                variable.VariableTypeId = variableType != null ? variableType.Id : 0;
                variable.ModifiedBy = modifiedBy.Id;
                variable.ModifiedDate = DateTime.UtcNow;
                variable.Comment = model.Comment;
                variable.VariableValueDescription = string.Join("|", model.VariableValueDescription);
                variable.CanFutureDate = variableType.Type == "Date" ? model.CanFutureDate : (bool?)null;
                dbContext.VariableValues.RemoveRange(variable.VariableValues.Select(c => c));
                dbContext.VariableRoles.RemoveRange(variable.VariableRoles.Select(c => c));

                foreach (var item in model.Values)
                {
                    dbContext.VariableValues.Add(new VariableValue()
                    {
                        Guid = Guid.NewGuid(),
                        Text = item,
                        Value = item,
                        VariableId = variable.Id
                    });
                }

                var roles = this.dbContext.Roles
                .Where(r => model.VariableRoles.Contains(r.Guid))
                .ToArray();

                foreach (var role in roles)
                {
                    dbContext.VariableRoles.Add(new VariableRole()
                    {
                        Guid = Guid.NewGuid(),
                        RoleId = role.Id,
                        VariableId = variable.Id
                    });
                }

                #region add in variable validation  table
                IEnumerable<VariableValidationRule> list = dbContext.VariableValidationRules.Where(x => x.VariableId == variable.Id).ToList();
                // Use Remove Range function to delete all records at once
                dbContext.VariableValidationRules.RemoveRange(list);

                var allValidationRules = this.dbContext.ValidationRules;

                if (variableType.Type == Core.Enum.VariableTypes.FileType.ToString())
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.ColorPicker.ToString())
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.Checkbox.ToString())
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.Free_Text.ToString().Replace("_", " "))
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.LKUP.ToString())
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.Text_Box.ToString().Replace("_", " "))
                {
                    model.ValidationRuleIds = new List<Guid>();
                }
                if (variableType.Type == Core.Enum.VariableTypes.Dropdown.ToString())
                {
                    model.ValidationRuleIds = new List<Guid>();
                }

                if (variableType.Type == "Numeric (Integer)")
                {
                    model.ValidationRuleIds = new List<Guid>();
                    if (model.ValidationRuleIds.Count == 0)
                    {
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Numeric").Guid);
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Range").Guid);
                    }
                }
                if (variableType.Type == "Numeric (Decimal)")
                {
                    model.ValidationRuleIds = new List<Guid>();
                    if (model.ValidationRuleIds.Count == 0)
                    {
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Decimal").Guid);
                        model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Range").Guid);
                    }
                }

                if (variableType.Type == "Date")
                {
                    model.ValidationRuleIds = new List<Guid>();
                    switch (model.DateFormat)
                    {
                        case "MMDDYYYY": 
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMDDYYYY").Guid);
                            break;
                        case "MMYYYY":
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMYYYY").Guid);
                            break;
                        case "YYYY":
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_YYYY").Guid);
                            break;
                        case "DDMMMYYYY":
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_DDMMMYYYY").Guid);
                            break;
                        case "MMMYYYY":
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date_MMMYYYY").Guid);
                            break;
                        default:
                            model.ValidationRuleIds.Add(allValidationRules.FirstOrDefault(x => x.RuleType == "Date").Guid);
                            break;
                    }

                }
                foreach (var variableValidationGuid in model.ValidationRuleIds)
                {
                    var validationRules = allValidationRules.FirstOrDefault(x => x.Guid == variableValidationGuid);

                    string msg = "";
                    string RegEx = "";

                    var limitType = "";
                    if (variableType.Type == "Numeric (Integer)" || variableType.Type == "Numeric (Decimal)")
                    {
                        limitType = "Range";
                    }

                    if (variableValidationGuid == new Guid())
                    {
                        msg = model.ValidationMessage;
                        RegEx = model.CustomRegEx;
                    }
                    else
                    {
                        msg = validationRules.ErrorMessage;
                        RegEx = validationRules != null ? validationRules.RegExRule.RegEx : null;
                    }

                    dbContext.VariableValidationRules.Add(new VariableValidationRule()
                    {
                        Guid = Guid.NewGuid(),
                        LimitType = limitType,

                        Max = msg == "Range" || msg == "Length" ? (double?)model.MaxRange : validationRules != null ? validationRules.MaxRange != null ? (double?)validationRules.MaxRange : (double?)null : (double?)null,
                        Min = msg == "Range" || msg == "Length" ? (double?)model.MinRange : validationRules != null ? validationRules.MinRange != null ? (double?)validationRules.MinRange : (double?)null : (double?)null,
                        RegEx = RegEx,
                        ValidationId = validationRules != null ? validationRules.Id : (int?)null,
                        ValidationMessage = validationRules.RuleType == "Range" ? model.OutsideRangeValidation : msg,
                        VariableId = variable.Id,
                    });
                }
                #endregion


                if (model.IsVariableLogTable)
                {
                    dbContext.VariableApprovalLogs.Add(new VariableApprovalLog()
                    {
                        Guid = Guid.NewGuid(),
                        VariableId = variable.Id,
                        CommentText = variable.Comment,
                        CreatedBy = modifiedBy.Id,
                        CreatedDate = DateTime.UtcNow,
                        TenantId = tenant != null ? tenant.Id : (int?)null,
                    });
                }



                #region for lookup variable entities
                if (variableType.Type == "LKUP")
                {
                    if (model.LookupEntityType != null)
                    {
                        var entityType = this.dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.LookupEntityType);

                        IEnumerable<VariableEntityType> variableEntityTypes = this.dbContext.VariableEntityTypes.Where(x => x.VariableId == variable.Id).ToList();
                        dbContext.VariableEntityTypes.RemoveRange(variableEntityTypes);

                        if (model.LookupEntitySubtype == Guid.Empty)
                        {
                            var entityName = this.dbContext.EntityTypes.Where(x => x.Guid == model.LookupEntityType).Select(x=>x.Name).FirstOrDefault();
                            var entitySubtype = this.dbContext.EntitySubTypes.Where(x => x.EntityType.Guid == model.LookupEntityType).ToList();
                            foreach (var subtype in entitySubtype)
                            {
                                dbContext.VariableEntityTypes.Add(new VariableEntityType()
                                {
                                    Guid = Guid.NewGuid(),
                                    EntityTypeId = entityType != null ? entityType.Id : 1,
                                    EntitySubTypeId = subtype != null ? subtype.Id : (int?)null,
                                    VariableId = variable.Id
                                });
                                
                            }
                            if (entityName == EntityTypesListInDB.Consumer_Group.ToString().Replace("_", " ") && entitySubtype != null)
                            {
                                var firstEntitySubtype = entitySubtype.FirstOrDefault();
                                dbContext.VariableEntityTypes.Add(new VariableEntityType()
                                {
                                    Guid = Guid.NewGuid(),
                                    EntityTypeId = entityType != null ? entityType.Id : 1,
                                    EntitySubTypeId = firstEntitySubtype != null ? firstEntitySubtype.Id : (int?)null,
                                    VariableId = variable.Id
                                });
                            }
                        }
                        else
                        {
                            var entitySubtype = this.dbContext.EntitySubTypes.FirstOrDefault(x => x.Guid == model.LookupEntitySubtype);
                            dbContext.VariableEntityTypes.Add(new VariableEntityType()
                            {
                                Guid = Guid.NewGuid(),
                                EntityTypeId = entityType != null ? entityType.Id : 1,
                                EntitySubTypeId = entitySubtype != null ? entitySubtype.Id : (int?)null,
                                VariableId = variable.Id
                            });
                        }
                    }
                }
                #endregion

                SaveChanges();

                return ToModel(variable);
            }

            return null;
        }

        public ProjectBuilderVariablesViewModel GetProjectBuilderVariables(Guid tenantId, Guid LoggedInUserId, Guid projectId)
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
            ProjectBuilderVariablesViewModel projectBuilderVariablesViewModel = new ProjectBuilderVariablesViewModel();

            projectBuilderVariablesViewModel.VariableCategory = _VariableCategoryProvider.GetAll(tenantId);
            projectBuilderVariablesViewModel.Role = _RoleProvider.GetAll(tenantId);
            projectBuilderVariablesViewModel.VariableType = _VariableTypeProvider.GetAll();
            projectBuilderVariablesViewModel.ValidationRule = _ValidationRuleProvider.GetAll();
            try
            {
                projectBuilderVariablesViewModel.LookupVariablesPreviewViewModelList = GetLookupVariablesPreview();
            }
            catch (Exception exc) { }

            return projectBuilderVariablesViewModel;
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

        public IEnumerable<ProjectBuilderFormViewModelViewModel> GetFormVariableByProjectId(Guid projectId)
        {
            var project = this.dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == projectId);
            if (project != null)
            {
                return this.dbContext.Forms.Where(x => x.ProjectId == project.Id).Select(ToProjectBuilderFormViewModelViewModel).ToList();
            }
            else
            {
                return null;
            }
        }

        public ProjectBuilderFormViewModelViewModel ToProjectBuilderFormViewModelViewModel(Form form)
        {
            var FormVariables = form.FormVariables.Select(x => new FormVariableViewModel()
            {
                VariableId = x.Variable.Guid,
                VariableName = x.Variable.VariableName,
                FormVariableIsApprovedStatus = x.Variable.IsApproved,
            }).ToList();

            return new ProjectBuilderFormViewModelViewModel()
            {
                FormId = form.Id,
                FormName = form.FormTitle,
                FormStatusId = form.FormStatusId,
                FormVariables = FormVariables,
                ProjectId = form.ProjectId,
            };
        }


        public IDictionary<List<string>, List<string>> GetVariableValues(Variable variable
            , IQueryable<FormDataEntryMongo> allEntities
            , IQueryable<EntityType> dbContext_EntityTypes
            , IQueryable<EntitySubType> entitySubtypes
            )
        {
            List<string> valueList = new List<string>();
            List<string> descriptionList = new List<string>();
            if (variable.VariableType.Type == VariableTypes.LKUP.ToString())
            {
                #region LKUP drop-down
                valueList = new List<string>();
                descriptionList = new List<string>();

                var entType = variable.VariableEntityTypes.FirstOrDefault();
                int entTypeid = entType != null ? entType.EntityTypeId : 0;
                var entTypes = dbContext_EntityTypes.FirstOrDefault(x => x.Id == entTypeid);

                int? entsubtype = null;
                string entsubtypeID = string.Empty;
                if (variable.VariableEntityTypes.Count == 1)
                {
                    entsubtype = entType != null ? entType.EntitySubTypeId : (int?)null;
                }

                string entityType = entTypes != null ? entTypes.Name : string.Empty;
                string entTypeName = entityType;
                if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
                else if (entTypeName == "Project") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
                else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
                else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
                else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }

                var entities = allEntities.Where(x => x.ActivityName == entTypeName).AsQueryable();
                IQueryable<FormDataEntryMongo> person = null;
                IQueryable<FormDataEntryMongo> participant = null;
                IQueryable<FormDataEntryMongo> place__Group = null;
                IQueryable<FormDataEntryMongo> project = null;

                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                    person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                    project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();

                var entityTypes = dbContext_EntityTypes.AsQueryable();
                int id = 0;

                List<long?> entitiesMongo = new List<long?>();

                if (participant != null && participant.Count() > 0)
                {
                    participant.ToList().ForEach(parti =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string ln = LastName != null ? LastName.SelectedValues : string.Empty;

                        valueList.Add(fn + " " + ln);
                        descriptionList.Add(fn + " " + ln);
                    });
                }

                if (person != null && person.Count() > 0)
                {
                    string Medical_Practitioner__Allied_Healt1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
                    string Non_Medical__Practitioner1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Non_Medical__Practitioner);

                    EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Name == Medical_Practitioner__Allied_Healt1);
                    EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Name == Non_Medical__Practitioner1);

                    person.ToList().ForEach(per =>
               {
                   id++;
                   FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                   FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                   FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());

                   string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                   string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                   string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                   Guid entityTypeId = Guid.Empty;

                   var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                   Guid entitySubtypeId = Guid.Empty;

                   switch (ps)
                   {
                       case "1":
                           entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                           entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                           break;
                       case "2":
                           entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                           entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                           break;
                       default:
                           break;
                   }

                   valueList.Add(fn + " " + ln);
                   descriptionList.Add(fn + " " + ln);
               });
                }

                if (place__Group != null && place__Group.Count() > 0)
                {
                    string Public_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Overnight_Admissions);
                    string Public_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Day_Admissions_Only);
                    string Private_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Overnight_Admissions);
                    string Private_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Day_Admissions_Only);
                    string Specialist_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Specialist_Clinic);
                    string General_Practice1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Practice);
                    string Allied_Health_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Allied_Health_Clinic);
                    string General_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Laboratory);
                    string Genetics_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Genetics_Laboratory);

                    string State_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.State_Health_Network);
                    string National_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.National_Health_Network);
                    string Regulatory_Body_TGA1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Regulatory_Body_TGA);
                    string Industry_Peak_Body1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Industry_Peak_Body);
                    string Device_Manufacturer1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Device_Manufacturer);
                    string Clinical_Craft_Group_Society1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);

                    EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Public_Overnight_Admissions1);
                    EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Public_Day_Admissions_Only1);
                    EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Private_Overnight_Admissions1);
                    EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Private_Day_Admissions_Only1);
                    EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Specialist_Clinic1);
                    EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Name == General_Practice1);
                    EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Allied_Health_Clinic1);
                    EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == General_Laboratory1);
                    EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == Genetics_Laboratory1);
                    EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == State_Health_Network1);
                    EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == National_Health_Network1);
                    EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Name == Regulatory_Body_TGA1);
                    EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Name == Industry_Peak_Body1);
                    EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Name == Device_Manufacturer1);
                    EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Craft_Group_Society1);

                    if (entityType == "Place/Group")
                    {
                        EntityType placeentity = dbContext_EntityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Place__Group);

                        place__Group.ToList().ForEach(plc =>
                        {
                            id++;
                            FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                            FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                            string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                            string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                            Guid entityTypeId = (placeentity != null ? placeentity.Guid : Guid.Empty);
                            Guid entitySubtypeId = Guid.Empty;

                            valueList.Add(plName);
                            descriptionList.Add(plName);
                        });
                    }
                    else
                    {
                        EntityType placeentity = dbContext_EntityTypes.FirstOrDefault(x => x.Name == entityType);

                        int etype = placeentity != null ? placeentity.Id : 0;

                        place__Group.ToList().ForEach(plc =>
                        {
                            id++;
                            FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                            FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                            string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                            string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                            string enttypeFrm = GetDBEntTypeFromSelecedEntType(enttype);

                            Guid entityTypeId = Guid.Empty;
                            Guid entitySubtypeId = Guid.Empty;

                            int entTypeDB = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                            int entTypeDBFRM = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                            if (entTypeDB == etype)
                            {
                                #region Place-Group
                                switch (etype)
                                {
                                    case (int)EntityTypesListInDBSummary.Hospital:
                                        #region Hospital
                                        var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                        entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                                        var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                        if (hospSType == "1")
                                        {
                                            entitySubtypeId = Public_Overnight_Admissions.Guid;
                                        }
                                        else if (hospSType == "2")
                                        {
                                            entitySubtypeId = Public_Day_Admissions_Only.Guid;
                                        }
                                        else if (hospSType == "3")
                                        {
                                            entitySubtypeId = Private_Overnight_Admissions.Guid;
                                        }
                                        else if (hospSType == "4")
                                        {
                                            entitySubtypeId = Private_Day_Admissions_Only.Guid;
                                        }
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                       
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Practice__Clinic:
                                        #region Practice/Clinic
                                        var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                        entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                                        var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                        if (PracSType == "1")
                                        {
                                            entitySubtypeId = Specialist_Clinic.Guid;
                                        }
                                        else if (PracSType == "2")
                                        {
                                            entitySubtypeId = General_Practice.Guid;
                                        }
                                        else if (PracSType == "3")
                                        {
                                            entitySubtypeId = Allied_Health_Clinic.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Laboratory:
                                        #region Laboratory
                                        var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                        entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                                        var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                        if (LabSType == "1")
                                        {
                                            entitySubtypeId = General_Laboratory.Guid;
                                        }
                                        else if (LabSType == "2")
                                        {
                                            entitySubtypeId = Genetics_Laboratory.Guid;
                                        }
                                       
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Medical_Imaging:
                                        #region Medical imaging
                                        var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                        entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Research_facility__University:
                                        #region Research facility/University
                                        var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                        entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Healthcare_Group:
                                        #region Healthcare Group
                                        var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                        entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Government_Organisation:
                                        #region Government Organisation
                                        var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                        entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                                        var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                        if (GovSType == "1")
                                        {
                                            entitySubtypeId = State_Health_Network.Guid;
                                        }
                                        else if (GovSType == "2")
                                        {
                                            entitySubtypeId = National_Health_Network.Guid;
                                        }
                                        else if (GovSType == "3")
                                        {
                                            entitySubtypeId = Regulatory_Body_TGA.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Industry_Group:
                                        #region Industry Group
                                        var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                        entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                                        var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                        if (IndSType == "1")
                                        {
                                            entitySubtypeId = Industry_Peak_Body.Guid;
                                        }
                                        else if (IndSType == "2")
                                        {
                                            entitySubtypeId = Device_Manufacturer.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Consumer_Group:
                                        #region Consumer Group
                                        var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                        entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                                        var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                                        if (ConSType == "1")
                                        {
                                            entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Activity_Venue:
                                        #region Activity Venue
                                        var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                        entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Vehicle:
                                        #region Vehicle
                                        var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                        entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        
                                        #endregion
                                            
                                        break;
                                    case (int)EntityTypesListInDBSummary.MAC:
                                        #region MAC
                                        var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                        entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Ethics_Committee:
                                        #region Ethics Committee
                                        var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                        entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                       
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }

                        });
                    }
                }

                if (project != null && project.Count() > 0)
                {
                    string Registry1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Registry);
                    string Clinical_Trial1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Trial);
                    string Cohort_Study1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Cohort_Study);
                    string Other1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Other);

                    EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Name == Registry1);
                    EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Trial1);
                    EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Name == Cohort_Study1);
                    EntitySubType Other = entitySubtypes.FirstOrDefault(x => x.Name == Other1);

                    project.ToList().ForEach(proj =>
                {
                    id++;
                    FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                    string fn = Name != null ? Name.SelectedValues : string.Empty;

                    Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                    Guid entitySubtypeId = Guid.Empty;

                    FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                    string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                    switch (pr)
                    {
                        case "1":
                            entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
                            break;
                        case "2":
                            entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
                            break;

                        case "3":
                            entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
                            break;
                        default:
                            break;
                    }
                    valueList.Add(fn);
                    descriptionList.Add(fn);
                });
                }
                #endregion
            }

            IDictionary<List<string>, List<string>> resultValues = new Dictionary<List<string>, List<string>>();
            resultValues.Add(valueList, descriptionList);
            return resultValues;
        }



        public IDictionary<List<string>, List<string>> GetVariableValues_Optimized(Variable variable
        , IQueryable<FormDataEntryMongo> allEntities
        , IQueryable<EntityType> dbContext_EntityTypes
        , IQueryable<EntitySubType> entitySubtypes
        )
        {
            List<string> valueList = new List<string>();
            List<string> descriptionList = new List<string>();
            if (variable.VariableType.Type == VariableTypes.LKUP.ToString())
            {
                #region LKUP drop-down

                var entType = variable.VariableEntityTypes.FirstOrDefault();
                int entTypeid = entType != null ? entType.EntityTypeId : 0;
                var entTypes = dbContext_EntityTypes.FirstOrDefault(x => x.Id == entTypeid);

                int? entsubtype = null;
                string entsubtypeID = string.Empty;
                if (variable.VariableEntityTypes.Count == 1)
                {
                    entsubtype = entType != null ? entType.EntitySubTypeId : (int?)null;
                }

                string entityType = entTypes != null ? entTypes.Name : string.Empty;
                string entTypeName = entityType;
                if (entTypeName == "Person") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration); }
                else if (entTypeName == "Project") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration); }
                else if (entTypeName == "Participant") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration); }
                else if (entTypeName == "Place/Group") { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }
                else { entTypeName = EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration); }

                var entities = allEntities.Where(x => x.ActivityName == entTypeName).AsQueryable(); ;

                IQueryable<FormDataEntryMongo> person = null;
                IQueryable<FormDataEntryMongo> participant = null;
                IQueryable<FormDataEntryMongo> place__Group = null;
                IQueryable<FormDataEntryMongo> project = null;

                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                    person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();
                if (entTypeName == EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration))
                    project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();

                var entityTypes = dbContext_EntityTypes.AsQueryable();

                string Medical_Practitioner__Allied_Healt1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
                string Non_Medical__Practitioner1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Non_Medical__Practitioner);
                string Public_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Overnight_Admissions);
                string Public_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Day_Admissions_Only);
                string Private_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Overnight_Admissions);
                string Private_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Day_Admissions_Only);
                string Specialist_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Specialist_Clinic);
                string General_Practice1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Practice);
                string Allied_Health_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Allied_Health_Clinic);
                string General_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Laboratory);
                string Genetics_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Genetics_Laboratory);
                string Registry1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Registry);
                string Clinical_Trial1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Trial);
                string Cohort_Study1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Cohort_Study);
                string Other1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Other);
                string State_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.State_Health_Network);
                string National_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.National_Health_Network);
                string Regulatory_Body_TGA1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Regulatory_Body_TGA);
                string Industry_Peak_Body1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Industry_Peak_Body);
                string Device_Manufacturer1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Device_Manufacturer);
                string Clinical_Craft_Group_Society1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);

                EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Name == Medical_Practitioner__Allied_Healt1);
                EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Name == Non_Medical__Practitioner1);
                EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Public_Overnight_Admissions1);
                EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Public_Day_Admissions_Only1);
                EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Private_Overnight_Admissions1);
                EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Private_Day_Admissions_Only1);
                EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Specialist_Clinic1);
                EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Name == General_Practice1);
                EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Allied_Health_Clinic1);
                EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == General_Laboratory1);
                EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == Genetics_Laboratory1);
                EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Name == Registry1);
                EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Trial1);
                EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Name == Cohort_Study1);
                EntitySubType Other = entitySubtypes.FirstOrDefault(x => x.Name == Other1);
                EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == State_Health_Network1);
                EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == National_Health_Network1);
                EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Name == Regulatory_Body_TGA1);
                EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Name == Industry_Peak_Body1);
                EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Name == Device_Manufacturer1);
                EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Craft_Group_Society1);
                //
                int id = 0;

                List<long?> entitiesMongo = new List<long?>();

                if (participant != null && participant.Count() > 0)
                {
                    participant.ToList().ForEach(parti =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string ln = LastName != null ? LastName.SelectedValues : string.Empty;

                        valueList.Add(fn + " " + ln);
                        descriptionList.Add(fn + " " + ln);
                    });
                }

                if (person != null && person.Count() > 0)
                {
                    person.ToList().ForEach(per =>
                    {
                        id++;
                        FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                        FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                        FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());

                        string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                        string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                        string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                        Guid entityTypeId = Guid.Empty;

                        var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                        Guid entitySubtypeId = Guid.Empty;

                        switch (ps)
                        {
                            case "1":
                                entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                                entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                                break;
                            case "2":
                                entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                                entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                                break;
                            default:
                                break;
                        }

                        valueList.Add(fn + " " + ln);
                        descriptionList.Add(fn + " " + ln);
                        
                    });
                }

                if (place__Group != null && place__Group.Count() > 0)
                {
                    if (entityType == "Place/Group")
                    {
                        EntityType placeentity = dbContext_EntityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Place__Group);

                        place__Group.ToList().ForEach(plc =>
                        {
                            id++;
                            FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                            FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                            string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                            string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                            Guid entityTypeId = (placeentity != null ? placeentity.Guid : Guid.Empty);
                            Guid entitySubtypeId = Guid.Empty;

                            valueList.Add(plName);
                            descriptionList.Add(plName);
                        });
                    }
                    else
                    {
                        EntityType placeentity = dbContext_EntityTypes.FirstOrDefault(x => x.Name == entityType);

                        int etype = placeentity != null ? placeentity.Id : 0;

                        place__Group.ToList().ForEach(plc =>
                        {
                            id++;
                            FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                            FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                            string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                            string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                            string enttypeFrm = GetDBEntTypeFromSelecedEntType(enttype);
                            Guid entityTypeId = Guid.Empty;
                            Guid entitySubtypeId = Guid.Empty;

                            int entTypeDB = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                            int entTypeDBFRM = !string.IsNullOrEmpty(enttypeFrm) ? Convert.ToInt32(enttypeFrm) : 0;
                            if (entTypeDB == etype)
                            {
                                #region Place-Group
                                switch (etype)
                                {
                                    case (int)EntityTypesListInDBSummary.Hospital:
                                        #region Hospital
                                        var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                                        entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                                        var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                                        if (hospSType == "1")
                                        {
                                            entitySubtypeId = Public_Overnight_Admissions.Guid;
                                        }
                                        else if (hospSType == "2")
                                        {
                                            entitySubtypeId = Public_Day_Admissions_Only.Guid;
                                        }
                                        else if (hospSType == "3")
                                        {
                                            entitySubtypeId = Private_Overnight_Admissions.Guid;
                                        }
                                        else if (hospSType == "4")
                                        {
                                            entitySubtypeId = Private_Day_Admissions_Only.Guid;
                                        }
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Practice__Clinic:
                                        #region Practice/Clinic
                                        var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                                        entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                                        var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                                        if (PracSType == "1")
                                        {
                                            entitySubtypeId = Specialist_Clinic.Guid;
                                        }
                                        else if (PracSType == "2")
                                        {
                                            entitySubtypeId = General_Practice.Guid;
                                        }
                                        else if (PracSType == "3")
                                        {
                                            entitySubtypeId = Allied_Health_Clinic.Guid;
                                        }
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Laboratory:
                                        #region Laboratory
                                        var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                                        entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                                        var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                                        if (LabSType == "1")
                                        {
                                            entitySubtypeId = General_Laboratory.Guid;
                                        }
                                        else if (LabSType == "2")
                                        {
                                            entitySubtypeId = Genetics_Laboratory.Guid;
                                        }
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Medical_Imaging:
                                        #region Medical imaging
                                        var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                                        entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Research_facility__University:
                                        #region Research facility/University
                                        var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                                        entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Healthcare_Group:
                                        #region Healthcare Group
                                        var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                                        entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                         
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Government_Organisation:
                                        #region Government Organisation
                                        var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                                        entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                                        var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                                        if (GovSType == "1")
                                        {
                                            entitySubtypeId = State_Health_Network.Guid;
                                        }
                                        else if (GovSType == "2")
                                        {
                                            entitySubtypeId = National_Health_Network.Guid;
                                        }
                                        else if (GovSType == "3")
                                        {
                                            entitySubtypeId = Regulatory_Body_TGA.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Industry_Group:
                                        #region Industry Group
                                        var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                                        entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                                        var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                                        if (IndSType == "1")
                                        {
                                            entitySubtypeId = Industry_Peak_Body.Guid;
                                        }
                                        else if (IndSType == "2")
                                        {
                                            entitySubtypeId = Device_Manufacturer.Guid;
                                        }

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Consumer_Group:
                                        #region Consumer Group
                                        var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                                        entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;

                                        FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                                        var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                                        if (ConSType == "1")
                                        {
                                            entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                                        }
                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Activity_Venue:
                                        #region Activity Venue
                                        var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                                        entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Vehicle:
                                        #region Vehicle
                                        var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                                        entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion


                                        break;
                                    case (int)EntityTypesListInDBSummary.MAC:
                                        #region MAC
                                        var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                                        entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    case (int)EntityTypesListInDBSummary.Ethics_Committee:
                                        #region Ethics Committee
                                        var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                                        entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                                        entitySubtypeId = Guid.Empty;

                                        valueList.Add(plName);
                                        descriptionList.Add(plName);
                                        #endregion
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                            }

                        });
                    }
                }

                if (project != null && project.Count() > 0)
                {
                    project.ToList().ForEach(proj =>
                    {
                        id++;
                        FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                        string fn = Name != null ? Name.SelectedValues : string.Empty;

                        Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                        Guid entitySubtypeId = Guid.Empty;


                        FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                        string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                        switch (pr)
                        {
                            case "1":
                                entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
                                break;
                            case "2":
                                entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
                                break;

                            case "3":
                                entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
                                break;
                            default:
                                break;
                        }
                        valueList.Add(fn);
                        descriptionList.Add(fn);
                    });
                }

                #endregion
            }
            IDictionary<List<string>, List<string>> resultValues = new Dictionary<List<string>, List<string>>();
            resultValues.Add(valueList, descriptionList);
            return resultValues;
        }



        public IEnumerable<LookupVariablesPreviewViewModel> GetLookupVariablesPreview()
        {
            string[] defaultEntityTypes = new string[]
            {
                EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration),
                EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration),
                EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration ),
                EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration),
            };

            List<LookupVariablesPreviewViewModel> personEntitiesList = new List<LookupVariablesPreviewViewModel>();

            var entities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => defaultEntityTypes.Contains(x.ActivityName)).AsQueryable();
            var person = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Person).OrderByDescending(x => x.CreatedDate).AsQueryable().ToList();
            var participant = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Participant).OrderByDescending(x => x.CreatedDate).AsQueryable();
            var place__Group = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Place__Group).OrderByDescending(x => x.CreatedDate).AsQueryable();

            var project = entities.Where(x => x.EntityTypeId == (int)EntityTypesListInDB.Project).OrderByDescending(x => x.CreatedDate).AsQueryable();

            var entitySubtypes = dbContext.EntitySubTypes.AsQueryable();
            var entityTypes = dbContext.EntityTypes.AsQueryable();

            string Medical_Practitioner__Allied_Healt1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Medical_Practitioner__Allied_Healt);
            string Non_Medical__Practitioner1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Non_Medical__Practitioner);
            string Public_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Overnight_Admissions);
            string Public_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Public_Day_Admissions_Only);
            string Private_Overnight_Admissions1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Overnight_Admissions);
            string Private_Day_Admissions_Only1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Private_Day_Admissions_Only);
            string Specialist_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Specialist_Clinic);
            string General_Practice1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Practice);
            string Allied_Health_Clinic1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Allied_Health_Clinic);
            string General_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.General_Laboratory);
            string Genetics_Laboratory1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Genetics_Laboratory);
            string Registry1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Registry);
            string Clinical_Trial1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Trial);
            string Cohort_Study1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Cohort_Study);
            string Other1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Other);
            string State_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.State_Health_Network);
            string National_Health_Network1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.National_Health_Network);
            string Regulatory_Body_TGA1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Regulatory_Body_TGA);
            string Industry_Peak_Body1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Industry_Peak_Body);
            string Device_Manufacturer1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Device_Manufacturer);
            string Clinical_Craft_Group_Society1 = EnumHelpers.GetEnumDescription(EntitySubTypesListInDB.Clinical_Craft_Group_Society);

            EntitySubType Medical_Practitioner__Allied_Healt = entitySubtypes.FirstOrDefault(x => x.Name == Medical_Practitioner__Allied_Healt1);
            EntitySubType Non_Medical__Practitioner = entitySubtypes.FirstOrDefault(x => x.Name == Non_Medical__Practitioner1);
            EntitySubType Public_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Public_Overnight_Admissions1);
            EntitySubType Public_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Public_Day_Admissions_Only1);
            EntitySubType Private_Overnight_Admissions = entitySubtypes.FirstOrDefault(x => x.Name == Private_Overnight_Admissions1);
            EntitySubType Private_Day_Admissions_Only = entitySubtypes.FirstOrDefault(x => x.Name == Private_Day_Admissions_Only1);
            EntitySubType Specialist_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Specialist_Clinic1);
            EntitySubType General_Practice = entitySubtypes.FirstOrDefault(x => x.Name == General_Practice1);
            EntitySubType Allied_Health_Clinic = entitySubtypes.FirstOrDefault(x => x.Name == Allied_Health_Clinic1);
            EntitySubType General_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == General_Laboratory1);
            EntitySubType Genetics_Laboratory = entitySubtypes.FirstOrDefault(x => x.Name == Genetics_Laboratory1);
            EntitySubType Registry = entitySubtypes.FirstOrDefault(x => x.Name == Registry1);
            EntitySubType Clinical_Trial = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Trial1);
            EntitySubType Cohort_Study = entitySubtypes.FirstOrDefault(x => x.Name == Cohort_Study1);
            EntitySubType Other = entitySubtypes.FirstOrDefault(x => x.Name == Other1);
            EntitySubType State_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == State_Health_Network1);
            EntitySubType National_Health_Network = entitySubtypes.FirstOrDefault(x => x.Name == National_Health_Network1);
            EntitySubType Regulatory_Body_TGA = entitySubtypes.FirstOrDefault(x => x.Name == Regulatory_Body_TGA1);
            EntitySubType Industry_Peak_Body = entitySubtypes.FirstOrDefault(x => x.Name == Industry_Peak_Body1);
            EntitySubType Device_Manufacturer = entitySubtypes.FirstOrDefault(x => x.Name == Device_Manufacturer1);
            EntitySubType Clinical_Craft_Group_Society = entitySubtypes.FirstOrDefault(x => x.Name == Clinical_Craft_Group_Society1);
            //
            int id = 0;
            person.ToList().ForEach(per =>
            {
                id++;
                FormDataEntryVariableMongo FirstName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                FormDataEntryVariableMongo LastName = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                FormDataEntryVariableMongo PerSType = per.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PerSType.ToString());

                string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                string ln = LastName != null ? LastName.SelectedValues : string.Empty;
                string ps = PerSType != null ? PerSType.SelectedValues : string.Empty;

                Guid entityTypeId = Guid.Empty;

                var entityTypeId1 = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Person);

                Guid entitySubtypeId = Guid.Empty;

                switch (ps)
                {
                    case "1":
                        entityTypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.EntityType.Guid : Guid.Empty;
                        entitySubtypeId = Medical_Practitioner__Allied_Healt != null ? Medical_Practitioner__Allied_Healt.Guid : Guid.Empty;
                        break;
                    case "2":
                        entityTypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.EntityType.Guid : Guid.Empty;
                        entitySubtypeId = Non_Medical__Practitioner != null ? Non_Medical__Practitioner.Guid : Guid.Empty;
                        break;
                    default:
                        break;
                }

                entityTypeId = entityTypeId1.Guid;

                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn + " " + ln,
                    EntitySubtypeId = entitySubtypeId,
                    EntityTypeId = entityTypeId,
                    EntityGroupId = per.EntityTypeGuid,
                    EntityGroupName = per.EntityTypeName,
                });
            });


            participant.ToList().ForEach(parti =>
            {

                id++;
                FormDataEntryVariableMongo FirstName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.FirstName.ToString());
                FormDataEntryVariableMongo LastName = parti.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());

                string fn = FirstName != null ? FirstName.SelectedValues : string.Empty;
                string ln = LastName != null ? LastName.SelectedValues : string.Empty;

                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn + " " + ln,
                    EntitySubtypeId = Guid.Empty,
                    EntityTypeId = parti != null ? parti.EntityTypeGuid : Guid.Empty,
                    EntityGroupId = parti.EntityTypeGuid,
                    EntityGroupName = parti.EntityTypeName,
                });
            });
            place__Group.ToList().ForEach(plc =>
            {
                id++;
                FormDataEntryVariableMongo FirstName = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                FormDataEntryVariableMongo EntType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.EntType.ToString());

                string plName = FirstName != null ? FirstName.SelectedValues : string.Empty;
                string enttype = EntType != null ? EntType.SelectedValues : string.Empty;

                Guid entityTypeId = Guid.Empty;
                Guid entitySubtypeId = Guid.Empty;

                #region Place-Group
                switch (enttype)
                {
                    case "2":
                        #region Hospital
                        var hospital = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Hospital);
                        entityTypeId = hospital != null ? hospital.Guid : Guid.Empty;

                        FormDataEntryVariableMongo HospSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.HospSType.ToString());
                        var hospSType = HospSType != null ? HospSType.SelectedValues : string.Empty;
                        if (hospSType == "1")
                        {
                            entitySubtypeId = Public_Overnight_Admissions.Guid;
                        }
                        else if (hospSType == "2")
                        {
                            entitySubtypeId = Public_Day_Admissions_Only.Guid;
                        }
                        else if (hospSType == "3")
                        {
                            entitySubtypeId = Private_Overnight_Admissions.Guid;
                        }
                        else if (hospSType == "4")
                        {
                            entitySubtypeId = Private_Day_Admissions_Only.Guid;
                        }
                        #endregion
                        break;
                    case "3":
                        #region Practice/Clinic
                        var practiceClinic = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Practice__Clinic);
                        entityTypeId = practiceClinic != null ? practiceClinic.Guid : Guid.Empty;

                        FormDataEntryVariableMongo pracSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.PracSType.ToString());
                        var PracSType = pracSType != null ? pracSType.SelectedValues : string.Empty;
                        if (PracSType == "1")
                        {
                            entitySubtypeId = Specialist_Clinic.Guid;
                        }
                        else if (PracSType == "2")
                        {
                            entitySubtypeId = General_Practice.Guid;
                        }
                        else if (PracSType == "3")
                        {
                            entitySubtypeId = Allied_Health_Clinic.Guid;
                        }
                        #endregion
                        break;

                    case "4":
                        #region Laboratory
                        var laboratory = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Laboratory);
                        entityTypeId = laboratory != null ? laboratory.Guid : Guid.Empty;

                        FormDataEntryVariableMongo labSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.LabSType.ToString());
                        var LabSType = labSType != null ? labSType.SelectedValues : string.Empty;
                        if (LabSType == "1")
                        {
                            entitySubtypeId = General_Laboratory.Guid;
                        }
                        else if (LabSType == "2")
                        {
                            entitySubtypeId = Genetics_Laboratory.Guid;
                        }
                        #endregion
                        break;
                    case "5":
                        #region Medical imaging
                        var medical_Imaging = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Medical_Imaging);
                        entityTypeId = medical_Imaging != null ? medical_Imaging.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "6":
                        #region Research facility/University
                        var research_facility__University = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Research_facility__University);
                        entityTypeId = research_facility__University != null ? research_facility__University.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "7":
                        #region Healthcare Group
                        var healthcare_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Healthcare_Group);
                        entityTypeId = healthcare_Group != null ? healthcare_Group.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "8":
                        #region Government Organisation
                        var government_Organisation = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Government_Organisation);
                        entityTypeId = government_Organisation != null ? government_Organisation.Guid : Guid.Empty;

                        FormDataEntryVariableMongo govSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.GovSType.ToString());
                        var GovSType = govSType != null ? govSType.SelectedValues : string.Empty;
                        if (GovSType == "1")
                        {
                            entitySubtypeId = State_Health_Network.Guid;
                        }
                        else if (GovSType == "2")
                        {
                            entitySubtypeId = National_Health_Network.Guid;
                        }
                        else if (GovSType == "3")
                        {
                            entitySubtypeId = Regulatory_Body_TGA.Guid;
                        }
                        #endregion
                        break;
                    case "9":
                        #region Industry Group
                        var industry_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Industry_Group);
                        entityTypeId = industry_Group != null ? industry_Group.Guid : Guid.Empty;

                        FormDataEntryVariableMongo indSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.IndSType.ToString());
                        var IndSType = indSType != null ? indSType.SelectedValues : string.Empty;
                        if (IndSType == "1")
                        {
                            entitySubtypeId = Industry_Peak_Body.Guid;
                        }
                        else if (IndSType == "2")
                        {
                            entitySubtypeId = Device_Manufacturer.Guid;
                        }
                        #endregion
                        break;
                    case "10":
                        #region Consumer Group
                        var consumer_Group = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Consumer_Group);
                        entityTypeId = consumer_Group != null ? consumer_Group.Guid : Guid.Empty;

                        FormDataEntryVariableMongo conSType = plc.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConSType.ToString());
                        var ConSType = conSType != null ? conSType.SelectedValues : string.Empty;
                        if (ConSType == "1")
                        {
                            entitySubtypeId = Clinical_Craft_Group_Society.Guid;
                        }
                        #endregion
                        break;
                    case "11":
                        #region Activity Venue
                        var activity_Venue = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Activity_Venue);
                        entityTypeId = activity_Venue != null ? activity_Venue.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "12":
                        #region Vehicle
                        var vehicle = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Vehicle);
                        entityTypeId = vehicle != null ? vehicle.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "13":
                        #region MAC
                        var mAC = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.MAC);
                        entityTypeId = mAC != null ? mAC.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    case "14":
                        #region Ethics Committee
                        var ethics_Committee = entityTypes.FirstOrDefault(x => x.Id == (int)EntityTypesListInDB.Ethics_Committee);
                        entityTypeId = ethics_Committee != null ? ethics_Committee.Guid : Guid.Empty;
                        entitySubtypeId = Guid.Empty;
                        #endregion
                        break;
                    default:
                        break;
                }
                #endregion

                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = plName,
                    EntitySubtypeId = entitySubtypeId,
                    EntityTypeId = entityTypeId,
                    EntityGroupId = plc.EntityTypeGuid,
                    EntityGroupName = plc.EntityTypeName,
                });
            });
            project.ToList().ForEach(proj =>
            {
                id++;
                FormDataEntryVariableMongo Name = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.Name.ToString());
                string fn = Name != null ? Name.SelectedValues : string.Empty;
                Guid entityTypeId = entityTypes.Where(x => x.Id == (int)EntityTypesListInDB.Project).Select(x => x.Guid).FirstOrDefault();
                Guid entitySubtypeId = Guid.Empty;
                FormDataEntryVariableMongo proSType = proj.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                string pr = proSType != null ? proSType.SelectedValues : string.Empty;
                switch (pr)
                {
                    case "1":
                        entitySubtypeId = Registry != null ? Registry.Guid : Guid.Empty;
                        break;
                    case "2":
                        entitySubtypeId = Clinical_Trial != null ? Clinical_Trial.Guid : Guid.Empty;
                        break;

                    case "3":
                        entitySubtypeId = Cohort_Study != null ? Cohort_Study.Guid : Guid.Empty;
                        break;
                    default:
                        break;
                }
                personEntitiesList.Add(new LookupVariablesPreviewViewModel
                {
                    Id = id,
                    EntityName = fn,
                    EntitySubtypeId = entitySubtypeId,
                    EntityTypeId = entityTypeId,
                    EntityGroupId = proj.EntityTypeGuid,
                    EntityGroupName = proj.EntityTypeName,
                });
            });
            return personEntitiesList;
        }


        public string GetDBEntTypeFromSelecedEntType(string enttype)
        {
            switch (enttype)
            {
                case "7":
                    enttype = "8";
                    break;
                case "8":
                    enttype = "9";
                    break;
                case "9":
                    enttype = "10";
                    break;
                case "10":
                    enttype = "11";
                    break;
                case "11":
                    enttype = "12";
                    break;
                case "12":
                    enttype = "13";
                    break;
                case "13":
                    enttype = "14";
                    break;
                case "14":
                    enttype = "15";
                    break;
                case "15":
                    enttype = "";
                    break;
                default:
                    break;
            }
            return enttype;

        }
    }
}