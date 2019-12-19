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
    public class EntityProvider : IEntityProvider
    {

        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        private readonly MongoDBContext _mongoDBContext;
        public EntityProvider(AspreeEntities _dbContext, IUserLoginProvider userLoginProvider, MongoDBContext mongoDBContext)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._mongoDBContext = mongoDBContext;
        }

        public EntityViewModel Create(EntityViewModel model)
        {
            var tenant = dbContext.Tenants.FirstOrDefault(t => t.Guid == model.TenantId);
            var entityType = this.dbContext.EntityTypes.FirstOrDefault(et => et.Guid == model.EntityTypeId);
            var entitySubType = this.dbContext.EntitySubTypes.FirstOrDefault(et => et.Guid == model.EntitySubTypeId);

            if (entitySubType != null)
            {
                if (dbContext.Entities.Any(et => et.EntityTypeId == entityType.Id && et.EntitySubTypeId == entitySubType.Id && et.TenantId == tenant.Id))
                {
                    throw new Core.AlreadyExistsException("Entity already exists.");
                }
            }
            else
            {
                if (dbContext.Entities.Any(et => et.EntityTypeId == entityType.Id && et.TenantId == tenant.Id))
                {
                    throw new Core.AlreadyExistsException("Entity already exists.");
                }
            }

            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var entityParent = this.dbContext.Entities.FirstOrDefault(et => et.Guid == model.ParentEntityId);

            var entity = new Entity()
            {
                Guid = Guid.NewGuid(),
                Name = model.Name,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                EntityTypeId = entityType.Id,
                EntitySubTypeId = model.EntitySubTypeId.HasValue ? entitySubType.Id : (int?)null,
                ParentEntityId = model.ParentEntityId.HasValue ? entityParent.Id : (int?)null,
                Status = (int)Core.Enum.Status.Active,
                TenantId = tenant != null ? tenant.Id : 0
            };

            dbContext.Entities.Add(entity);

            SaveChanges();

            foreach (var entityVariable in model.DroppedVariablesList)
            {
                var variableid = this.dbContext.Variables.FirstOrDefault(et => et.Guid == entityVariable);

                var entityVariables = new EntityVariable()
                {
                    Guid = Guid.NewGuid(),
                    EntityId = entity.Id,
                    VariableId = variableid.Id,
                };
                dbContext.EntityVariables.Add(entityVariables);

            }
            SaveChanges();
            return ToModel(entity);
        }

        public EntityViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entity = dbContext.Entities.FirstOrDefault(fs => fs.Guid == guid);
            if (entity != null)
            {
                entity.DeactivatedBy = deactivatedBy.Id;
                entity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(entity);
            }
            return null;
        }

        public EntityViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var entity = dbContext.Entities.FirstOrDefault(fs => fs.Id == id);
            if (entity != null)
            {
                entity.DeactivatedBy = deactivatedBy.Id;
                entity.DateDeactivated = DateTime.UtcNow;
                SaveChanges();
                return ToModel(entity);
            }
            return null;
        }

        public IEnumerable<EntityViewModel> GetAll()
        {
            return dbContext.Entities
              .Select(ToModel)
              .ToList();
        }

        public IEnumerable<EntityViewModel> GetAll(Guid tenantId)
        {
            return dbContext.Entities
                 .Where(et => et.TenantId.HasValue && et.Tenant.Guid == tenantId && et.DateDeactivated == null)
                 .Select(ToModel)
                 .ToList();
        }

        public EntityViewModel GetByGuid(Guid guid)
        {
            var entity = dbContext.Entities
                .FirstOrDefault(fs => fs.Guid == guid);

            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public EntityViewModel GetById(int id)
        {
            var entity = dbContext.Entities
                 .FirstOrDefault(fs => fs.Id == id);

            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        public EntityViewModel ToModel(Entity entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;

            var entitysubtype = dbContext.EntitySubTypes.FirstOrDefault(x => x.Id == entity.EntitySubTypeId);

            var entityvariableList = dbContext.EntityVariables.Where(x => x.EntityId == entity.Id)
                .Select(ToEntityVariableModel).ToList();

            return new EntityViewModel()
            {
                Guid = entity.Guid,
                Name = entity.Name,
                EntityTypeId = entity.EntityType.Guid,
                EntitySubTypeId = entity.EntitySubTypeId.HasValue ? entity.EntitySubType.Guid : (Guid?)null,
                ParentEntityId = entity.ParentEntityId.HasValue ? entity.EntityType.Guid : (Guid?)null,
                Id = entity.Id,
                Status = entity.Status,
                CreatedBy = createdBy.Guid,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = modifiedBy != null ? modifiedBy.Guid : (Guid?)null,
                ModifiedDate = entity.ModifiedDate,
                DeactivatedBy = deactivatedBy != null ? deactivatedBy.Guid : (Guid?)null,
                DateDeactivated = entity.DateDeactivated != null ? entity.DateDeactivated : (DateTime?)null,
                EntityVariableList = entityvariableList,
                EntityTypeName = entity.EntityType != null ? entity.EntityType.Name : null,
                EntitySubtypeName = entitysubtype != null ? entitysubtype.Name : null,
            };
        }

        public EntityViewModel Update(EntityViewModel model)
        {
            if (dbContext.Entities.Any(et => et.Name.ToLower() == model.Name.ToLower()
            && et.Guid != model.Guid))
            {
                //throw new Core.AlreadyExistsException("Entity already exists.");
            }

            var entity = dbContext.Entities
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            var entityType = this.dbContext.EntityTypes.FirstOrDefault(et => et.Guid == model.EntityTypeId);
            var entitySubType = this.dbContext.EntitySubTypes.FirstOrDefault(et => et.Guid == model.EntitySubTypeId);
            var entityParent = this.dbContext.Entities.FirstOrDefault(et => et.Guid == model.ParentEntityId);

            if (entity != null)
            {
                var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);

                entity.Name = model.Name;
                entity.EntityTypeId = entityType != null ? entityType.Id : 0;
                entity.EntitySubTypeId = entitySubType != null ? entitySubType.Id : (int?)null;
                entity.ParentEntityId = entityParent != null ? entityParent.Id : (int?)null;
                entity.ModifiedBy = modifiedBy.Id;
                entity.ModifiedDate = DateTime.UtcNow;

                entity.EntitySubType = entitySubType;

                SaveChanges();

                IEnumerable<EntityVariable> list = dbContext.EntityVariables.Where(x => x.EntityId == entity.Id).ToList();
                // Use Remove Range function to delete all records at once
                dbContext.EntityVariables.RemoveRange(list);
                foreach (var entityVariable in model.DroppedVariablesList)
                {
                    var variableid = this.dbContext.Variables.FirstOrDefault(et => et.Guid == entityVariable);

                    var entityVariables = new EntityVariable()
                    {
                        Guid = Guid.NewGuid(),
                        EntityId = entity.Id,
                        VariableId = variableid.Id,
                    };
                    dbContext.EntityVariables.Add(entityVariables);
                }
                SaveChanges();

                return ToModel(entity);
            }

            return null;
        }

        public EntityViewModel GetByTenantGuid(Guid guid)
        {

            var tenant = dbContext.Tenants.FirstOrDefault(t => t.Guid == guid);

            var entity = dbContext.Entities
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(fs => fs.TenantId == tenant.Id);

            if (entity != null)
                return ToModel(entity);

            return null;
        }


        public EntityViewModel GetByEntityTypeAndSubTypeGuid(Guid tenantGuid, Guid typeGuid, Guid? subTypeGuid = null)
        {
            var entity = new Entity();

            if (!subTypeGuid.HasValue)
            {
                entity = dbContext.Entities
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(fs => fs.Tenant.Guid == tenantGuid && fs.EntityType.Guid == typeGuid && !fs.EntitySubTypeId.HasValue);
            }
            else
            {
                entity = dbContext.Entities
                .OrderByDescending(x => x.Id)
                .FirstOrDefault(fs => fs.Tenant.Guid == tenantGuid && fs.EntityType.Guid == typeGuid && fs.EntitySubType.Guid == subTypeGuid);
            }


            if (entity != null)
                return ToModel(entity);

            return null;
        }

        public EntityVariableViewModel ToEntityVariableModel(EntityVariable entity)
        {
            var variable = dbContext.Variables.FirstOrDefault(x => x.Id == entity.VariableId);

            return new EntityVariableViewModel()
            {
                Guid = entity.Guid,
                EntityId = entity.EntityId,
                VariableId = entity.VariableId,
                Id = entity.Id,
                EntityVariableGuid = variable.Guid,

                IdVariableTable = variable.Id,
                VariableName = variable.VariableName,
                VariableLabel = variable.VariableLabel,
                Question = variable.Question,
                ValueDescription = variable.ValueDescription,
                HelpText = variable.HelpText,
                VariableTypeId = variable.VariableTypeId,
                ValidationMessage = variable.ValidationMessage,
                RequiredMessage = variable.RequiredMessage,

                MinRange = variable.MinRange != null ? variable.MinRange : (double?)null,
                MaxRange = variable.MaxRange != null ? variable.MaxRange : (double?)null,

                RegEx = variable.RegEx,
                IsSoftRange = variable.IsSoftRange != null ? variable.IsSoftRange : (bool?)null,
                ValidationRuleId = variable.ValidationRuleId != null ? variable.ValidationRuleId : (int?)null,
                DependentVariableId = variable.DependentVariableId != null ? variable.DependentVariableId : (int?)null,

                IsRequired = variable.IsRequired,
                CanCollectMultiple = variable.CanCollectMultiple,
                VariableCategoryId = variable.VariableCategoryId != null ? variable.VariableCategoryId : (int?)null,
                IsApproved = variable.IsApproved,
                Comment = variable.Comment,
                VariableGuid = variable.Guid,
                TenantId = variable.TenantId != null ? variable.TenantId : (int?)null,
                VariableType = variable.VariableType.Type,
                Values = variable.Values.Split('|').ToList(),
                VariableValueDescription = variable.VariableValueDescription.Split('|').ToList(),
            };
        }

        public IEnumerable<EntityViewModel> GetAllEntitiesCreatedBySearch()
        {
            EntityViewModel model = new EntityViewModel();
            List<EntityViewModel> entityList = new List<EntityViewModel>();

            int formDataEntryid = 0;

            EntityType entityType = dbContext.EntityTypes.FirstOrDefault(x => x.Name.ToLower() == Core.Enum.EntityTypes.Project.ToString().ToLower());
            List<int> projectIds = dbContext.FormDataEntries.Where(x => x.EntityId == entityType.Id && x.Form.FormTitle == "Project Registration" && x.ProjectDeployStatus != (int)Core.Enum.ProjectStatusTypes.Published).Select(x => x.Id).ToList();

            string[] defaultFormTitles =
                {
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration),
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration),
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration),
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration)
                };

            var formDataEntry = dbContext.FormDataEntries.Where(x => projectIds.Contains((int)x.ProjectId) && defaultFormTitles.Contains(x.Form.FormTitle));
            if (formDataEntry != null)
            {
                foreach (var proj in formDataEntry)
                {
                    try
                    {
                        model = new EntityViewModel();
                        formDataEntryid = proj.Id;

                        var formDataEntryVariables = dbContext.FormDataEntryVariables.Where(et => et.FormDataEntryId == formDataEntryid);
                        var entType = proj.Form.FormEntityTypes.FirstOrDefault();

                        model.EntID = formDataEntryVariables.Where(x => x.Variable.VariableName == "EntID").Select(x => x.SelectedValues).FirstOrDefault();
                        model.EntityTypeName = entType != null ? entType.EntityType != null ? entType.EntityType.Name : string.Empty : string.Empty;

                        string formName = proj.Form.FormTitle;
                        if (formName == "Person Registration")
                        {
                            var fname = formDataEntryVariables.Where(x => x.Variable.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                            var sname = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                            fname = fname != null ? fname : string.Empty;
                            sname = sname != null ? sname : string.Empty;
                            model.Name = String.Concat(fname, " ", sname);
                            var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == "PerSType").Select(x => x.SelectedValues).FirstOrDefault();
                            {
                                var perSType = dbContext.Variables.FirstOrDefault(x => x.VariableName == "PerSType");
                                var variableValues = perSType != null ? perSType.Values != null ? perSType.Values.Split('|').ToList() : null : null;
                                var variableValuesDesc = perSType != null ? perSType.VariableValueDescription != null ? perSType.VariableValueDescription.Split('|').ToList() : null : null;
                                for (int i = 0; i < variableValues.Count; i++)
                                {
                                    if (variableValues[i] == ProjectSubtype)
                                    {
                                        model.EntitySubtypeName = variableValuesDesc[i];
                                    }
                                }
                            }
                        }
                        else if (formName == "Participant Registration")
                        {
                            var fname = formDataEntryVariables.Where(x => x.Variable.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                            var mname = formDataEntryVariables.Where(x => x.Variable.VariableName == "MiddleName").Select(x => x.SelectedValues).FirstOrDefault();
                            var sname = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();

                            fname = fname != null ? fname : string.Empty;
                            mname = mname != null ? mname : string.Empty;
                            sname = sname != null ? sname : string.Empty;
                            model.Name = String.Concat(fname, " ", mname, " ", sname);
                            model.Name = model.Name.Replace("  ", " ");
                        }
                        else if (formName == "Place/Group Registration")
                        {
                            var sname = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                            model.Name = sname != null ? sname : string.Empty;

                            var placeSubType = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.EntType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                            try
                            {
                                var str = new string[] { "Person", "Hospital", "Practice/Clinic", "Laboratory", "Medical Imaging", "Research facility/University", "Healthcare Group", "Government Organisation", "Industry Group", "Consumer Group", "Activity Venue", "Vehicle", "MAC", "Ethics Committee", "API" };
                                int ii = !String.IsNullOrEmpty(placeSubType) ? Convert.ToInt32(placeSubType) : 0;
                                string vald = str[ii - 1];
                                model.EntitySubtypeName = vald;
                            }
                            catch (Exception ecPlc) { }
                        }
                        else if (formName == "Project Registration")
                        {
                            var sname = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                            model.Name = sname != null ? sname : string.Empty;

                            var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                            {
                                var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ProSType");
                                var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                                var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                                for (int i = 0; i < variableValues.Count; i++)
                                {
                                    if (variableValues[i] == ProjectSubtype)
                                    {
                                        model.EntitySubtypeName = variableValuesDesc[i];
                                    }
                                }
                            }
                        }
                        model.ModifiedDate = proj.ModifiedDate != null ? proj.ModifiedDate : proj.CreatedDate;
                        model.FormGuid = proj.Form.Guid;
                        entityList.Add(model);
                    }
                    catch (Exception exception) { }
                }
            }
            entityList.AddRange(MongoEntities(defaultFormTitles));
            return entityList.OrderByDescending(x => x.ModifiedDate);
        }

        public List<EntityViewModel> MongoEntities(string[] defaultFormTitles)
        {
            EntityViewModel model = new EntityViewModel();
            List<EntityViewModel> entityList = new List<EntityViewModel>();

            List<FormDataEntryMongo> mongoEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().Where(x => defaultFormTitles.Contains(x.FormTitle)).AsQueryable().ToList();
            mongoEntities.ForEach(y =>
            {
                model = new EntityViewModel();
                model.EntID = y.EntityNumber.ToString("D7");
                model.EntityTypeName = y.EntityTypeName;
                model.ModifiedDate = y.ModifiedDate != null ? y.ModifiedDate : y.CreatedDate;

                if (y.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                {
                    var fname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                    var sname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                    fname = fname != null ? fname : string.Empty;
                    sname = sname != null ? sname : string.Empty;
                    model.Name = String.Concat(fname, " ", sname);

                    var ProjectSubtype = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "PerSType").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var perSType = dbContext.Variables.FirstOrDefault(x => x.VariableName == "PerSType");
                        var variableValues = perSType != null ? perSType.Values != null ? perSType.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = perSType != null ? perSType.VariableValueDescription != null ? perSType.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ProjectSubtype)
                            {
                                model.EntitySubtypeName = variableValuesDesc[i];
                            }
                        }
                    }
                }
                else if (y.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var fname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "FirstName").Select(x => x.SelectedValues).FirstOrDefault();
                    var mname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "MiddleName").Select(x => x.SelectedValues).FirstOrDefault();
                    var sname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                    fname = fname != null ? fname : string.Empty;
                    mname = mname != null ? mname : string.Empty;
                    sname = sname != null ? sname : string.Empty;
                    model.Name = String.Concat(fname, " ", mname, " ", sname);
                    model.Name = model.Name.Replace("  ", " ");
                }
                else if (y.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    var sname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == DefaultsVariables.Name.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    model.Name = sname != null ? sname : string.Empty;                    
                    try
                    {
                        var placeSubType = y.formDataEntryVariableMongoList.Where(x => x.VariableName == DefaultsVariables.EntType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                        var str = new string[] { "Person", "Hospital", "Practice/Clinic", "Laboratory", "Medical Imaging", "Research facility/University", "Healthcare Group", "Government Organisation", "Industry Group", "Consumer Group", "Activity Venue", "Vehicle", "MAC", "Ethics Committee", "API" };
                        int ii = !String.IsNullOrEmpty(placeSubType) ? Convert.ToInt32(placeSubType) : 0;
                        string vald = str[ii - 1];
                        model.EntitySubtypeName = vald;
                    }
                    catch (Exception ecPlc) { }
                }
                else if (y.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
                {
                    var sname = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();
                    model.Name = sname != null ? sname : string.Empty;

                    var ProjectSubtype = y.formDataEntryVariableMongoList.Where(x => x.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ProSType");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ProjectSubtype)
                            {
                                model.EntitySubtypeName = variableValuesDesc[i];
                            }
                        }
                    }
                }

                model.FormGuid = y.FormGuid;
                entityList.Add(model);
            });

            return entityList;
        }
    }
}