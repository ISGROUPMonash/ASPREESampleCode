using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;

namespace Aspree.Provider.Provider
{
    public class PrivilegeProvider : IPrivilegeProvider
    {

        private readonly AspreeEntities _dbContext;
        private readonly ISecurity _security = new Security();
        public PrivilegeProvider(AspreeEntities dbContext)
        {
            this._dbContext = dbContext;
        }

        public PrivilegeSmallViewModel Create(PrivilegeSmallViewModel model)
        {
            throw new NotImplementedException();
        }

        public PrivilegeSmallViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public PrivilegeSmallViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PrivilegeSmallViewModel> GetAll()
        {
            return this._dbContext.Privileges//.OrderBy(x => x.Name)
                .Select(ToModel)
                .ToList();
        }

        public PrivilegeSmallViewModel GetByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public PrivilegeSmallViewModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            this._dbContext.SaveChanges();
        }

        public void SetDefaults()
        {
            #region Privilege
            //add default system permissions
            if (!this._dbContext.Privileges.Any())
            {
                var privileges = Enum.GetNames(typeof(Core.Enum.Privileges));
                foreach (var privilege in privileges)
                {
                    this._dbContext.Privileges.Add(new Data.Privilege
                    {
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1,
                        Guid = Guid.NewGuid(),
                        Name = privilege.Replace("_", " ")
                    });
                }

                SaveChanges();
            }

            #endregion Privilege

            #region Status
            //add default system status
            if (!this._dbContext.Status.Any())
            {
                var statusList = Enum.GetNames(typeof(Core.Enum.Status));
                foreach (var status in statusList)
                {
                    this._dbContext.Status.Add(new Data.Status
                    {
                        Guid = Guid.NewGuid(),
                        IsActive = true,
                        Status1 = status,
                    });
                }

                SaveChanges();
            }
            #endregion Status

            #region Roles

            //add system roles
            var permistions = this._dbContext.Privileges.ToArray();
            if (!this._dbContext.Roles.Any())
            {
                var da = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    Name = "Definition Admin",
                    IsSystemRole = true,
                    Status = 1,
                };

                this._dbContext.Roles.Add(da);
                SaveChanges();

                da.RolePrivileges = permistions.ToList()// permistions.Where(p => p.Name.Contains("User") || p.Name.Contains("Role") || p.Name.Contains("Email"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = da.Id
                    })
                .ToList();

                var sa = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    Name = "System Admin",
                    IsSystemRole = true,
                    Status = 1,
                };

                this._dbContext.Roles.Add(sa);
                SaveChanges();

                sa.RolePrivileges = permistions.Where(p => !p.Name.Contains("Email"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = sa.Id
                    })
                .ToList();

                var pa = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    Name = "Project Admin",
                    IsSystemRole = true,
                    Status = 1,
                };

                this._dbContext.Roles.Add(pa);
                SaveChanges();

                pa.RolePrivileges = permistions.Where(p => !p.Name.Contains("Email"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = pa.Id
                    })
                .ToList();

                SaveChanges();
            }

            #endregion Roles

            #region AuthTypeMasters

            if (!this._dbContext.AuthTypeMasters.Any())
            {
                this._dbContext.AuthTypeMasters.Add(new Data.AuthTypeMaster
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    AuthType = "Password",
                });

                this._dbContext.AuthTypeMasters.Add(new Data.AuthTypeMaster
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    AuthType = "Institutional",
                });

                SaveChanges();
            }

            #endregion AuthTypeMasters

            #region SecurityQuestions

            if (!this._dbContext.SecurityQuestions.Any())
            {
                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your favourite color?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your childhood nick name?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "Where were you born?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your pet animal name?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your favourite hobby?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is the name of your favorite childhood friend?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "In what city or town did your mother and father meet?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your favorite team?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What is your favorite movie?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What was your favorite food as a child?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "Who is your childhood sports hero?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "In what town was your first job?"
                });

                this._dbContext.SecurityQuestions.Add(new Data.SecurityQuestion
                {
                    Guid = Guid.NewGuid(),
                    Question = "What was the name of the company where you had your first job?"
                });

                SaveChanges();
            }

            #endregion SecurityQuestions

            #region Tenants

            if (!this._dbContext.Tenants.Any())
            {
                this._dbContext.Tenants.Add(new Data.Tenant
                {
                    Guid = new Guid("c505be1c-6374-4c3f-b0d7-dad859ca633d"),
                    Email = "superadmin@aspree.com",
                    CompanyName = "Definition Admin",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    FirstName = "Super",
                    LastName = "Admin",
                    Status = (int)Core.Enum.Status.Active
                });

                this._dbContext.Tenants.Add(new Data.Tenant
                {
                    Guid = Guid.NewGuid(),
                    Email = "aspreext@aspree.com",
                    CompanyName = "Aspree - XT",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    FirstName = "Aspree",
                    LastName = "XT",
                    Status = (int)Core.Enum.Status.Active
                });

                SaveChanges();
            }

            var tenantId = this._dbContext.Tenants.ToList().Last().Id;

            #endregion Tenants

            #region Project Roles

            if (!this._dbContext.UserLogins.Any())
            {
                var des = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    Name = "Data Entry Supervisor",
                    IsSystemRole = true,
                    //IsSystemRole = false,
                    Status = 1,
                    TenantId = tenantId
                };

                this._dbContext.Roles.Add(des);
                SaveChanges();

                des.RolePrivileges = permistions.Where(p => p.Name.Contains("Entry"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = des.Id
                    })
                .ToList();

                SaveChanges();


                var dao = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid(),
                    Name = "Data Entry Operator",
                    TenantId = tenantId,
                    IsSystemRole = true,
                    //IsSystemRole = false,
                    Status = 1,
                };

                this._dbContext.Roles.Add(dao);
                SaveChanges();

                dao.RolePrivileges = permistions.Where(p => p.Name.Contains("Entry"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = dao.Id
                    })
                .ToList();

                SaveChanges();
            }
            #endregion Project Roles

            #region Temp Data Entry Roles

            if (!this._dbContext.Roles.Any(r => r.Name == "Data Entry"))
            {
                var permistions1 = this._dbContext.Privileges.ToArray();
                var deo1 = new Data.Role
                {
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 2,
                    Guid = Guid.NewGuid(),
                    Name = "Data Entry",
                    IsSystemRole = true,
                    //IsSystemRole = false,
                    Status = 1,
                    TenantId = tenantId
                };

                this._dbContext.Roles.Add(deo1);
                SaveChanges();

                deo1.RolePrivileges = permistions1.Where(p => p.Name.Contains("Entry") || p.Name.Contains("Variable"))
                    .Select(c => new RolePrivilege()
                    {
                        Guid = Guid.NewGuid(),
                        PrivilegeId = c.Id,
                        RoleId = deo1.Id
                    })
                .ToList();

                SaveChanges();
            }
            #endregion

            #region UserLogins

            String salt = Guid.NewGuid().ToString().Replace("-", "");
            string password = _security.ComputeHash("12345678", salt);


            if (!this._dbContext.UserLogins.Any())
            {
                var roles = _dbContext.Roles.ToArray();

                var da = new Data.UserLogin
                {
                    Guid = new Guid("c505be1c-6374-4c3f-b0d7-dad859ca633d"),
                    Email = "definitionadmin@aspree.com",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    FirstName = "DA",
                    LastName = "User",
                    Mobile = "1234567890",
                    //Password = "12345678",
                    Password = password,
                    Salt = salt,
                    Status = 1,
                    Address = "Sydney",
                    TenantId = this._dbContext.Tenants.First().Id,
                    AuthTypeId = this._dbContext.AuthTypeMasters.First().Id,
                    SecurityQuestionId = this._dbContext.SecurityQuestions.First().Id,
                    Answer = "pink"
                };

                this._dbContext.UserLogins.Add(da);
                SaveChanges();

                da.UserRoles.Add(new UserRole()
                {
                    UserId = da.Id,
                    RoleId = roles.First(r => r.Name == Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " ")).Id,
                    Guid = Guid.NewGuid()
                });

                var sa = new Data.UserLogin
                {
                    Guid = Guid.NewGuid(),
                    Email = "systemadmin@aspree.com",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    FirstName = "SA",
                    LastName = "User",
                    Mobile = "1234567890",
                    //Password = "12345678",
                    Password = password,
                    Salt = salt,
                    Status = 1,
                    Address = "Sydney",
                    TenantId = tenantId,
                    AuthTypeId = this._dbContext.AuthTypeMasters.First().Id,
                    SecurityQuestionId = this._dbContext.SecurityQuestions.First().Id,
                    Answer = "pink"
                };

                this._dbContext.UserLogins.Add(sa);
                SaveChanges();

                sa.UserRoles.Add(new UserRole()
                {
                    UserId = sa.Id,
                    RoleId = roles.First(r => r.Name == Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " ")).Id,
                    Guid = Guid.NewGuid()
                });


                var pa = new Data.UserLogin
                {
                    Guid = Guid.NewGuid(),
                    Email = "projectadmin@aspree.com",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    FirstName = "PA",
                    LastName = "User",
                    Mobile = "1234567890",
                    //Password = "12345678",
                    Password = password,
                    Salt = salt,
                    Status = 1,
                    Address = "Sydney",
                    TenantId = tenantId,
                    AuthTypeId = this._dbContext.AuthTypeMasters.First().Id,
                    SecurityQuestionId = this._dbContext.SecurityQuestions.First().Id,
                    Answer = "pink"
                };

                this._dbContext.UserLogins.Add(pa);
                SaveChanges();

                pa.UserRoles.Add(new UserRole()
                {
                    UserId = pa.Id,
                    RoleId = roles.First(r => r.Name == Core.Enum.RoleTypes.Project_Admin.ToString().Replace("_", " ")).Id,
                    Guid = Guid.NewGuid()
                });


                SaveChanges();
            }

            #endregion UserLogins

            #region PushEmailEvents

            if (!this._dbContext.PushEmailEvents.Any())
            {
                var templates = Enum.GetNames(typeof(Core.Enum.EmailTemplateTypes));
                foreach (var template in templates)
                {
                    this._dbContext.PushEmailEvents.Add(new Data.PushEmailEvent
                    {
                        Guid = Guid.NewGuid(),
                        DisplayName = template.Replace("_", " "),
                        EventName = template,
                        IsEmailEvent = true
                    });
                }

                SaveChanges();
            }


            if (!this._dbContext.EmailTemplates.Any())
            {
                var events = this._dbContext.PushEmailEvents.ToArray();

                this._dbContext.EmailTemplates.Add(new Data.EmailTemplate
                {
                    Guid = Guid.NewGuid(),
                    Subject = "Account Created - Aspree",
                    MailBody = string.Empty,
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    EmailKeywords = "@UserName@ @Url@",
                    IsActive = true,
                    PushEmailEventId = events.First(e => e.EventName == Core.Enum.EmailTemplateTypes.Welcome.ToString()).Id,
                    TenantId = tenantId
                });

                this._dbContext.EmailTemplates.Add(new Data.EmailTemplate
                {
                    Guid = Guid.NewGuid(),
                    Subject = "Reset Password - Aspree",
                    MailBody = string.Empty,
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    EmailKeywords = "@UserName@ @Url@",
                    IsActive = true,
                    PushEmailEventId = events.First(e => e.EventName == Core.Enum.EmailTemplateTypes.Forgot_Password.ToString()).Id,
                    TenantId = tenantId
                });

                this._dbContext.EmailTemplates.Add(new Data.EmailTemplate
                {
                    Guid = Guid.NewGuid(),
                    Subject = "Change Password - Aspree",
                    MailBody = string.Empty,
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    EmailKeywords = "@UserName@ @Url@",
                    IsActive = true,
                    PushEmailEventId = events.First(e => e.EventName == Core.Enum.EmailTemplateTypes.Change_Password.ToString()).Id,
                    TenantId = tenantId
                });

                SaveChanges();
            }

            if (!this._dbContext.SmtpServers.Any())
            {
                this._dbContext.SmtpServers.Add(new SmtpServer()
                {
                    FromEmail = "no-reply@monash.edu",
                    Guid = Guid.NewGuid(),
                    MailServer = "smtp.monash.edu",
                    Name = "Aspree",
                    Port = 25,
                    Status = (int)Core.Enum.Status.Active,
                    Password = string.Empty
                });

                SaveChanges();
            }

            #endregion PushEmailEvents

            #region VariableTypes

            if (!this._dbContext.VariableTypes.Any())
            {
                var variableTypes = Enum.GetNames(typeof(Core.Enum.VariableTypes));
                foreach (var variableType in variableTypes)
                {
                    this._dbContext.VariableTypes.Add(new Data.VariableType
                    {
                        Guid = Guid.NewGuid(),
                        Status = (int)Core.Enum.Status.Active,
                        Type = variableType.Replace("___", ")").Replace("__", " (").Replace("_", " "),
                    });
                }

                SaveChanges();
            }

            #endregion VariableTypes

            #region ActivityStatus

            if (!this._dbContext.ActivityStatus.Any())
            {
                var activityStatus = Enum.GetNames(typeof(Core.Enum.ActivityStatusTypes));
                foreach (var status in activityStatus)
                {
                    this._dbContext.ActivityStatus.Add(new Data.ActivityStatu
                    {
                        Guid = Guid.NewGuid(),
                        Status = status.Replace("_", " "),
                        IsActive = true
                    });
                }

                SaveChanges();
            }

            #endregion ActivityStatus

            #region FormStatus

            if (!this._dbContext.FormStatus.Any())
            {
                var formStatus = Enum.GetNames(typeof(Core.Enum.FormStatusTypes));
                foreach (var status in formStatus)
                {
                    this._dbContext.FormStatus.Add(new Data.FormStatu
                    {
                        Guid = Guid.NewGuid(),
                        Status = status.Replace("_", " "),
                        IsActive = true
                    });
                }

                SaveChanges();
            }

            #endregion FormStatus

            #region ProjectStatus

            if (!this._dbContext.ProjectStatus.Any())
            {
                var projectStatus = Enum.GetNames(typeof(Core.Enum.ProjectStatusTypes));
                foreach (var status in projectStatus)
                {
                    this._dbContext.ProjectStatus.Add(new Data.ProjectStatu
                    {
                        Guid = Guid.NewGuid(),
                        Status = status.Replace("_", " "),
                        IsActive = true
                    });
                }

                SaveChanges();
            }

            #endregion ProjectStatus

            #region EntityTypes

            if (!this._dbContext.EntityTypes.Any())
            {
                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Person",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Hospital",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Practice/Clinic",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Laboratory",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Medical Imaging",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Research facility/University",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Project",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Healthcare Group",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Government Organisation",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Industry Group",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Consumer Group",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Activity Venue",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Vehicle",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "MAC",
                    TenantId = tenantId
                });

                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Ethics Committee",
                    TenantId = tenantId
                });

                //added on 22-nov-2018
                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Participant",
                    TenantId = tenantId
                });
                //added on 22-nov-2018
                this._dbContext.EntityTypes.Add(new Data.EntityType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Place/Group",
                    TenantId = tenantId
                });

                SaveChanges();

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Medical Practitioner/Allied Health",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Person").Id
                });


                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Non-Medical Practitioner",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Person").Id
                });

                ///////////////
                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Public (Overnight Admissions)",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Hospital").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Public (Day Admissions Only)",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Hospital").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Private (Overnight Admissions)",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Hospital").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Private (Day Admissions Only)",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Hospital").Id
                });
                /////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Specialist Clinic",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Practice/Clinic").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "General Practice",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Practice/Clinic").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Allied Health Clinic",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Practice/Clinic").Id
                });

                ///////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "General Laboratory",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Laboratory").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Genetics Laboratory",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Laboratory").Id
                });

                /////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Registry",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Project").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Clinical Trial ",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Project").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Cohort Study",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Project").Id
                });

                /////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "State Health Network",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Government Organisation").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "National Health Network",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Government Organisation").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Regulatory Body (TGA)",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Government Organisation").Id
                });

                /////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Industry Peak Body",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Industry Group").Id
                });

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Device Manufacturer",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Industry Group").Id
                });

                /////////////

                this._dbContext.EntitySubTypes.Add(new Data.EntitySubType
                {
                    Guid = Guid.NewGuid(),
                    Name = "Clinical Craft Group/ Society ",
                    EntityTypeId = this._dbContext.EntityTypes.First(et => et.Name == "Consumer Group").Id
                });

                SaveChanges();
            }

            #endregion ProjectStatus

            #region CheckLists 

            if (!this._dbContext.CheckLists.Any())
            {

                this._dbContext.CheckLists.Add(new Data.CheckList
                {
                    Guid = Guid.NewGuid(),
                    Title = "Test 1",
                    CreatedBy = this._dbContext.UserLogins.First(ul => ul.Email == "projectadmin@aspree.com").Id,
                    CreatedDate = DateTime.UtcNow
                });

                this._dbContext.CheckLists.Add(new Data.CheckList
                {
                    Guid = Guid.NewGuid(),
                    Title = "Test 2",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow
                });

                this._dbContext.CheckLists.Add(new Data.CheckList
                {
                    Guid = Guid.NewGuid(),
                    Title = "Test 3",
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow
                });

                SaveChanges();
            }

            #endregion CheckLists 

            #region Sites 

            if (!this._dbContext.Sites.Any())
            {

                this._dbContext.Sites.Add(new Data.Site
                {
                    Guid = Guid.NewGuid(),
                    Name = "Sydney",
                    AddressLine1 = "Sydney",
                    AddressLine2 = "Sydney",
                    CreatedBy = this._dbContext.UserLogins.First(ul => ul.Email == "projectadmin@aspree.com").Id,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId
                });

                SaveChanges();
            }

            #endregion Sites 



            #region Scheduling Type Questions
            if (!this._dbContext.SchedulingTypeMasters.Any())
            {
                var schedulingTypes = Enum.GetNames(typeof(Core.Enum.SchedulingTypeQuestions));
                foreach (var schedulingType in schedulingTypes)
                {
                    this._dbContext.SchedulingTypeMasters.Add(new Data.SchedulingTypeMaster
                    {
                        SchedulingType = schedulingType.Replace("__", "?").Replace("_", " "),
                    });
                }
                SaveChanges();
            }
            #endregion







            #region Country
            if (!this._dbContext.Countries.Any())
            {
                var countries = Enum.GetNames(typeof(Core.Enum.Country));
                foreach (var country in countries)
                {
                    this._dbContext.Countries.Add(new Data.Country
                    {
                        Guid = Guid.NewGuid(),
                        Abbr = "AU",
                        Name = country.Replace("_", " "),
                    });
                }
                SaveChanges();
            }
            #endregion

            #region State
            if (!this._dbContext.States.Any())
            {
                var australia = this._dbContext.Countries.FirstOrDefault(x => x.Name == Core.Enum.Country.Australia.ToString().Replace("_", " "));
                var stateList = Enum.GetNames(typeof(Core.Enum.State));
                foreach (var state in stateList)
                {
                    this._dbContext.States.Add(new Data.State
                    {
                        Guid = Guid.NewGuid(),
                        Name = state.Replace("_", " "),
                        CountryId = australia.Id,
                    });
                }
                SaveChanges();
            }

            #endregion

            #region City
            if (!this._dbContext.Cities.Any())
            {
                var New_South_Wales = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.New_South_Wales.ToString().Replace("_", " "));
                var Queensland = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Queensland.ToString().Replace("_", " "));
                var South_Australia = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.South_Australia.ToString().Replace("_", " "));
                var Tasmania = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Tasmania.ToString().Replace("_", " "));
                var Victoria = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Victoria.ToString().Replace("_", " "));
                var Western_Australia = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Western_Australia.ToString().Replace("_", " "));
                var Northern_Territory = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Northern_Territory.ToString().Replace("_", " "));
                var Australian_Capital_Territory = this._dbContext.States.FirstOrDefault(c => c.Name == Core.Enum.State.Australian_Capital_Territory.ToString().Replace("_", " "));


                #region New_South_Wales
                var sydney = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Sydney",
                });
                var newcastle = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Newcastle",
                });
                var Central_Coast = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Central Coast",
                });
                var Wollongong = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Wollongong",
                });


                var Albury = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Albury",
                });
                var Maitland = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Maitland",
                });
                var Wagga_Wagga = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Wagga Wagga",
                });
                var Port_Macquarie = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Port Macquarie",
                });
                var Tamworth = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Tamworth",
                });
                var Orange = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Orange",
                });
                var Dubbo = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Dubbo",
                });
                var Lismore = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Lismore",
                });
                var Bathurst = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Bathurst",
                });
                var Coffs_Harbour = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Coffs Harbour",
                });
                var Richmond = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Richmond",
                });
                var Nowra = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = New_South_Wales.Id,
                    Name = "Nowra",
                });
                #endregion

                #region Queensland
                var Brisbane = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Brisbane",
                });
                var Gold_Coast = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Gold Coast",
                });
                var Cairns = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Cairns",
                });
                var Mackay = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Mackay",
                });

                var Sunshine_Coast = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Sunshine Coast",
                });
                var Townsville = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Townsville",
                });
                var Toowoomba = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Toowoomba",
                });
                var Rockhampton = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Rockhampton",
                });
                var Bundaberg = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Bundaberg",
                });
                var Hervey_Bay = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Hervey Bay",
                });
                var Gladstone = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Queensland.Id,
                    Name = "Gladstone",
                });
                #endregion

                #region South_Australia
                var Adelaide = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = South_Australia.Id,
                    Name = "Adelaide",
                });
                var Mount_Gambier = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = South_Australia.Id,
                    Name = "Mount Gambier",
                });
                //var Whyalla = this._dbContext.Cities.Add(new Data.City
                //{
                //    Guid = Guid.NewGuid(),
                //    StatedId = South_Australia.Id,
                //    Name = "Whyalla",
                //});
                //var Victor_Harbor = this._dbContext.Cities.Add(new Data.City
                //{
                //    Guid = Guid.NewGuid(),
                //    StatedId = South_Australia.Id,
                //    Name = "Victor Harbor",
                //});
                #endregion

                #region Tasmania
                var Hobart = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Tasmania.Id,
                    Name = "Hobart",
                });
                //var Richmond = this._dbContext.Cities.Add(new Data.City
                //{
                //    Guid = Guid.NewGuid(),
                //    StatedId = Tasmania.Id,
                //    Name = "Richmond",
                //});
                var Launceston = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Tasmania.Id,
                    Name = "Launceston",
                });
                //var Burnie = this._dbContext.Cities.Add(new Data.City
                //{
                //    Guid = Guid.NewGuid(),
                //    StatedId = Tasmania.Id,
                //    Name = "Burnie",
                //});
                #endregion

                #region Victoria
                var Melbourne = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Melbourne",
                });
                var Geelong = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Geelong",
                });
                var Ballarat = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Ballarat",
                });
                var Bendigo = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Bendigo",
                });

                var Shepparton = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Shepparton",
                });
                var Melton = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Melton",
                });
                var Mildura = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Mildura",
                });
                var Warrnambool = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Warrnambool",
                });
                var Sunbury = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Victoria.Id,
                    Name = "Sunbury",
                });
                #endregion

                #region Western_Australia
                var Perth = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Perth",
                });
                var Bunbury = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Bunbury",
                });
                var Albany = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Albany",
                });
                var Kalgoorlie = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Kalgoorlie",
                });
                var Rockingham = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Rockingham",
                });
                var Geraldton = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Geraldton",
                });
                var Mandurah = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Western_Australia.Id,
                    Name = "Mandurah",
                });
                #endregion

                #region Northern_Territory
                var Darwin = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Northern_Territory.Id,
                    Name = "Darwin",
                });
                var Alice_Springs = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Northern_Territory.Id,
                    Name = "Alice Springs",
                });

                #endregion

                #region Australian_Capital_Territory
                var Canberra = this._dbContext.Cities.Add(new Data.City
                {
                    Guid = Guid.NewGuid(),
                    StatedId = Australian_Capital_Territory.Id,
                    Name = "Canberra",
                });
                #endregion



                SaveChanges();
                #region suburb of sydney
                var Balmain = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Balmain",
                    CityId = sydney.Id,
                    StateId = New_South_Wales.Id,
                });
                var Newtown = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Newtown",
                    CityId = sydney.Id,
                    StateId = New_South_Wales.Id,
                });

                var Adamstown = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Adamstown",
                    CityId = newcastle.Id,
                    StateId = New_South_Wales.Id,
                });
                var Beresfield = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Beresfield",
                    CityId = newcastle.Id,
                    StateId = New_South_Wales.Id,
                });

                var Calga = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Calga",
                    CityId = Central_Coast.Id,
                    StateId = New_South_Wales.Id,
                });

                var Austinmer = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Austinmer",
                    CityId = Wollongong.Id,
                    StateId = New_South_Wales.Id,
                });
                var Bellambi = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Bellambi",
                    CityId = Wollongong.Id,
                    StateId = New_South_Wales.Id,
                });



                #endregion
                #region suburb of queensland
                var Albino = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Albino",
                    CityId = Brisbane.Id,
                    StateId = Queensland.Id,
                });
                var Banyo = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Banyo",
                    CityId = Brisbane.Id,
                    StateId = Queensland.Id,
                });
                var Alberton = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Alberton",
                    CityId = Gold_Coast.Id,
                    StateId = Queensland.Id,
                });
                var Pimpana = this._dbContext.Suburbs.Add(new Data.Suburb
                {
                    Guid = Guid.NewGuid(),
                    Name = "Pimpana",
                    CityId = Gold_Coast.Id,
                    StateId = Queensland.Id,
                });
                #endregion

                SaveChanges();

                #region postcode of sydney
                var Balmain_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2041",
                    CityId = sydney.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Balmain.Id,
                });

                var Newtown_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2042",
                    CityId = sydney.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Newtown.Id,
                });

                var Adamstown_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2289",
                    CityId = newcastle.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Adamstown.Id,
                });
                var Beresfield_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2322",
                    CityId = newcastle.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Beresfield.Id,
                });

                var Calga_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2250",
                    CityId = newcastle.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Calga.Id,
                });
                var Austinmer_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2215",
                    CityId = Wollongong.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Austinmer.Id,
                });
                var Bellambi_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "2518",
                    CityId = Wollongong.Id,
                    StateId = New_South_Wales.Id,
                    SuburbId = Bellambi.Id,
                });

                #endregion
                #region postcode of queensland
                var Albino_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "24021",
                    CityId = Brisbane.Id,
                    StateId = Queensland.Id,
                    SuburbId = Balmain.Id,
                });

                var Banyo_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "4014",
                    CityId = Brisbane.Id,
                    StateId = Queensland.Id,
                    SuburbId = Banyo.Id,
                });

                var Alberton_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "5014",
                    CityId = Gold_Coast.Id,
                    StateId = Queensland.Id,
                    SuburbId = Alberton.Id,
                });
                var Pimpana_postcode = this._dbContext.PostCodes.Add(new Data.PostCode
                {
                    Guid = Guid.NewGuid(),
                    PostalCode = "4209",
                    CityId = Gold_Coast.Id,
                    StateId = Queensland.Id,
                    SuburbId = Pimpana.Id,
                });

                #endregion

                SaveChanges();
            }
            #endregion

            #region Categories
            var createdBy = this._dbContext.UserLogins.First(ul => ul.Email == "systemadmin@aspree.com").Id;
            if (!this._dbContext.VariableCategories.Any())
            {

                #region old-data-VariableCategory
                //this._dbContext.VariableCategories.Add(new Data.VariableCategory
                //{
                //    Guid = Guid.NewGuid(),
                //    CategoryName = "Default",
                //    CreatedBy = createdBy,
                //    CreatedDate = DateTime.UtcNow,
                //    TenantId = tenantId
                //});

                //this._dbContext.VariableCategories.Add(new Data.VariableCategory
                //{
                //    Guid = Guid.NewGuid(),
                //    CategoryName = "Custom",
                //    CreatedBy = createdBy,
                //    CreatedDate = DateTime.UtcNow,
                //    TenantId = tenantId
                //});
                #endregion

                var variableCategories = Enum.GetNames(typeof(Core.Enum.VariableCategories));
                foreach (var variableCategory in variableCategories)
                {
                    this._dbContext.VariableCategories.Add(new Data.VariableCategory
                    {
                        Guid = Guid.NewGuid(),
                        CategoryName = variableCategory.Replace("_", " "),
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.UtcNow,
                        TenantId = tenantId,
                        IsDefaultVariableCategory = (int)Core.Enum.DefaultVariableType.Default
                    });
                }
                SaveChanges();
            }
            if (!this._dbContext.FormCategories.Any())
            {
                var formCategories = Enum.GetNames(typeof(Core.Enum.FormCategories));
                foreach (var formCategory in formCategories)
                {
                    #region old-data-FormCategory
                    //this._dbContext.FormCategories.Add(new Data.FormCategory
                    //{
                    //    Guid = Guid.NewGuid(),
                    //    CategoryName = "Default",
                    //    CreatedBy = createdBy,
                    //    CreatedDate = DateTime.UtcNow,
                    //    TenantId = tenantId
                    //});

                    //this._dbContext.FormCategories.Add(new Data.FormCategory
                    //{
                    //    Guid = Guid.NewGuid(),
                    //    CategoryName = "Custom",
                    //    CreatedBy = createdBy,
                    //    CreatedDate = DateTime.UtcNow,
                    //    TenantId = tenantId
                    //});
                    #endregion
                    this._dbContext.FormCategories.Add(new Data.FormCategory
                    {
                        Guid = Guid.NewGuid(),
                        CategoryName = formCategory.Replace("_", " "),
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.UtcNow,
                        TenantId = tenantId,
                        IsDefaultFormCategory = (int)Core.Enum.DefaultFormType.Default,
                    });
                }
                SaveChanges();
            }

            if (!this._dbContext.ActivityCategories.Any())
            {
                var activityCategories = Enum.GetNames(typeof(Core.Enum.ActivityCategories));
                foreach (var activityCategory in activityCategories)
                {
                    this._dbContext.ActivityCategories.Add(new Data.ActivityCategory
                    {
                        Guid = Guid.NewGuid(),
                        CategoryName = activityCategory.Replace("_", " "),
                        CreatedBy = createdBy,
                        CreatedDate = DateTime.UtcNow,
                        TenantId = tenantId
                    });
                }
                SaveChanges();
            }

            #endregion Categories

            #region ValidationRule

            if (!this._dbContext.ValidationRules.Any())
            {
                var regExRuleEmail = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Email",
                    Description = "Email",
                    Guid = Guid.NewGuid(),
                    //RegEx = @"/\S+@\S+\.\S+/",
                    RegEx = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                };

                this._dbContext.RegExRules.Add(regExRuleEmail);

                var regExRuleUrl = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Url",
                    Description = "Url",
                    Guid = Guid.NewGuid(),
                    //RegEx = @"[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)",
                    RegEx = @"[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)",
                };

                this._dbContext.RegExRules.Add(regExRuleUrl);


                var regExRuleNumeric = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Numeric",
                    Description = "Numeric",
                    Guid = Guid.NewGuid(),
                    RegEx = @"^(\d*\.)?\d+$",// number with decimal point
                    //RegEx = @"^[0-9]*$",
                };



                this._dbContext.RegExRules.Add(regExRuleNumeric);


                var regExRuleLetterOnly = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Letter Only",
                    Description = "Letter Only",
                    Guid = Guid.NewGuid(),
                    RegEx = @"^[a-zA-Z]+$"
                };
                this._dbContext.RegExRules.Add(regExRuleLetterOnly);

                var regExRuleDecimalOnly = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Decimal",
                    Description = "Decimal",
                    Guid = Guid.NewGuid(),
                    RegEx = @"^[0-9]*(\.[0-9]{1,2})?$"
                };
                this._dbContext.RegExRules.Add(regExRuleDecimalOnly);

                var regExRuleDate = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Date",
                    Description = "Date",
                    Guid = Guid.NewGuid(),
                    RegEx = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"
                };
                this._dbContext.RegExRules.Add(regExRuleDate);

                var regExRuleLength = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Length",
                    Description = "Length",
                    Guid = Guid.NewGuid(),
                    //RegEx = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"
                };
                this._dbContext.RegExRules.Add(regExRuleLength);


                var regExRuleRange = new RegExRule()
                {
                    CreatedBy = 1,
                    CreatedDate = DateTime.UtcNow,
                    RegExName = "Range",
                    Description = "Range",
                    Guid = Guid.NewGuid(),
                    //RegEx = @"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$"
                };
                this._dbContext.RegExRules.Add(regExRuleRange);
                SaveChanges();

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleEmail.RegExName,
                    ErrorMessage = "Invalid Email Address",
                    RegExId = regExRuleEmail.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleUrl.RegExName,
                    ErrorMessage = "Invalid Url",
                    RegExId = regExRuleUrl.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleNumeric.RegExName,
                    ErrorMessage = "Numeric Only",
                    RegExId = regExRuleNumeric.Id
                });


                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleLetterOnly.RegExName,
                    ErrorMessage = "Letter Only",
                    RegExId = regExRuleLetterOnly.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleDecimalOnly.RegExName,
                    ErrorMessage = "Decimal Only",
                    RegExId = regExRuleDecimalOnly.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleDate.RegExName,
                    ErrorMessage = "Valid Date Format(DD/MM/YYYY)",
                    RegExId = regExRuleDate.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleLength.RegExName,
                    ErrorMessage = "Length",
                    RegExId = regExRuleLength.Id
                });

                this._dbContext.ValidationRules.Add(new ValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    RuleType = regExRuleRange.RegExName,
                    ErrorMessage = "Range",
                    RegExId = regExRuleRange.Id
                });

                SaveChanges();
            }

            #endregion

            #region Variables
            //if (!this._dbContext.Variables.Any())
            {
                //var createdBy = this._dbContext.UserLogins.First(ul => ul.Email == "systemadmin@aspree.com").Id;

                var VariableCategorySystem = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.System.ToString());
                var VariableCategoryDefault = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.Default.ToString());

                //var VariableCategoryUniqueIdentifier = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.Unique_Identifier.ToString());
                //var VariableCategoryEntityRegistration = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.Entity_Registration.ToString());
                //var VariableCategoryDemographics = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.Demographics.ToString());
                //var VariableCategoryProjectDetails = this._dbContext.VariableCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.VariableCategories.Project_Details.ToString());

                var variableTypeNumeric = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type == Core.Enum.VariableTypes.Numeric__Integer___.ToString().Replace("___", ")").Replace("__", " (").Replace("_", " "));
                var variableTypeDropdown = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Dropdown.ToString());
                var variableTypeFree_Text = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Free_Text.ToString());
                var variableTypeText_Box = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Text_Box.ToString());
                var variableTypeDate = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Date.ToString());
                var variableTypeLKUP = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.LKUP.ToString());
                var variableTypeHeading = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Heading.ToString());
                var variableTypeSubheading = this._dbContext.VariableTypes.FirstOrDefault(et => et.Type.Replace(" ", "_") == Core.Enum.VariableTypes.Other_Text.ToString());

                #region variable 48(Heading)
                var Heading = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Heading,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Heading",
                    VariableLabel = "Heading",

                    Question = "Insert heading text",
                    ValueDescription = "Insert heading text",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryUniqueIdentifier != null ? VariableCategoryUniqueIdentifier.Id : (int?)null,
                    VariableTypeId = variableTypeHeading != null ? variableTypeHeading.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = false,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 49(OtherText)
                var OtherText = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Heading,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "OtherText",
                    VariableLabel = "OtherText",

                    Question = "Insert other text",
                    ValueDescription = "Insert other text",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryUniqueIdentifier != null ? VariableCategoryUniqueIdentifier.Id : (int?)null,
                    VariableTypeId = variableTypeSubheading != null ? variableTypeSubheading.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = false,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable-1(EntID)
                var EntID = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    VariableName = "EntID",
                    Question = "Entity ID",
                    Values = "",
                    VariableValueDescription = "",
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                    ValueDescription = "Entity ID",
                    VariableLabel = "EntID",
                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryUniqueIdentifier != null ? VariableCategoryUniqueIdentifier.Id : (int?)null,
                    VariableTypeId = variableTypeNumeric != null ? variableTypeNumeric.Id : 0,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,
                });
                #endregion
                SaveChanges();
                #region variable 2(EntGrp)
                var EntGrp = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "EntGrp",
                    Question = "Entity Group",
                    ValueDescription = "Entity Group",
                    VariableLabel = "EntGrp",

                    Values = "1|2|3|4",
                    VariableValueDescription = "Participant|Person|Place/Group|Project",

                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable 3(EntType)
                var EntType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "EntType",
                    VariableLabel = "EntGrp",

                    Question = "Entity type",
                    ValueDescription = "Entity type",


                    Values = "1|2|3|4|5|6|7|8|9|10|11|12|13|14",
                    //VariableValueDescription = "Person|Hospital|Practice/Clinic|Laboratory|Medical imaging|Research facility/University|Healthcare group|Government organisation|Industry group|Consumer group|Activity Venue|Vehicle|MAC|Ethics Committee",
                    VariableValueDescription = "Person|Hospital|Practice/Clinic|EntID|Medical imaging|Research facility/University|Healthcare group|Government organisation|Industry group|Consumer group|Activity Venue|Vehicle|MAC|Ethics Committee",

                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable 4(PerSType)
                var PerSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "PerSType",
                    VariableLabel = "PerSType",

                    Question = "Person sub-type",
                    ValueDescription = "Person sub-type",


                    Values = "1|2",
                    VariableValueDescription = "Medical Practitioner/Allied Health|Non-medical practitioner",

                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable 5(HospSType)
                var HospSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "HospSType",
                    VariableLabel = "HospSType",

                    Question = "Hospital sub-type",
                    ValueDescription = "Hospital sub-type",


                    Values = "1|2|3|4",
                    VariableValueDescription = "Public(overnight admissions)|Public(day admissions only)|Private(overnight admissions)|Private(day admissions only)",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable 6(PracSType)
                var PracSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "PracSType",
                    VariableLabel = "PracSType",

                    Question = "Practice/Clinic sub-type",
                    ValueDescription = "Practice/Clinic sub-type",


                    Values = "1|2|3",
                    VariableValueDescription = "Specialist clinic|General Practice|Allied health clinic",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion
                SaveChanges();
                #region variable 7(LabSType)
                var LabSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "LabSType",
                    VariableLabel = "LabSType",

                    Question = "Laboratory sub-type",
                    ValueDescription = "Laboratory sub-type",


                    Values = "1|2",
                    VariableValueDescription = "General laboratory|Genetics laboratory",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 8(ProSType)
                var ProSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "ProSType",
                    VariableLabel = "ProSType",

                    Question = "Project sub-type",
                    ValueDescription = "Project sub-type",


                    Values = "1|2|3|4",
                    VariableValueDescription = "Registry|Clinical Trial|Cohort study|Other",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 9(GovSType)
                var GovSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "GovSType",
                    VariableLabel = "GovSType",

                    Question = "Government organisation sub-type",
                    ValueDescription = "Government organisation sub-type",


                    Values = "1|2|3",
                    VariableValueDescription = "State health network|National health network|Regulatory body(TGA)",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 10(IndSType)
                var IndSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "IndSType",
                    VariableLabel = "IndSType",

                    Question = "Industry sub-type",
                    ValueDescription = "Industry sub-type",


                    Values = "1|2",
                    VariableValueDescription = "Industry peak body|Device manufacturer",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 11(ConSType)
                var ConSType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "ConSType",
                    VariableLabel = "ConSType",

                    Question = "Consumer sub-group",
                    ValueDescription = "Consumer sub-group",


                    Values = "1",
                    VariableValueDescription = "Clinical craft group/Society ",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 12(Title)
                var Title = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Title",
                    VariableLabel = "Title",

                    Question = "Title",
                    ValueDescription = "Title",


                    Values = "1|2|3|4|5|6|7|8|9|10|11|12|13|14|15",
                    VariableValueDescription = "A/Prof|Brother|Dame|Dr|Father|Lady|Miss|Mr|Mrs|Ms|Pastor|Prof|Rev|Sir|Sister",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 13(Name)
                var Name = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Name",
                    VariableLabel = "Name",

                    Question = "Name/Surname",
                    ValueDescription = "Name/Surname",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 14(FirstName)
                var FirstName = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "FirstName",
                    VariableLabel = "FirstName",

                    Question = "First Name",
                    ValueDescription = "First Name",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 15(MiddleName)
                var MiddleName = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "MiddleName",
                    VariableLabel = "MiddleName",

                    Question = "Middle Name",
                    ValueDescription = "Middle Name",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 16(NoMidNm)
                var NoMidNm = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "NoMidNm",
                    VariableLabel = "NoMidNm",

                    Question = "No middle Name",
                    ValueDescription = "No middle Name",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem!= null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeText_Box != null ? variableTypeText_Box.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 17(PrefName)
                var PrefName = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "PrefName",
                    VariableLabel = "PrefName",

                    Question = "Preferred Name (Not to be utilised for mailing)",
                    ValueDescription = "Preferred Name (Not to be utilised for mailing)",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 18(Unit)
                var Unit = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Unit",
                    VariableLabel = "Unit",

                    Question = "Unit number",
                    ValueDescription = "Unit number",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 18(NoUnit)
                var NoUnit = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "NoUnit",
                    VariableLabel = "NoUnit",

                    Question = "No unit number",
                    ValueDescription = "No unit number",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeText_Box != null ? variableTypeText_Box.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 19(StrtNum)
                var StrtNum = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtNum",
                    VariableLabel = "StrtNum",

                    Question = "Street number",
                    ValueDescription = "Street number",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 20(StrtNme)
                var StrtNme = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtNme",
                    VariableLabel = "StrtNme",

                    Question = "Street name",
                    ValueDescription = "Street name",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 21(StrtType)
                var StrtType = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtType",
                    VariableLabel = "StrtType",

                    Question = "Street type",
                    ValueDescription = "Street type",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 22(Suburb)
                var Suburb = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Suburb",
                    VariableLabel = "Suburb",

                    Question = "Suburb",
                    ValueDescription = "Suburb",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 23(Country)
                var Country = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Country",
                    VariableLabel = "Country",

                    Question = "Country",
                    ValueDescription = "Country",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 24(State)
                var State = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "State",
                    VariableLabel = "State",

                    Question = "State",
                    ValueDescription = "State",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem  != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 25(Postcode)
                var Postcode = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Postcode",
                    VariableLabel = "Postcode",

                    Question = "Postcode",
                    ValueDescription = "Postcode",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeText_Box != null ? variableTypeText_Box.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 26(DifAddress)
                var DifAddress = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "DifAddress",
                    VariableLabel = "DifAddress",

                    Question = "Different mailing address",
                    ValueDescription = "Different mailing address",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeText_Box != null ? variableTypeText_Box.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 27(StrtNum2)
                var StrtNum2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtNum2",
                    VariableLabel = "StrtNum2",

                    Question = "Street number 2",
                    ValueDescription = "Street number 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeNumeric != null ? variableTypeNumeric.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    MaxRange = (double?)null,
                    MinRange = 1,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "You have entered a number that is outside the data entry range of 1 or more. Please review and amend the data entry.",
                    Comment = "",
                });
                #endregion

                #region variable 28(StrtNme2)
                var StrtNme2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtNme2",
                    VariableLabel = "StrtNme2",

                    Question = "Street name 2",
                    ValueDescription = "Street name 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 29(StrtType2)
                var StrtType2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "StrtType2",
                    VariableLabel = "StrtType2",

                    Question = "Street type 2",
                    ValueDescription = "Street type 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 30(Suburb2)
                var Suburb2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Suburb2",
                    VariableLabel = "Suburb2",

                    Question = "Suburb 2",
                    ValueDescription = "Suburb 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault  != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 31(Country2)
                var Country2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Country2",
                    VariableLabel = "Country2",

                    Question = "Country 2",
                    ValueDescription = "Country 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 32(State2)
                var State2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "State2",
                    VariableLabel = "State2",

                    Question = "State 2",
                    ValueDescription = "State 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 33(Postcode2)
                var Postcode2 = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Postcode2",
                    VariableLabel = "Postcode2",

                    Question = "Postcode 2",
                    ValueDescription = "Postcode 2",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeLKUP != null ? variableTypeLKUP.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 34(NoAddress)
                var NoAddress = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "NoAddress",
                    VariableLabel = "NoAddress",

                    Question = "Address not provided",
                    ValueDescription = "Address not provided",


                    Values = "1|2",
                    VariableValueDescription = "Address not known|No fixed address",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 35(Email)
                var Email = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,
                    VariableName = "Email",
                    VariableLabel = "Email",
                    Question = "Email",
                    ValueDescription = "Email",
                    Values = "",
                    VariableValueDescription = "",
                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,
                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });

                #endregion

                #region variable 47(Phone)
                var Phone = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Phone",
                    VariableLabel = "Phone",

                    Question = "Phone",
                    ValueDescription = "Phone",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategoryDefault != null ? VariableCategoryDefault.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeNumeric != null ? variableTypeNumeric.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 36(Username)
                var Username = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Username",
                    VariableLabel = "Username",

                    Question = "Entity username",
                    ValueDescription = "Entity username",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeFree_Text != null ? variableTypeFree_Text.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 37(SysAppr)
                var SysAppr = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "SysAppr",
                    VariableLabel = "SysAppr",

                    Question = "Approved by System Admin",
                    ValueDescription = "Approved by System Admin",


                    Values = "0|1",
                    VariableValueDescription = "No|Yes",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 38(Active)
                var Active = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Active",
                    VariableLabel = "Active",

                    Question = "Active user",
                    ValueDescription = "Active user",


                    Values = "0|1",
                    VariableValueDescription = "No|Yes",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 39(SysRole)
                var SysRole = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "SysRole",
                    VariableLabel = "SysRole",

                    Question = "System role",
                    ValueDescription = "System role",


                    Values = "1|2",
                    VariableValueDescription = "System administrator|User",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryEntityRegistration != null ? VariableCategoryEntityRegistration.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 40(DOB)
                var DOB = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "DOB",
                    VariableLabel = "DOB",

                    Question = "Date of birth",
                    ValueDescription = "Date of birth",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryDemographics != null ? VariableCategoryDemographics.Id : (int?)null,
                    VariableTypeId = variableTypeDate != null ? variableTypeDate.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 41(Gender)
                var Gender = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Gender",
                    VariableLabel = "Gender",

                    Question = "Gender",
                    ValueDescription = "Gender",


                    Values = "1|2|3",
                    VariableValueDescription = "Male|Female|Other",



                    VariableCategoryId = VariableCategorySystem!= null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryDemographics != null ? VariableCategoryDemographics.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = true,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 42(ConfData)
                var ConfData = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "ConfData",
                    VariableLabel = "ConfData",

                    Question = "Will this project hold fully identified patient data ?",
                    ValueDescription = "Will this project hold fully identified patient data ?",


                    Values = "0|1",
                    VariableValueDescription = "No|Yes",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryProjectDetails != null ? VariableCategoryProjectDetails.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 43(CnstModel)
                var CnstModel = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "CnstModel",
                    VariableLabel = "CnstModel",

                    Question = "What is the consent model for this project ?",
                    ValueDescription = "What is the consent model for this project ?",


                    Values = "1|2|3",
                    VariableValueDescription = "Signed consent prior to entry|Opt in|Opt out",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryProjectDetails != null ? VariableCategoryProjectDetails.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 44(Ethics)
                var Ethics = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "Ethics",
                    VariableLabel = "Ethics",

                    Question = "Does this project have ethics approval",
                    ValueDescription = "Does this project have ethics approval",


                    Values = "0|1",
                    VariableValueDescription = "No|Yes",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryProjectDetails != null ? VariableCategoryProjectDetails.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 45(DataStore)
                var DataStore = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "DataStore",
                    VariableLabel = "DataStore",

                    Question = "What are the data storage requirements based on your ethic approval?",
                    ValueDescription = "What are the data storage requirements based on your ethic approval?",


                    Values = "1|2|3|4",
                    VariableValueDescription = "7 years|15 years|30 years|Indefinite",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryProjectDetails != null ? VariableCategoryProjectDetails.Id : (int?)null,
                    VariableTypeId = variableTypeDropdown != null ? variableTypeDropdown.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    RequiredMessage = "Please enter missing data.",
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion

                #region variable 46(ProDt)
                var ProDt = this._dbContext.Variables.Add(new Data.Variable
                {
                    Guid = Guid.NewGuid(),
                    IsDefaultVariable = (int)Core.Enum.DefaultVariableType.Default,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId,

                    VariableName = "ProDt",
                    VariableLabel = "ProDt",

                    Question = "Project activation date",
                    ValueDescription = "Project activation date",


                    Values = "",
                    VariableValueDescription = "",



                    VariableCategoryId = VariableCategorySystem != null ? VariableCategorySystem.Id : (int?)null,
                    //VariableCategoryId = VariableCategoryProjectDetails != null ? VariableCategoryProjectDetails.Id : (int?)null,
                    VariableTypeId = variableTypeDate != null ? variableTypeDate.Id : 0,

                    CanCollectMultiple = false,
                    IsApproved = true,
                    //MaxRange = 500,
                    //MinRange = 0,
                    IsRequired = true,
                    DependentVariableId = null,
                    ValidationMessage = "",
                    Comment = "",
                });
                #endregion             

                SaveChanges();

                #region Variable Validation Rules

                var validationRules = this._dbContext.ValidationRules.FirstOrDefault(x => x.RuleType == Core.Enum.Variables.Email.ToString());
                this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = "",

                    Max = (int?)null,
                    Min = (int?)null,
                    RegEx = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                    ValidationId = validationRules != null ? validationRules.Id : (int?)null,
                    ValidationMessage = validationRules != null ? validationRules.ErrorMessage : "Invalid Email Address",
                    VariableId = Email.Id,
                });

                var validationRulesPhone = this._dbContext.ValidationRules.FirstOrDefault(x => x.RuleType == Core.Enum.Variables.Phone.ToString());
                this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = "",

                    Max = (int?)null,
                    Min = (int?)null,
                    RegEx = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}",
                    ValidationId = validationRulesPhone != null ? validationRulesPhone.Id : (int?)null,
                    ValidationMessage = validationRulesPhone != null ? validationRulesPhone.ErrorMessage : "Invalid Phone No",
                    VariableId = Phone.Id,
                });


                var validationRulesNumeric = this._dbContext.ValidationRules.FirstOrDefault(x => x.RuleType == Core.Enum.Variables.Numeric.ToString());
                this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = "",

                    Max = (int?)null,
                    Min = (int?)null,
                    //RegEx = @"^[0-9]*$",
                    RegEx = @"^[0-9]+$",// number only
                    //RegEx = @"^(\d*\.)?\d+$",// number with decimal point
                    ValidationId = validationRulesNumeric != null ? validationRulesNumeric.Id : (int?)null,
                    ValidationMessage = "Numbers Only",
                    VariableId = EntID.Id,
                });

                //this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                //{
                //    Guid = Guid.NewGuid(),
                //    LimitType = "",

                //    Max = (int?)null,
                //    Min = (int?)null,
                //    //RegEx = @"^[0-9]*$",
                //    RegEx = @"^[0-9]+$",// number only
                //    //RegEx = @"^(\d*\.)?\d+$",// number with decimal point
                //    ValidationId = validationRulesNumeric != null ? validationRulesNumeric.Id : (int?)null,
                //    ValidationMessage = "Numbers Only",
                //    VariableId = Unit.Id,
                //});

                //this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                //{
                //    Guid = Guid.NewGuid(),
                //    LimitType = "",

                //    Max = (int?)null,
                //    Min = (int?)null,
                //    //RegEx = @"^[0-9]*$",
                //    RegEx = @"^[0-9]+$",// number only
                //    //RegEx = @"^(\d*\.)?\d+$",// number with decimal point
                //    ValidationId = validationRulesNumeric != null ? validationRulesNumeric.Id : (int?)null,
                //    ValidationMessage = "Numbers Only",
                //    VariableId = StrtNum.Id,
                //});

                this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = "",

                    Max = (int?)null,
                    Min = (int?)null,
                    //RegEx = @"^[0-9]*$",
                    RegEx = @"^[0-9]+$",// number only
                    //RegEx = @"^(\d*\.)?\d+$",// number with decimal point
                    ValidationId = validationRulesNumeric != null ? validationRulesNumeric.Id : (int?)null,
                    ValidationMessage = "Numbers Only",
                    VariableId = StrtNum2.Id,
                });

                this._dbContext.VariableValidationRules.Add(new VariableValidationRule()
                {
                    Guid = Guid.NewGuid(),
                    LimitType = "",

                    Max = (int?)null,
                    Min = (int?)null,
                    //RegEx = @"^[0-9]*$",
                    RegEx = @"^[0-9]+$",// number only
                    ValidationId = validationRulesNumeric != null ? validationRulesNumeric.Id : (int?)null,
                    ValidationMessage = "Numbers Only",
                    VariableId = Postcode.Id,
                });
                #endregion

                SaveChanges();



                var formCategoryUniqueIdentifier = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Unique_Identifier.ToString());
                var formCategoryEntityRegistration = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Entity_Registration.ToString());
                var formCategoryDefault = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Default.ToString());

                var PersonEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Person.ToString().Replace("__", "/"));
                var ParticipantEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Participant.ToString().Replace("__", "/"));
                var PlaceGroupEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Place__Group.ToString().Replace("__", "/"));
                var ProjectEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Project.ToString().Replace("__", "/"));

                var DefinitionAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " "));
                var SystemAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "));
                var ProjectAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Project_Admin.ToString().Replace("_", " "));
                var DataEntrySupervisorRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry_Supervisor.ToString().Replace("_", " "));
                var DataEntryOperatorRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry_Operator.ToString().Replace("_", " "));
                var DataEntryRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry.ToString().Replace("_", " "));

                #region form 1(PersonRegistration)
                #region form-1
                var PersonRegistrationForm = this._dbContext.Forms.Add(new Data.Form
                {
                    FormTitle = "Person Registration",
                    FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,
                    Guid = Guid.NewGuid(),
                    FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                    FormState = (int)Core.Enum.FormStateTypes.Published,
                    IsTemplate = false,
                    IsPublished = true,
                    TenantId = tenantId,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,


                    IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,

                    //PreviousVersion = null,
                    //Version = null,
                    //ApprovedBy = model.ApprovedBy,
                    //ApprovedDate = model.ApprovedDate,
                    //ProjectId = project != null ? project.Id : (int?)null,
                });
                #endregion
                SaveChanges();

                #region form 1-variables
                var PRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntID.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntID.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntID.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 1,
                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,
                });

                var PRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntGrp.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntGrp.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntGrp.ValidationMessage,
                });
                var PRegVariableEntType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntType.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntType.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntType.ValidationMessage,
                });

                var PRegVariablePerSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = PerSType.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = PerSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = PerSType.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 4,
                });

                var PRegVariableTitle = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Title.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Title.HelpText,
                    IsRequired = true,
                    ValidationMessage = Title.ValidationMessage,
                });

                var PRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Name.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Name.HelpText,
                    IsRequired = true,
                    ValidationMessage = Name.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 3,
                });

                var PRegVariableFirstName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = FirstName.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = FirstName.HelpText,
                    IsRequired = true,
                    ValidationMessage = FirstName.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 2,
                });

                var PRegVariablePrefName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = PrefName.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = PrefName.HelpText,
                    IsRequired = true,
                    ValidationMessage = PrefName.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });
                var PRegVariableUnit = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Unit.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Unit.HelpText,
                    IsRequired = true,
                    ValidationMessage = Unit.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });
                var PRegVariableStrtNum = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNum.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNum.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNum.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableStrtNme = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNme.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNme.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNme.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableStrtType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtType.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtType.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtType.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableSuburb = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Suburb.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Suburb.HelpText,
                    IsRequired = true,
                    ValidationMessage = Suburb.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableCountry = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Country.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Country.HelpText,
                    IsRequired = true,
                    ValidationMessage = Country.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableState = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = State.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = State.HelpText,
                    IsRequired = true,
                    ValidationMessage = State.ValidationMessage,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });
                var PRegVariablePostcode = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Postcode.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Postcode.HelpText,
                    IsRequired = true,
                    ValidationMessage = Postcode.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableDifAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = DifAddress.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = DifAddress.HelpText,
                    IsRequired = true,
                    ValidationMessage = DifAddress.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableStrtNum2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNum2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNum2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNum2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableStrtNme2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNme2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNme2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNme2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableStrtType2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtType2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtType2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtType2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });

                var PRegVariableSuburb2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Suburb2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Suburb2.HelpText,
                    IsRequired = true,
                    ValidationMessage = Suburb2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableState2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = State2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = State2.HelpText,
                    IsRequired = true,
                    ValidationMessage = State2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });

                var PRegVariablePostcode2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Postcode2.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Postcode2.HelpText,
                    IsRequired = true,
                    ValidationMessage = Postcode2.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });

                var PRegVariableNoAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = NoAddress.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = NoAddress.HelpText,
                    IsRequired = true,
                    ValidationMessage = NoAddress.ValidationMessage,

                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = createdBy,
                });


                var PRegVariableEmail = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Email.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Email.HelpText,
                    IsRequired = false,
                    ValidationMessage = Email.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 5,
                });

                var PRegVariablePhone = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Phone.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Phone.HelpText,
                    IsRequired = true,
                    ValidationMessage = Phone.ValidationMessage,
                });


                var PRegVariableUsername = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Username.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Username.HelpText,
                    IsRequired = true,
                    ValidationMessage = Username.ValidationMessage,
                });


                var PRegVariableSysAppr = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = SysAppr.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = SysAppr.HelpText,
                    IsRequired = true,
                    ValidationMessage = SysAppr.ValidationMessage,
                });

                var PRegVariableActive = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Active.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Active.HelpText,
                    IsRequired = true,
                    ValidationMessage = Active.ValidationMessage,
                });

                var PRegVariableSysRole = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = SysRole.Id,
                    FormId = PersonRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = SysRole.HelpText,
                    IsRequired = true,
                    ValidationMessage = SysRole.ValidationMessage,
                });

                #endregion
                SaveChanges();

                #region form 1-variable-role
                #region form variable1-roles
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DefinitionAdminRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = SystemAdminRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = ProjectAdminRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntrySupervisorRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryOperatorRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryRole.Id,
                //    FormVariableId = PRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                #endregion
                #region form variable2-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion                
                #region form variable3-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable4-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable5-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable6-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable7-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariablePrefName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable8-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable9-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable10-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable11-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable12-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable13-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable14-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable15-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable16-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable17-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable18-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable19-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable20-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable21-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable22-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable23-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable24-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion

                #region form variable29-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariablePhone.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion

                #region form variable25-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable26-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable27-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableActive.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable28-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PRegVariableSysRole.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion

                #endregion
                SaveChanges();

                #region form 1-entit type
                this._dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = PersonRegistrationForm.Id,
                    EntityTypeId = PersonEntityTyep != null ? PersonEntityTyep.Id : 0,
                });
                #endregion
                SaveChanges();
                #endregion


                #region form 2(Participant Registration)
                var ParticipantRegistrationForm = this._dbContext.Forms.Add(new Data.Form
                {
                    FormTitle = "Participant Registration",
                    FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                    Guid = Guid.NewGuid(),

                    FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                    FormState = (int)Core.Enum.FormStateTypes.Published,
                    IsTemplate = false,
                    IsPublished = true,
                    TenantId = tenantId,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                    //PreviousVersion = null,
                    //Version = null,
                    //ApprovedBy = model.ApprovedBy,
                    //ApprovedDate = model.ApprovedDate,
                    //ProjectId = project != null ? project.Id : (int?)null,

                });
                SaveChanges();

                #region form 2-variables
                var ParRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntID.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntID.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntID.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 1,
                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,
                });
                var ParRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntGrp.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntGrp.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntGrp.ValidationMessage,
                });

                var ParRegVariableTitle = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Title.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Title.HelpText,
                    IsRequired = true,
                    ValidationMessage = Title.ValidationMessage,
                });

                var ParRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Name.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Name.HelpText,
                    IsRequired = true,
                    ValidationMessage = Name.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 4,
                });


                var ParRegVariableFirstName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = FirstName.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = FirstName.HelpText,
                    IsRequired = true,
                    ValidationMessage = FirstName.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 2,
                });

                var ParRegVariableMiddleName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = MiddleName.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = MiddleName.HelpText,
                    IsRequired = false,
                    ValidationMessage = MiddleName.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 3,
                });

                var ParRegVariableNoMidNm = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = NoMidNm.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = NoMidNm.HelpText,
                    IsRequired = true,
                    ValidationMessage = NoMidNm.ValidationMessage,
                });
                var ParRegVariableDOB = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = DOB.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = DOB.HelpText,
                    IsRequired = true,
                    ValidationMessage = DOB.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 5,
                });

                var ParRegVariableGender = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Gender.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Gender.HelpText,
                    IsRequired = true,
                    ValidationMessage = Gender.ValidationMessage,
                    IsSearchVisible = true,
                    SearchPageOrder = 6,
                });

                var ParRegVariableUsername = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Username.Id,
                    FormId = ParticipantRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Username.HelpText,
                    IsRequired = true,
                    ValidationMessage = Username.ValidationMessage,
                });

                //var ParRegVariableSysUser = this._dbContext.FormVariables.Add(new Data.FormVariable
                //{
                //    Guid = Guid.NewGuid(),
                //    VariableId = SysUser.Id,
                //    FormId = ParticipantRegistrationForm.Id,
                //    ValidationRuleType = 0,
                //    HelpText = SysUser.HelpText,
                //    IsRequired = true,
                //    ValidationMessage = SysUser.ValidationMessage,
                //});


                #endregion
                SaveChanges();

                #region form 2-variable-role

                #region form variable1-roles
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DefinitionAdminRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = SystemAdminRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = ProjectAdminRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntrySupervisorRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryOperatorRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryRole.Id,
                //    FormVariableId = ParRegVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                #endregion
                #region form variable2-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable3-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableTitle.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable4-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable5-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableFirstName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable6-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableMiddleName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable7-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableNoMidNm.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable8-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableDOB.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable9-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableGender.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable10-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ParRegVariableUsername.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable11-roles
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DefinitionAdminRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = SystemAdminRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = ProjectAdminRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntrySupervisorRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryOperatorRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryRole.Id,
                //    FormVariableId = ParRegVariableSysUser.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                #endregion
                #endregion
                SaveChanges();

                #region form 2-entit type
                this._dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = ParticipantRegistrationForm.Id,
                    EntityTypeId = ParticipantEntityTyep != null ? ParticipantEntityTyep.Id : 0,
                });
                #endregion
                SaveChanges();

                #endregion


                #region form 3(PlaceGroupRegistration)
                var PlaceGroupRegistrationForm = this._dbContext.Forms.Add(new Data.Form
                {
                    FormTitle = "Place/Group Registration",
                    FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                    Guid = Guid.NewGuid(),

                    FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                    FormState = (int)Core.Enum.FormStateTypes.Published,
                    IsTemplate = false,
                    IsPublished = true,
                    TenantId = tenantId,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                    //PreviousVersion = null,
                    //Version = null,
                    //ApprovedBy = model.ApprovedBy,
                    //ApprovedDate = model.ApprovedDate,
                    //ProjectId = project != null ? project.Id : (int?)null,

                });
                SaveChanges();

                #region form 3-variables

                var PlcGrpRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntID.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntID.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntID.ValidationMessage,

                    IsSearchVisible = true,
                    SearchPageOrder = 1,

                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,
                });

                var PlcGrpRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntGrp.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntGrp.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntGrp.ValidationMessage,
                });

                var PlcGrpRegVariableEntType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntType.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntType.ValidationMessage,

                    IsSearchVisible = true,
                    SearchPageOrder = 3,
                });
                var PlcGrpRegVariablePerSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = PerSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = PerSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = PerSType.ValidationMessage,
                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "1" : null,

                });
                var PlcGrpRegVariableHospSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = HospSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = HospSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = HospSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "2" : null,
                });

                var PlcGrpRegVariablePracSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = PracSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = PracSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = PracSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "3" : null,
                });

                var PlcGrpRegVariableLabSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = LabSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = LabSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = LabSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "4" : null,
                });

                var PlcGrpRegVariableGovSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = GovSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = GovSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = GovSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "8" : null,
                });


                var PlcGrpRegVariableIndSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = IndSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = IndSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = IndSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "9" : null,
                });

                var PlcGrpRegVariableConSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = ConSType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = ConSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = ConSType.ValidationMessage,

                    DependentVariableId = EntType != null ? EntType.Id : (int?)null,
                    ResponseOption = EntType != null ? "10" : null,
                });

                var PlcGrpRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Name.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Name.HelpText,
                    IsRequired = true,
                    ValidationMessage = Name.ValidationMessage,

                    IsSearchVisible = true,
                    SearchPageOrder = 2,
                });

                var PlcGrpRegVariableUnit = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Unit.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Unit.HelpText,
                    IsRequired = true,
                    ValidationMessage = Unit.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtNum = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNum.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNum.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNum.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtNme = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNme.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNme.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNme.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtType.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtType.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtType.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });
                var PlcGrpRegVariableSuburb = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Suburb.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Suburb.HelpText,
                    IsRequired = true,
                    ValidationMessage = Suburb.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });
                var PlcGrpRegVariableCountry = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Country.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Country.HelpText,
                    IsRequired = false,
                    ValidationMessage = Country.ValidationMessage,

                    //DeactivatedBy = createdBy,
                    //DateDeactivated = DateTime.UtcNow,

                    IsSearchVisible = true,
                    SearchPageOrder = 4,
                });
                var PlcGrpRegVariableState = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = State.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = State.HelpText,
                    IsRequired = false,
                    ValidationMessage = State.ValidationMessage,

                    //DeactivatedBy = createdBy,
                    //DateDeactivated = DateTime.UtcNow,

                    IsSearchVisible = true,
                    SearchPageOrder = 5,
                });

                var PlcGrpRegVariablePostcode = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Postcode.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Postcode.HelpText,
                    IsRequired = false,
                    ValidationMessage = Postcode.ValidationMessage,

                    //DeactivatedBy = createdBy,
                    //DateDeactivated = DateTime.UtcNow,

                    IsSearchVisible = true,
                    SearchPageOrder = 6,
                });

                var PlcGrpRegVariableDifAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = DifAddress.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = DifAddress.HelpText,
                    IsRequired = true,
                    ValidationMessage = DifAddress.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtNum2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNum2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNum2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNum2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtNme2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtNme2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtNme2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtNme2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableStrtType2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = StrtType2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = StrtType2.HelpText,
                    IsRequired = true,
                    ValidationMessage = StrtType2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableSuburb2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Suburb2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Suburb2.HelpText,
                    IsRequired = true,
                    ValidationMessage = Suburb2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });
                var PlcGrpRegVariableCountry2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Country2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Country2.HelpText,
                    IsRequired = true,
                    ValidationMessage = Country2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });
                var PlcGrpRegVariableState2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = State2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = State2.HelpText,
                    IsRequired = true,
                    ValidationMessage = State2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });
                var PlcGrpRegVariablePostcode2 = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Postcode2.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Postcode2.HelpText,
                    IsRequired = true,
                    ValidationMessage = Postcode2.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableNoAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = NoAddress.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = NoAddress.HelpText,
                    IsRequired = true,
                    ValidationMessage = NoAddress.ValidationMessage,

                    DeactivatedBy = createdBy,
                    DateDeactivated = DateTime.UtcNow,
                });

                var PlcGrpRegVariableEmail = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Email.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Email.HelpText,
                    IsRequired = true,
                    ValidationMessage = Email.ValidationMessage,


                });

                var PlcGrpRegVariableSysAppr = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = SysAppr.Id,
                    FormId = PlaceGroupRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = SysAppr.HelpText,
                    IsRequired = true,
                    ValidationMessage = SysAppr.ValidationMessage,
                });
                SaveChanges();
                #endregion

                #region form 3-variable-role
                #region form variable0-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableEntID.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion


                #region form variable2-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable3-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableEntType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable1-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariablePerSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable4-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableHospSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable5-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariablePracSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable6-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableLabSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable7-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableGovSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable8-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableIndSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable9-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableConSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable10-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable11-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableUnit.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable12-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable13-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable14-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable15-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable16-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable17-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableState.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable18-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable19-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableDifAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable20-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable21-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable22-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableStrtType2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable23-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableSuburb2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable24-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableCountry2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable25-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableState2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable26-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariablePostcode2.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable27-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableNoAddress.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable28-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableEmail.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable29-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = PlcGrpRegVariableSysAppr.Id,
                    CanCreate = false,
                    CanView = true,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion

                #endregion
                SaveChanges();

                #region form 3-entit type
                this._dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = PlaceGroupRegistrationForm.Id,
                    EntityTypeId = PlaceGroupEntityTyep != null ? PlaceGroupEntityTyep.Id : 0,
                });
                #endregion
                SaveChanges();
                #endregion


                #region form 4(Project Registration)
                var ProjectRegistrationForm = this._dbContext.Forms.Add(new Data.Form
                {
                    FormTitle = "Project Registration",
                    FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                    Guid = Guid.NewGuid(),

                    FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                    FormState = (int)Core.Enum.FormStateTypes.Published,
                    IsTemplate = false,
                    IsPublished = true,
                    TenantId = tenantId,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                    //PreviousVersion = null,
                    //Version = null,
                    //ApprovedBy = model.ApprovedBy,
                    //ApprovedDate = model.ApprovedDate,
                    //ProjectId = project != null ? project.Id : (int?)null,

                });
                SaveChanges();

                #region form 4-variables
                var ProjectRegFormVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntID.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntID.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntID.ValidationMessage,
                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,
                });
                var ProjectRegFormVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntGrp.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntGrp.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntGrp.ValidationMessage,
                });

                var ProjectRegFormVariableProSType = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = ProSType.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = ProSType.HelpText,
                    IsRequired = true,
                    ValidationMessage = ProSType.ValidationMessage,
                });

                var ProjectRegFormVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Name.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Name.HelpText,
                    IsRequired = true,
                    ValidationMessage = Name.ValidationMessage,
                });

                var ProjectRegFormVariableConfData = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = ConfData.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = ConfData.HelpText,
                    IsRequired = true,
                    ValidationMessage = ConfData.ValidationMessage,
                });

                var ProjectRegFormVariableCnstModel = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = CnstModel.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = CnstModel.HelpText,
                    IsRequired = true,
                    ValidationMessage = CnstModel.ValidationMessage,
                });
                var ProjectRegFormVariableEthics = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = Ethics.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = Ethics.HelpText,
                    IsRequired = true,
                    ValidationMessage = Ethics.ValidationMessage,
                });
                var ProjectRegFormVariableDataStore = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = DataStore.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = DataStore.HelpText,
                    IsRequired = true,
                    ValidationMessage = DataStore.ValidationMessage,
                });
                var ProjectRegFormVariableProDt = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = ProDt.Id,
                    FormId = ProjectRegistrationForm.Id,
                    ValidationRuleType = 0,
                    HelpText = ProDt.HelpText,
                    IsRequired = true,
                    ValidationMessage = ProDt.ValidationMessage,
                });







                #endregion
                SaveChanges();

                #region form 4-variable-role                
                #region form variable1-roles
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DefinitionAdminRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = SystemAdminRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = ProjectAdminRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntrySupervisorRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryOperatorRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryRole.Id,
                //    FormVariableId = ProjectRegFormVariableEntID.Id,
                //    CanCreate = false,
                //    CanView = false,
                //    CanEdit = false,
                //    CanDelete = false,
                //});
                #endregion
                #region form variable2-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableEntGrp.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable3-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableProSType.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable4-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableName.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable5-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableConfData.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable6-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableCnstModel.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable7-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableEthics.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable8-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableDataStore.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #region form variable9-roles
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DefinitionAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = SystemAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = true,
                    CanView = true,
                    CanEdit = true,
                    CanDelete = true,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = ProjectAdminRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntrySupervisorRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryOperatorRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                _dbContext.FormVariableRoles.Add(new FormVariableRole()
                {
                    Guid = Guid.NewGuid(),
                    RoleId = DataEntryRole.Id,
                    FormVariableId = ProjectRegFormVariableProDt.Id,
                    CanCreate = false,
                    CanView = false,
                    CanEdit = false,
                    CanDelete = false,
                });
                #endregion
                #endregion
                SaveChanges();

                #region form 4-entit type
                this._dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = ProjectRegistrationForm.Id,
                    EntityTypeId = ProjectEntityTyep != null ? ProjectEntityTyep.Id : 0,
                });
                #endregion
                SaveChanges();
                #endregion


                #region form 5(Unique Identifier)
                var UniqueIdentifierForm = this._dbContext.Forms.Add(new Data.Form
                {
                    FormTitle = "Unique Identifier",
                    FormCategoryId = formCategoryDefault != null ? formCategoryDefault.Id : (int?)null,

                    Guid = Guid.NewGuid(),

                    FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                    FormState = (int)Core.Enum.FormStateTypes.Published,
                    IsTemplate = false,
                    IsPublished = true,
                    TenantId = tenantId,
                    CreatedBy = createdBy,
                    CreatedDate = DateTime.UtcNow,
                    IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                    //PreviousVersion = null,
                    //Version = null,
                    //ApprovedBy = model.ApprovedBy,
                    //ApprovedDate = model.ApprovedDate,
                    //ProjectId = project != null ? project.Id : (int?)null,

                });
                SaveChanges();

                #region form 5-variables
                var UniqueIdFormFormVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
                {
                    Guid = Guid.NewGuid(),
                    VariableId = EntID.Id,
                    FormId = UniqueIdentifierForm.Id,
                    ValidationRuleType = 0,
                    HelpText = EntID.HelpText,
                    IsRequired = true,
                    ValidationMessage = EntID.ValidationMessage,
                    //MaxRange = variable.MaxRange,
                    //MinRange = variable.MinRange,
                    //RegEx = variable.RegEx,
                    //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                    //ResponseOption = variable.ResponseOption,
                });
                #endregion
                SaveChanges();

                #region form 5-variable-role                
                #region form variable1-roles
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DefinitionAdminRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = SystemAdminRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = ProjectAdminRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntrySupervisorRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryOperatorRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                //_dbContext.FormVariableRoles.Add(new FormVariableRole()
                //{
                //    Guid = Guid.NewGuid(),
                //    RoleId = DataEntryRole.Id,
                //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
                //    CanCreate = true,
                //    CanView = true,
                //    CanEdit = true,
                //    CanDelete = true,
                //});
                #endregion
                #endregion
                SaveChanges();

                #region form 5-entit type
                this._dbContext.FormEntityTypes.Add(new FormEntityType()
                {
                    Guid = Guid.NewGuid(),
                    FormId = UniqueIdentifierForm.Id,
                    EntityTypeId = PlaceGroupEntityTyep != null ? PlaceGroupEntityTyep.Id : 0,
                });
                #endregion
                #endregion
                SaveChanges();



















            }
            #endregion

            #region Projects 

            if (!this._dbContext.Projects.Any())
            {
                var project = new Data.Project
                {
                    Guid = Guid.NewGuid(),
                    ProjectName = "Test Project",
                    ProjectStatusId = (int)Core.Enum.ProjectStatusTypes.Active,
                    StartDate = DateTime.UtcNow.Date,
                    EndDate = DateTime.UtcNow.AddYears(1),
                    CreatedBy = this._dbContext.UserLogins.First(ul => ul.Email == "projectadmin@aspree.com").Id,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = tenantId
                };

                this._dbContext.Projects.Add(project);

                SaveChanges();

                this._dbContext.ProjectSites.Add(new ProjectSite()
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = project.Id,
                    SiteId = this._dbContext.Sites.First().Id
                });

                var projectStaffMembersRoles = _dbContext.Roles.Where(x => x.Name == "System Admin").ToList();
                var projectStaffMembersId = _dbContext.UserLogins.FirstOrDefault(x => x.Email == "systemadmin@aspree.com").Id;
                foreach (var projUser in projectStaffMembersRoles)
                {
                    //var user = dbContext.UserLogins.FirstOrDefault(v => v.Guid == projUser.UserGuid);
                    //var role = dbContext.Roles.FirstOrDefault(v => v.Guid == projUser.RoleGuid);
                    var projectstaffRoles = new ProjectStaffMemberRole()
                    {
                        Guid = Guid.NewGuid(),
                        ProjectId = project.Id,
                        UserId = projectStaffMembersId,
                        RoleId = projUser.Id,
                        //UserId = user.Id,
                        //RoleId = role.Id,
                        CreatedBy = createdBy,
                        StaffCreatedDate = DateTime.UtcNow,
                    };
                    _dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);
                }


                SaveChanges();

                CreateDefaultFormsForProject(project.Id, createdBy, tenantId);

                CreateDefaultActivitiesForProject(project.Id, createdBy, tenantId);
            }

            #endregion Projects 
        }
        public PrivilegeSmallViewModel ToModel(Data.Privilege entity)
        {
            return new PrivilegeSmallViewModel()
            {
                Guid = entity.Guid,
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public PrivilegeSmallViewModel Update(PrivilegeSmallViewModel model)
        {
            throw new NotImplementedException();
        }


        public bool CreateDefaultFormsForProject(int projectId, int createdBy, int tenantId)
        {
            var variablesQry = _dbContext.Variables.Where(vr => vr.IsDefaultVariable == (int)Core.Enum.DefaultVariableType.Default);

            var formCategoryUniqueIdentifier = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Unique_Identifier.ToString());
            var formCategoryEntityRegistration = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Entity_Registration.ToString());
            var formCategoryDefault = this._dbContext.FormCategories.FirstOrDefault(et => et.CategoryName.Replace(" ", "_") == Core.Enum.FormCategories.Default.ToString());

            var PersonEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Person.ToString().Replace("__", "/"));
            var ParticipantEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Participant.ToString().Replace("__", "/"));
            var PlaceGroupEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Place__Group.ToString().Replace("__", "/"));
            var ProjectEntityTyep = this._dbContext.EntityTypes.FirstOrDefault(et => et.Name == Core.Enum.EntityTypes.Project.ToString().Replace("__", "/"));

            var DefinitionAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Definition_Admin.ToString().Replace("_", " "));
            var SystemAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "));
            var ProjectAdminRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Project_Admin.ToString().Replace("_", " "));
            var DataEntrySupervisorRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry_Supervisor.ToString().Replace("_", " "));
            var DataEntryOperatorRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry_Operator.ToString().Replace("_", " "));
            var DataEntryRole = this._dbContext.Roles.FirstOrDefault(rl => rl.Name == Core.Enum.RoleTypes.Data_Entry.ToString().Replace("_", " "));


            var dependentVarEntType = this._dbContext.Variables.FirstOrDefault(vr => vr.VariableName == "EntType");


            #region form 1(PersonRegistration)
            #region form-1
            var PersonRegistrationForm = this._dbContext.Forms.Add(new Data.Form
            {
                FormTitle = "Person Registration",
                FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,
                Guid = Guid.NewGuid(),
                FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                FormState = (int)Core.Enum.FormStateTypes.Published,
                IsTemplate = false,
                IsPublished = true,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,

                //PreviousVersion = null,
                //Version = null,
                //ApprovedBy = model.ApprovedBy,
                //ApprovedDate = model.ApprovedDate,
                ProjectId = projectId,
            });
            #endregion
            SaveChanges();

            #region form 1-variables
            var PRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "EntID").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "EntID").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "EntID").ValidationMessage,

                IsSearchVisible = true,
                SearchPageOrder = 1,
                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });

            var PRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "EntGrp").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "EntGrp").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "EntGrp").ValidationMessage,
            });
            var PRegVariableEntType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "EntType").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "EntType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "EntType").ValidationMessage,
            });

            var PRegVariablePerSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "PerSType").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "PerSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "PerSType").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 4,
            });

            var PRegVariableTitle = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Title").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Title").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Title").ValidationMessage,
            });

            var PRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Name").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Name").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Name").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 3,
            });

            var PRegVariableFirstName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "FirstName").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "FirstName").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "FirstName").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 2,
            });

            var PRegVariablePrefName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "PrefName").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "PrefName").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "PrefName").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,

            });
            var PRegVariableUnit = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Unit").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Unit").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Unit").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PRegVariableStrtNum = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableStrtNme = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableStrtType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableSuburb = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableCountry = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Country").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Country").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Country").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableState = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "State").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "State").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "State").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PRegVariablePostcode = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableDifAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "DifAddress").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "DifAddress").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "DifAddress").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableStrtNum2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNum2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableStrtNme2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtNme2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableStrtType2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "StrtType2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PRegVariableSuburb2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Suburb2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableState2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "State2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "State2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "State2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PRegVariablePostcode2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode2").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Postcode2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PRegVariableNoAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "NoAddress").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "NoAddress").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "NoAddress").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });


            var PRegVariableEmail = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Email").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Email").HelpText,
                IsRequired = false,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Email").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 5,
            });

            var PRegVariablePhone = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Phone").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Phone").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Phone").ValidationMessage,
            });

            var PRegVariableUsername = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Username").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Username").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Username").ValidationMessage,
            });


            var PRegVariableSysAppr = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "SysAppr").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "SysAppr").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "SysAppr").ValidationMessage,
            });

            var PRegVariableActive = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "Active").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "Active").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "Active").ValidationMessage,
            });

            var PRegVariableSysRole = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(c => c.VariableName == "SysRole").Id,
                FormId = PersonRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(c => c.VariableName == "SysRole").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(c => c.VariableName == "SysRole").ValidationMessage,
            });
            #endregion
            SaveChanges();

            #region form 1-variable-role
            #region form variable1-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable2-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable3-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable4-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable4-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable5-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable6-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable7-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariablePrefName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable8-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable9-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable10-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable11-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable12-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable13-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable14-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable15-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable16-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable17-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable18-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable19-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable20-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable21-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable22-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable23-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable24-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable29-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariablePhone.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable25-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable26-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable27-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableActive.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable28-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PRegVariableSysRole.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #endregion
            SaveChanges();

            #region form 1-entit type
            this._dbContext.FormEntityTypes.Add(new FormEntityType()
            {
                Guid = Guid.NewGuid(),
                FormId = PersonRegistrationForm.Id,
                EntityTypeId = PersonEntityTyep != null ? PersonEntityTyep.Id : 0,
            });
            #endregion
            SaveChanges();
            #endregion


            #region form 2(Participant Registration)
            var ParticipantRegistrationForm = this._dbContext.Forms.Add(new Data.Form
            {
                FormTitle = "Participant Registration",
                FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                Guid = Guid.NewGuid(),

                FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                FormState = (int)Core.Enum.FormStateTypes.Published,
                IsTemplate = false,
                IsPublished = true,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                //PreviousVersion = null,
                //Version = null,
                //ApprovedBy = model.ApprovedBy,
                //ApprovedDate = model.ApprovedDate,
                ProjectId = projectId,

            });
            SaveChanges();

            #region form 2-variables
            var ParRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 1,
                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });
            var ParRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").ValidationMessage,
            });

            var ParRegVariableTitle = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Title").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Title").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Title").ValidationMessage,
            });

            var ParRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Name").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Name").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Name").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 4,
            });


            var ParRegVariableFirstName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "FirstName").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "FirstName").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "FirstName").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 2,
            });

            var ParRegVariableMiddleName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "MiddleName").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "MiddleName").HelpText,
                IsRequired = false,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "MiddleName").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 3,
            });

            var ParRegVariableNoMidNm = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "NoMidNm").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "NoMidNm").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "NoMidNm").ValidationMessage,
            });
            var ParRegVariableDOB = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "DOB").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "DOB").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "DOB").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 5,
            });

            var ParRegVariableGender = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Gender").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Gender").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Gender").ValidationMessage,
                IsSearchVisible = true,
                SearchPageOrder = 6,
            });

            var ParRegVariableUsername = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Username").Id,
                FormId = ParticipantRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Username").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Username").ValidationMessage,
            });

            //var ParRegVariableSysUser = this._dbContext.FormVariables.Add(new Data.FormVariable
            //{
            //    Guid = Guid.NewGuid(),
            //    VariableId = SysUser.Id,
            //    FormId = ParticipantRegistrationForm.Id,
            //    ValidationRuleType = 0,
            //    HelpText = SysUser.HelpText,
            //    IsRequired = true,
            //    ValidationMessage = SysUser.ValidationMessage,
            //});


            #endregion
            SaveChanges();

            #region form 2-variable-role

            #region form variable1-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable2-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable3-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableTitle.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable4-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable5-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableFirstName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable6-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableMiddleName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable7-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableNoMidNm.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable8-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableDOB.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable9-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableGender.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable10-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ParRegVariableUsername.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable11-roles
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DefinitionAdminRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = SystemAdminRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = ProjectAdminRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntrySupervisorRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntryOperatorRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntryRole.Id,
            //    FormVariableId = ParRegVariableSysUser.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            #endregion
            #endregion
            SaveChanges();

            #region form 2-entit type
            this._dbContext.FormEntityTypes.Add(new FormEntityType()
            {
                Guid = Guid.NewGuid(),
                FormId = ParticipantRegistrationForm.Id,
                EntityTypeId = ParticipantEntityTyep != null ? ParticipantEntityTyep.Id : 0,
            });
            #endregion
            SaveChanges();

            #endregion


            #region form 3(PlaceGroupRegistration)
            var PlaceGroupRegistrationForm = this._dbContext.Forms.Add(new Data.Form
            {
                FormTitle = "Place/Group Registration",
                FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                Guid = Guid.NewGuid(),

                FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                FormState = (int)Core.Enum.FormStateTypes.Published,
                IsTemplate = false,
                IsPublished = true,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                //PreviousVersion = null,
                //Version = null,
                //ApprovedBy = model.ApprovedBy,
                //ApprovedDate = model.ApprovedDate,
                ProjectId = projectId,

            });
            SaveChanges();

            #region form 3-variables

            var PlcGrpRegVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").ValidationMessage,

                IsSearchVisible = true,
                SearchPageOrder = 1,

                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });

            var PlcGrpRegVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").ValidationMessage,
            });

            var PlcGrpRegVariableEntType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntType").ValidationMessage,

                IsSearchVisible = true,
                SearchPageOrder = 3,
            });
            var PlcGrpRegVariablePerSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "PerSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "PerSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "PerSType").ValidationMessage,
                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "1" : null,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });
            var PlcGrpRegVariableHospSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "HospSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "HospSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "HospSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "2" : null,
            });

            var PlcGrpRegVariablePracSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "PracSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "PracSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "PracSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "3" : null,
            });

            var PlcGrpRegVariableLabSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "LabSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "LabSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "LabSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "4" : null,
            });

            var PlcGrpRegVariableGovSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "GovSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "GovSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "GovSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "8" : null,
            });


            var PlcGrpRegVariableIndSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "IndSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "IndSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "IndSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "9" : null,
            });

            var PlcGrpRegVariableConSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "ConSType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "ConSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "ConSType").ValidationMessage,

                DependentVariableId = dependentVarEntType != null ? dependentVarEntType.Id : (int?)null,
                ResponseOption = dependentVarEntType != null ? "10" : null,
            });

            var PlcGrpRegVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Name").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Name").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Name").ValidationMessage,

                IsSearchVisible = true,
                SearchPageOrder = 2,
            });

            var PlcGrpRegVariableUnit = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Unit").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Unit").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Unit").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtNum = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtNme = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PlcGrpRegVariableSuburb = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PlcGrpRegVariableCountry = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Country").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Country").HelpText,
                IsRequired = false,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Country").ValidationMessage,

                //DeactivatedBy = createdBy,
                //DateDeactivated = DateTime.UtcNow,

                IsSearchVisible = true,
                SearchPageOrder = 4,
            });
            var PlcGrpRegVariableState = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "State").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "State").HelpText,
                IsRequired = false,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "State").ValidationMessage,

                //DeactivatedBy = createdBy,
                //DateDeactivated = DateTime.UtcNow,
                IsSearchVisible = true,
                SearchPageOrder = 5,
            });

            var PlcGrpRegVariablePostcode = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode").HelpText,
                IsRequired = false,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode").ValidationMessage,

                //DeactivatedBy = createdBy,
                //DateDeactivated = DateTime.UtcNow,

                IsSearchVisible = true,
                SearchPageOrder = 6,
            });

            var PlcGrpRegVariableDifAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "DifAddress").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "DifAddress").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "DifAddress").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtNum2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNum2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtNme2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtNme2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableStrtType2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "StrtType2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableSuburb2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Suburb2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PlcGrpRegVariableCountry2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Country2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Country2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Country2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PlcGrpRegVariableState2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "State2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "State2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "State2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });
            var PlcGrpRegVariablePostcode2 = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode2").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode2").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Postcode2").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableNoAddress = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "NoAddress").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "NoAddress").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "NoAddress").ValidationMessage,

                DeactivatedBy = createdBy,
                DateDeactivated = DateTime.UtcNow,
            });

            var PlcGrpRegVariableEmail = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Email").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Email").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Email").ValidationMessage,
            });

            var PlcGrpRegVariableSysAppr = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "SysAppr").Id,
                FormId = PlaceGroupRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "SysAppr").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "SysAppr").ValidationMessage,
            });
            SaveChanges();
            #endregion

            #region form 3-variable-role                                
            #region form variable1-roles

            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DefinitionAdminRole.Id,
            //    FormVariableId = PlcGrpRegVariableEntID.Id,
            //    CanCreate = false,
            //    CanView = true,
            //    CanEdit = false,
            //    CanDelete = false,
            //});

            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable2-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable3-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableEntType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable1-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariablePerSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #region form variable4-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableHospSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable5-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariablePracSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable6-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableLabSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable7-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableGovSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable8-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableIndSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable9-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableConSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable10-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable11-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableUnit.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable12-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable13-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable14-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable15-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable16-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableCountry.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable17-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableState.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable18-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable19-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableDifAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable20-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNum2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable21-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtNme2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable22-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableStrtType2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable23-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableSuburb2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable24-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableCountry2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable25-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableState2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable26-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariablePostcode2.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable27-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableNoAddress.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable28-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableEmail.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable29-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = PlcGrpRegVariableSysAppr.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion

            #endregion
            SaveChanges();

            #region form 3-entit type
            this._dbContext.FormEntityTypes.Add(new FormEntityType()
            {
                Guid = Guid.NewGuid(),
                FormId = PlaceGroupRegistrationForm.Id,
                EntityTypeId = PlaceGroupEntityTyep != null ? PlaceGroupEntityTyep.Id : 0,
            });
            #endregion
            SaveChanges();
            #endregion


            #region form 4(Project Registration)
            var ProjectRegistrationForm = this._dbContext.Forms.Add(new Data.Form
            {
                FormTitle = "Project Registration",
                FormCategoryId = formCategoryEntityRegistration != null ? formCategoryEntityRegistration.Id : (int?)null,

                Guid = Guid.NewGuid(),

                FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                FormState = (int)Core.Enum.FormStateTypes.Published,
                IsTemplate = false,
                IsPublished = true,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                //PreviousVersion = null,
                //Version = null,
                //ApprovedBy = model.ApprovedBy,
                //ApprovedDate = model.ApprovedDate,
                ProjectId = projectId,

            });
            SaveChanges();

            #region form 4-variables
            var ProjectRegFormVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").ValidationMessage,
                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });
            var ProjectRegFormVariableEntGrp = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntGrp").ValidationMessage,
            });

            var ProjectRegFormVariableProSType = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "ProSType").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "ProSType").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "ProSType").ValidationMessage,
            });

            var ProjectRegFormVariableName = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Name").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Name").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Name").ValidationMessage,
            });

            var ProjectRegFormVariableConfData = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "ConfData").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "ConfData").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "ConfData").ValidationMessage,
            });

            var ProjectRegFormVariableCnstModel = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "CnstModel").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "CnstModel").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "CnstModel").ValidationMessage,
            });
            var ProjectRegFormVariableEthics = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "Ethics").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "Ethics").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "Ethics").ValidationMessage,
            });
            var ProjectRegFormVariableDataStore = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "DataStore").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "DataStore").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "DataStore").ValidationMessage,
            });
            var ProjectRegFormVariableProDt = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "ProDt").Id,
                FormId = ProjectRegistrationForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "ProDt").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "ProDt").ValidationMessage,
            });







            #endregion
            SaveChanges();

            #region form 4-variable-role                
            #region form variable1-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableEntID.Id,
                CanCreate = false,
                CanView = true,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable2-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableEntGrp.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable3-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableProSType.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable4-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableName.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable5-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableConfData.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable6-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableCnstModel.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable7-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableEthics.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable8-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableDataStore.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #region form variable9-roles
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DefinitionAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = SystemAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = true,
                CanView = true,
                CanEdit = true,
                CanDelete = true,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = ProjectAdminRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntrySupervisorRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryOperatorRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            _dbContext.FormVariableRoles.Add(new FormVariableRole()
            {
                Guid = Guid.NewGuid(),
                RoleId = DataEntryRole.Id,
                FormVariableId = ProjectRegFormVariableProDt.Id,
                CanCreate = false,
                CanView = false,
                CanEdit = false,
                CanDelete = false,
            });
            #endregion
            #endregion
            SaveChanges();

            #region form 4-entit type
            this._dbContext.FormEntityTypes.Add(new FormEntityType()
            {
                Guid = Guid.NewGuid(),
                FormId = ProjectRegistrationForm.Id,
                EntityTypeId = ProjectEntityTyep != null ? ProjectEntityTyep.Id : 0,
            });
            #endregion
            SaveChanges();
            #endregion


            #region form 5(Unique Identifier)
            var UniqueIdentifierForm = this._dbContext.Forms.Add(new Data.Form
            {
                FormTitle = "Unique Identifier",
                FormCategoryId = formCategoryDefault != null ? formCategoryDefault.Id : (int?)null,

                Guid = Guid.NewGuid(),

                FormStatusId = (int)Core.Enum.FormStatusTypes.Published,
                FormState = (int)Core.Enum.FormStateTypes.Published,
                IsTemplate = false,
                IsPublished = true,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Default,
                //PreviousVersion = null,
                //Version = null,
                //ApprovedBy = model.ApprovedBy,
                //ApprovedDate = model.ApprovedDate,
                ProjectId = projectId,

            });
            SaveChanges();

            #region form 5-variables
            var UniqueIdFormFormVariableEntID = this._dbContext.FormVariables.Add(new Data.FormVariable
            {
                Guid = Guid.NewGuid(),
                VariableId = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").Id,
                FormId = UniqueIdentifierForm.Id,
                ValidationRuleType = 0,
                HelpText = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").HelpText,
                IsRequired = true,
                ValidationMessage = variablesQry.FirstOrDefault(v => v.VariableName == "EntID").ValidationMessage,
                //MaxRange = variable.MaxRange,
                //MinRange = variable.MinRange,
                //RegEx = variable.RegEx,
                //DependentVariableId = dependentVariableid != null ? dependentVariableid.Id : (int?)null,
                //ResponseOption = variable.ResponseOption,
            });
            #endregion
            SaveChanges();

            #region form 5-variable-role                
            #region form variable1-roles
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DefinitionAdminRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = SystemAdminRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = ProjectAdminRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntrySupervisorRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntryOperatorRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            //_dbContext.FormVariableRoles.Add(new FormVariableRole()
            //{
            //    Guid = Guid.NewGuid(),
            //    RoleId = DataEntryRole.Id,
            //    FormVariableId = UniqueIdFormFormVariableEntID.Id,
            //    CanCreate = true,
            //    CanView = true,
            //    CanEdit = true,
            //    CanDelete = true,
            //});
            #endregion
            #endregion
            SaveChanges();

            #region form 5-entit type
            this._dbContext.FormEntityTypes.Add(new FormEntityType()
            {
                Guid = Guid.NewGuid(),
                FormId = UniqueIdentifierForm.Id,
                EntityTypeId = PlaceGroupEntityTyep != null ? PlaceGroupEntityTyep.Id : 0,
            });
            #endregion
            #endregion
            SaveChanges();

            return true;
        }

        public bool CreateDefaultActivitiesForProject(int projectId, int createdBy, int tenantId)
        {
            var activityCategory = this._dbContext.ActivityCategories.FirstOrDefault(et => et.CategoryName == Core.Enum.ActivityCategories.Default.ToString());
            var acitvityStatus = this._dbContext.ActivityStatus.FirstOrDefault(et => et.Status == "Draft");
            var activityRoles = _dbContext.Roles.ToList();

            #region Person Registration Activity

            #region save person registration activities
            var personRegistrationActivity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = "Person Registration",
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                EndDate = new DateTime(2099, 12, 31),
                RepeatationCount = 0,
                RepeatationType = 1,
                ScheduleType = 1,//model.ScheduleType,
                StartDate = DateTime.UtcNow,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Default,
                //DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                //RepeatationOffset = model.RepeatationOffset,
            };
            _dbContext.Activities.Add(personRegistrationActivity);
            #endregion

            #region person registration activity entity type            
            var personregistrationentityTypes = (from c in _dbContext.EntityTypes
                                                 where (
                                                 c.Name == "Person"
                                                 //|| c.Name == "Participant"
                                                 //|| c.Name == "Place/Group"
                                                 //|| c.Name == "Project"
                                                 )
                                                 select c.Id).ToList();

            foreach (var entityType in personregistrationentityTypes)
            {
                this._dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = personRegistrationActivity.Id,
                    EntityTypeId = entityType,
                });
            }
            #endregion

            #region activity roles

            foreach (var activityRole in activityRoles)
            {
                this._dbContext.ActivityRoles.Add(new ActivityRole()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = personRegistrationActivity.Id,
                    RoleId = activityRole.Id
                });
            }
            #endregion

            #region activity forms
            var personRegistrationActivityForms = _dbContext.Forms.Where(x => x.ProjectId == projectId && x.FormTitle == "Person Registration");
            foreach (var form in personRegistrationActivityForms)
            {
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    ActivityId = personRegistrationActivity.Id
                };
                _dbContext.ActivityForms.Add(activityForm);
            }
            #endregion

            //foreach (var formRole in form.Roles)
            //{
            //        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole);
            //        dbContext.ActivityFormRoles.Add(new ActivityFormRole()
            //        {
            //            Guid = Guid.NewGuid(),
            //            RoleId = roleDb.Id,
            //            ActivityFormId = activityForm.Id
            //        });
            //    
            //}


            SaveChanges();
            #endregion

            #region Participant Registration Activity

            #region save participant registration activities
            var participantRegistrationActivity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = "Participant Registration",
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                EndDate = new DateTime(2099, 12, 31),
                RepeatationCount = 0,
                RepeatationType = 1,
                ScheduleType = 1,//model.ScheduleType,
                StartDate = DateTime.UtcNow,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Default,
                //DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                //RepeatationOffset = model.RepeatationOffset,
            };
            _dbContext.Activities.Add(participantRegistrationActivity);
            #endregion

            #region person pegistration activity entity type            
            var participantregistrationentityTypes = (from c in _dbContext.EntityTypes
                                                      where (
                                                      //c.Name == "Person"
                                                      c.Name == "Participant"
                                                      //|| c.Name == "Place/Group"
                                                      //|| c.Name == "Project"
                                                      )
                                                      select c.Id).ToList();

            foreach (var entityType in participantregistrationentityTypes)
            {
                this._dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = participantRegistrationActivity.Id,
                    EntityTypeId = entityType,
                });
            }
            #endregion

            #region activity roles            
            foreach (var activityRole in activityRoles)
            {
                this._dbContext.ActivityRoles.Add(new ActivityRole()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = participantRegistrationActivity.Id,
                    RoleId = activityRole.Id
                });
            }
            #endregion

            #region activity forms
            var participantregistrationactivityForms = _dbContext.Forms.Where(x => x.ProjectId == projectId && x.FormTitle == "Participant Registration");
            foreach (var form in participantregistrationactivityForms)
            {
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    ActivityId = participantRegistrationActivity.Id
                };
                _dbContext.ActivityForms.Add(activityForm);
            }
            #endregion

            //foreach (var formRole in form.Roles)
            //{
            //        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole);
            //        dbContext.ActivityFormRoles.Add(new ActivityFormRole()
            //        {
            //            Guid = Guid.NewGuid(),
            //            RoleId = roleDb.Id,
            //            ActivityFormId = activityForm.Id
            //        });
            //    
            //}


            SaveChanges();
            #endregion

            #region Place/Group Registration Activity

            #region save place/group registration activities
            var placeGroupRegistrationActivity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = "Place/Group Registration",
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                EndDate = new DateTime(2099, 12, 31),
                RepeatationCount = 0,
                RepeatationType = 1,
                ScheduleType = 1,//model.ScheduleType,
                StartDate = DateTime.UtcNow,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Default,
                //DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                //RepeatationOffset = model.RepeatationOffset,
            };
            _dbContext.Activities.Add(placeGroupRegistrationActivity);
            #endregion

            #region person pegistration activity entity type            
            var placeGroupRegistrationEntityTypes = (from c in _dbContext.EntityTypes
                                                     where (
                                                     //c.Name == "Person"
                                                     //|| c.Name == "Participant"
                                                     c.Name == "Place/Group"
                                                     //|| c.Name == "Project"
                                                     )
                                                     select c.Id).ToList();

            foreach (var entityType in placeGroupRegistrationEntityTypes)
            {
                this._dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = placeGroupRegistrationActivity.Id,
                    EntityTypeId = entityType,
                });
            }
            #endregion

            #region activity roles            
            foreach (var activityRole in activityRoles)
            {
                this._dbContext.ActivityRoles.Add(new ActivityRole()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = placeGroupRegistrationActivity.Id,
                    RoleId = activityRole.Id
                });
            }
            #endregion

            #region activity forms
            var placegroupActivityForms = _dbContext.Forms.Where(x => x.ProjectId == projectId && x.FormTitle == "Place/Group Registration");
            foreach (var form in placegroupActivityForms)
            {
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    ActivityId = placeGroupRegistrationActivity.Id
                };
                _dbContext.ActivityForms.Add(activityForm);
            }
            #endregion

            //foreach (var formRole in form.Roles)
            //{
            //        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole);
            //        dbContext.ActivityFormRoles.Add(new ActivityFormRole()
            //        {
            //            Guid = Guid.NewGuid(),
            //            RoleId = roleDb.Id,
            //            ActivityFormId = activityForm.Id
            //        });
            //    
            //}


            SaveChanges();
            #endregion

            #region Project Registration Activity

            #region save project registration activities
            var projectRegistrationActivity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = "Project Registration",
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                EndDate = new DateTime(2099, 12, 31),
                RepeatationCount = 0,
                RepeatationType = 1,
                ScheduleType = 1,//model.ScheduleType,
                StartDate = DateTime.UtcNow,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Default,
                //DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                //RepeatationOffset = model.RepeatationOffset,
            };
            _dbContext.Activities.Add(projectRegistrationActivity);
            #endregion

            #region person pegistration activity entity type            
            var entityTypes = (from c in _dbContext.EntityTypes
                               where (
                               //c.Name == "Person"
                               //|| c.Name == "Participant"
                               //|| c.Name == "Place/Group"
                               c.Name == "Project"
                               )
                               select c.Id).ToList();

            foreach (var entityType in entityTypes)
            {
                this._dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = projectRegistrationActivity.Id,
                    EntityTypeId = entityType,
                });
            }
            #endregion

            #region activity roles            
            foreach (var activityRole in activityRoles)
            {
                this._dbContext.ActivityRoles.Add(new ActivityRole()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = projectRegistrationActivity.Id,
                    RoleId = activityRole.Id
                });
            }
            #endregion

            #region activity forms
            var projectRegistrationActivityForms = _dbContext.Forms.Where(x => x.ProjectId == projectId && x.FormTitle == "Project Registration");
            foreach (var form in projectRegistrationActivityForms)
            {
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    ActivityId = projectRegistrationActivity.Id
                };
                _dbContext.ActivityForms.Add(activityForm);
            }
            #endregion

            //foreach (var formRole in form.Roles)
            //{
            //        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole);
            //        dbContext.ActivityFormRoles.Add(new ActivityFormRole()
            //        {
            //            Guid = Guid.NewGuid(),
            //            RoleId = roleDb.Id,
            //            ActivityFormId = activityForm.Id
            //        });
            //    
            //}


            SaveChanges();
            #endregion

            return true;
        }


        public bool CreateDefaultActivitiesForProject_Old(int projectId, int createdBy, int tenantId)
        {
            #region save activities

            var activityCategory = this._dbContext.ActivityCategories.FirstOrDefault(et => et.CategoryName == Core.Enum.ActivityCategories.Default.ToString());
            var acitvityStatus = this._dbContext.ActivityStatus.FirstOrDefault(et => et.Status == "Draft");

            var activity = new Activity()
            {
                Guid = Guid.NewGuid(),
                ActivityName = "Default Activity",
                ActivityCategoryId = activityCategory != null ? activityCategory.Id : (int?)null,
                ActivityStatusId = acitvityStatus.Id,
                EndDate = new DateTime(2099, 12, 31),
                RepeatationCount = 0,
                RepeatationType = 1,
                ScheduleType = 1,//model.ScheduleType,
                StartDate = DateTime.UtcNow,
                ProjectId = projectId,
                TenantId = tenantId,
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow,

                IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Default,
                //DependentActivityId = dependentActivity != null ? dependentActivity.Id : (int?)null,
                //RepeatationOffset = model.RepeatationOffset,
            };

            _dbContext.Activities.Add(activity);

            var entityTypes = (from c in _dbContext.EntityTypes
                               where (c.Name == "Person"
                               || c.Name == "Participant"
                               || c.Name == "Place/Group"
                               || c.Name == "Project")
                               select c.Id).ToList();

            foreach (var entityType in entityTypes)
            {
                this._dbContext.ActivityEntityTypes.Add(new ActivityEntityType()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = activity.Id,
                    EntityTypeId = entityType,
                });
            }

            var activityRoles = _dbContext.Roles.ToList();
            foreach (var activityRole in activityRoles)
            {
                this._dbContext.ActivityRoles.Add(new ActivityRole()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = activity.Id,
                    RoleId = activityRole.Id
                });
            }

            var activityForms = _dbContext.Forms.Where(x => x.ProjectId == projectId);

            foreach (var form in activityForms)
            {
                var activityForm = new ActivityForm()
                {
                    Guid = Guid.NewGuid(),
                    FormId = form.Id,
                    ActivityId = activity.Id
                };

                _dbContext.ActivityForms.Add(activityForm);


            }
            SaveChanges();

            //foreach (var formRole in form.Roles)
            //{
            //        var roleDb = roles.FirstOrDefault(v => v.Guid == formRole);
            //        dbContext.ActivityFormRoles.Add(new ActivityFormRole()
            //        {
            //            Guid = Guid.NewGuid(),
            //            RoleId = roleDb.Id,
            //            ActivityFormId = activityForm.Id
            //        });
            //    
            //}
            //SaveChanges();

            #endregion

            return true;
        }



        public void AllCountriesList()
        {
            var countries = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("AD", "ANDORRA"),
                new KeyValuePair<string, string>("AE", "UNITED ARAB EMIRATES"),
                new KeyValuePair<string, string>("AF", "AFGHANISTAN"),
                new KeyValuePair<string, string>("AG", "ANTIGUA AND BARBUDA"),
                new KeyValuePair<string, string>("AI", "ANGUILLA"),
                new KeyValuePair<string, string>("AL", "ALBANIA"),
                new KeyValuePair<string, string>("AM", "ARMENIA"),
                new KeyValuePair<string, string>("AO", "ANGOLA"),
                new KeyValuePair<string, string>("AQ", "ANTARCTICA"),
                new KeyValuePair<string, string>("AR", "ARGENTINA"),
                new KeyValuePair<string, string>("AS", "AMERICAN SAMOA"),
                new KeyValuePair<string, string>("AT", "AUSTRIA"),
                new KeyValuePair<string, string>("AU", "AUSTRALIA"),
                new KeyValuePair<string, string>("AW", "ARUBA"),
                new KeyValuePair<string, string>("AX", "ALAND ISLANDS"),
                new KeyValuePair<string, string>("AZ", "AZERBAIJAN"),
                new KeyValuePair<string, string>("BA", "BOSNIA AND HERZEGOVINA"),
                new KeyValuePair<string, string>("BB", "BARBADOS"),
                new KeyValuePair<string, string>("BD", "BANGLADESH"),
                new KeyValuePair<string, string>("BE", "BELGIUM"),
                new KeyValuePair<string, string>("BF", "BURKINA FASO"),
                new KeyValuePair<string, string>("BG", "BULGARIA"),
                new KeyValuePair<string, string>("BH", "BAHRAIN"),
                new KeyValuePair<string, string>("BI", "BURUNDI"),
                new KeyValuePair<string, string>("BJ", "BENIN"),
                new KeyValuePair<string, string>("BL", "SAINT BARTHELEMY"),
                new KeyValuePair<string, string>("BM", "BERMUDA"),
                new KeyValuePair<string, string>("BN", "BRUNEI DARUSSALAM"),
                new KeyValuePair<string, string>("BO", "BOLIVIA, PLURINATIONAL STATE OF"),
                new KeyValuePair<string, string>("BQ", "BONAIRE, SINT EUSTATIUS AND SABA"),
                new KeyValuePair<string, string>("BR", "BRAZIL"),
                new KeyValuePair<string, string>("BS", "BAHAMAS"),
                new KeyValuePair<string, string>("BT", "BHUTAN"),
                new KeyValuePair<string, string>("BW", "BOTSWANA"),
                new KeyValuePair<string, string>("BY", "BELARUS"),
                new KeyValuePair<string, string>("BZ", "BELIZE"),
                new KeyValuePair<string, string>("CA", "CANADA"),
                new KeyValuePair<string, string>("CC", "COCOS (KEELING) ISLANDS"),
                new KeyValuePair<string, string>("CD", "CONGO, THE DEMOCRATIC REPUBLIC OF THE"),
                new KeyValuePair<string, string>("CF", "CENTRAL AFRICAN REPUBLIC"),
                new KeyValuePair<string, string>("CG", "CONGO"),
                new KeyValuePair<string, string>("CH", "SWITZERLAND"),
                new KeyValuePair<string, string>("CI", "COTE DIVOIRE"),
                new KeyValuePair<string, string>("CK", "COOK ISLANDS"),
                new KeyValuePair<string, string>("CL", "CHILE"),
                new KeyValuePair<string, string>("CM", "CAMEROON"),
                new KeyValuePair<string, string>("CN", "CHINA"),
                new KeyValuePair<string, string>("CO", "COLOMBIA"),
                new KeyValuePair<string, string>("CR", "COSTA RICA"),
                new KeyValuePair<string, string>("CU", "CUBA"),
                new KeyValuePair<string, string>("CV", "CAPE VERDE"),
                new KeyValuePair<string, string>("CW", "CURACAO"),
                new KeyValuePair<string, string>("CX", "CHRISTMAS ISLAND"),
                new KeyValuePair<string, string>("CY", "CYPRUS"),
                new KeyValuePair<string, string>("CZ", "CZECH REPUBLIC"),
                new KeyValuePair<string, string>("DE", "GERMANY"),
                new KeyValuePair<string, string>("DJ", "DJIBOUTI"),
                new KeyValuePair<string, string>("DK", "DENMARK"),
                new KeyValuePair<string, string>("DM", "DOMINICA"),
                new KeyValuePair<string, string>("DO", "DOMINICAN REPUBLIC"),
                new KeyValuePair<string, string>("DZ", "ALGERIA"),
                new KeyValuePair<string, string>("EC", "ECUADOR"),
                new KeyValuePair<string, string>("EE", "ESTONIA"),
                new KeyValuePair<string, string>("EG", "EGYPT"),
                new KeyValuePair<string, string>("EH", "WESTERN SAHARA"),
                new KeyValuePair<string, string>("ER", "ERITREA"),
                new KeyValuePair<string, string>("ES", "SPAIN"),
                new KeyValuePair<string, string>("ET", "ETHIOPIA"),
                new KeyValuePair<string, string>("FI", "FINLAND"),
                new KeyValuePair<string, string>("FJ", "FIJI"),
                new KeyValuePair<string, string>("FK", "FALKLAND ISLANDS (MALVINAS)"),
                new KeyValuePair<string, string>("FM", "MICRONESIA, FEDERATED STATES OF"),
                new KeyValuePair<string, string>("FO", "FAROE ISLANDS"),
                new KeyValuePair<string, string>("FR", "FRANCE"),
                new KeyValuePair<string, string>("GA", "GABON"),
                new KeyValuePair<string, string>("GB", "UNITED KINGDOM"),
                new KeyValuePair<string, string>("GD", "GRENADA"),
                new KeyValuePair<string, string>("GE", "GEORGIA"),
                new KeyValuePair<string, string>("GF", "FRENCH GUIANA"),
                new KeyValuePair<string, string>("GG", "GUERNSEY"),
                new KeyValuePair<string, string>("GH", "GHANA"),
                new KeyValuePair<string, string>("GI", "GIBRALTAR"),
                new KeyValuePair<string, string>("GL", "GREENLAND"),
                new KeyValuePair<string, string>("GM", "GAMBIA"),
                new KeyValuePair<string, string>("GN", "GUINEA"),
                new KeyValuePair<string, string>("GP", "GUADELOUPE"),
                new KeyValuePair<string, string>("GQ", "EQUATORIAL GUINEA"),
                new KeyValuePair<string, string>("GR", "GREECE"),
                new KeyValuePair<string, string>("GS", "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS"),
                new KeyValuePair<string, string>("GT", "GUATEMALA"),
                new KeyValuePair<string, string>("GU", "GUAM"),
                new KeyValuePair<string, string>("GW", "GUINEA-BISSAU"),
                new KeyValuePair<string, string>("GY", "GUYANA"),
                new KeyValuePair<string, string>("HK", "HONG KONG"),
                new KeyValuePair<string, string>("HM", "HEARD ISLAND AND MCDONALD ISLANDS"),
                new KeyValuePair<string, string>("HN", "HONDURAS"),
                new KeyValuePair<string, string>("HR", "CROATIA"),
                new KeyValuePair<string, string>("HT", "HAITI"),
                new KeyValuePair<string, string>("HU", "HUNGARY"),
                new KeyValuePair<string, string>("ID", "INDONESIA"),
                new KeyValuePair<string, string>("IE", "IRELAND"),
                new KeyValuePair<string, string>("IL", "ISRAEL"),
                new KeyValuePair<string, string>("IM", "ISLE OF MAN"),
                new KeyValuePair<string, string>("IN", "INDIA"),
                new KeyValuePair<string, string>("IQ", "BRITISH INDIAN OCEAN TERRITORY"),
                new KeyValuePair<string, string>("IQ", "IRAQ"),
                new KeyValuePair<string, string>("IR", "IRAN, ISLAMIC REPUBLIC OF"),
                new KeyValuePair<string, string>("IS", "ICELAND"),
                new KeyValuePair<string, string>("IT", "ITALY"),
                new KeyValuePair<string, string>("JE", "JERSEY"),
                new KeyValuePair<string, string>("JM", "JAMAICA"),
                new KeyValuePair<string, string>("JO", "JORDAN"),
                new KeyValuePair<string, string>("JP", "JAPAN"),
                new KeyValuePair<string, string>("KE", "KENYA"),
                new KeyValuePair<string, string>("KG", "KYRGYZSTAN"),
                new KeyValuePair<string, string>("KH", "CAMBODIA"),
                new KeyValuePair<string, string>("KI", "KIRIBATI"),
                new KeyValuePair<string, string>("KM", "COMOROS"),
                new KeyValuePair<string, string>("KN", "SAINT KITTS AND NEVIS"),
                new KeyValuePair<string, string>("KP", "KOREA, DEMOCRATIC PEOPLE'S REPUBLIC OF"),
                new KeyValuePair<string, string>("KR", "KOREA, REPUBLIC OF"),
                new KeyValuePair<string, string>("KW", "KUWAIT"),
                new KeyValuePair<string, string>("KY", "CAYMAN ISLANDS"),
                new KeyValuePair<string, string>("KZ", "KAZAKHSTAN"),
                new KeyValuePair<string, string>("LA", "LAO PEOPLE'S DEMOCRATIC REPUBLIC"),
                new KeyValuePair<string, string>("LB", "LEBANON"),
                new KeyValuePair<string, string>("LC", "SAINT LUCIA"),
                new KeyValuePair<string, string>("LI", "LIECHTENSTEIN"),
                new KeyValuePair<string, string>("LK", "SRI LANKA"),
                new KeyValuePair<string, string>("LR", "LIBERIA"),
                new KeyValuePair<string, string>("LS", "LESOTHO"),
                new KeyValuePair<string, string>("LT", "LITHUANIA"),
                new KeyValuePair<string, string>("LU", "LUXEMBOURG"),
                new KeyValuePair<string, string>("LV", "LATVIA"),
                new KeyValuePair<string, string>("LY", "LIBYA"),
                new KeyValuePair<string, string>("MA", "MOROCCO"),
                new KeyValuePair<string, string>("MC", "MONACO"),
                new KeyValuePair<string, string>("MD", "MOLDOVA, REPUBLIC OF"),
                new KeyValuePair<string, string>("ME", "MONTENEGRO"),
                new KeyValuePair<string, string>("MF", "SAINT MARTIN (FRENCH PART)"),
                new KeyValuePair<string, string>("MG", "MADAGASCAR"),
                new KeyValuePair<string, string>("MH", "MARSHALL ISLANDS"),
                new KeyValuePair<string, string>("MK", "MACEDONIA, THE FORMER YUGOSLAV REPUBLIC OF"),
                new KeyValuePair<string, string>("ML", "MALI"),
                new KeyValuePair<string, string>("MM", "MYANMAR"),
                new KeyValuePair<string, string>("MN", "MONGOLIA"),
                new KeyValuePair<string, string>("MO", "MACAO"),
                new KeyValuePair<string, string>("MP", "NORTHERN MARIANA ISLANDS"),
                new KeyValuePair<string, string>("MQ", "MARTINIQUE"),
                new KeyValuePair<string, string>("MR", "MAURITANIA"),
                new KeyValuePair<string, string>("MS", "MONTSERRAT"),
                new KeyValuePair<string, string>("MT", "MALTA"),
                new KeyValuePair<string, string>("MU", "MAURITIUS"),
                new KeyValuePair<string, string>("MV", "MALDIVES"),
                new KeyValuePair<string, string>("MW", "MALAWI"),
                new KeyValuePair<string, string>("MX", "MEXICO"),
                new KeyValuePair<string, string>("MY", "MALAYSIA"),
                new KeyValuePair<string, string>("MZ", "MOZAMBIQUE"),
                new KeyValuePair<string, string>("NA", "NAMIBIA"),
                new KeyValuePair<string, string>("NC", "NEW CALEDONIA"),
                new KeyValuePair<string, string>("NE", "NIGER"),
                new KeyValuePair<string, string>("NF", "NORFOLK ISLAND"),
                new KeyValuePair<string, string>("NG", "NIGERIA"),
                new KeyValuePair<string, string>("NI", "NICARAGUA"),
                new KeyValuePair<string, string>("NL", "NETHERLANDS"),
                new KeyValuePair<string, string>("NO", "NORWAY"),
                new KeyValuePair<string, string>("NP", "NEPAL"),
                new KeyValuePair<string, string>("NR", "NAURU"),
                new KeyValuePair<string, string>("NU", "NIUE"),
                new KeyValuePair<string, string>("NZ", "NEW ZEALAND"),
                new KeyValuePair<string, string>("OM", "OMAN"),
                new KeyValuePair<string, string>("PA", "PANAMA"),
                new KeyValuePair<string, string>("PE", "PERU"),
                new KeyValuePair<string, string>("PF", "FRENCH POLYNESIA"),
                new KeyValuePair<string, string>("PG", "PAPUA NEW GUINEA"),
                new KeyValuePair<string, string>("PH", "PHILIPPINES"),
                new KeyValuePair<string, string>("PK", "PAKISTAN"),
                new KeyValuePair<string, string>("PL", "POLAND"),
                new KeyValuePair<string, string>("PM", "SAINT PIERRE AND MIQUELON"),
                new KeyValuePair<string, string>("PN", "PITCAIRN"),
                new KeyValuePair<string, string>("PR", "PUERTO RICO"),
                new KeyValuePair<string, string>("PS", "PALESTINE, STATE OF"),
                new KeyValuePair<string, string>("PT", "PORTUGAL"),
                new KeyValuePair<string, string>("PW", "PALAU"),
                new KeyValuePair<string, string>("PY", "PARAGUAY"),
                new KeyValuePair<string, string>("QA", "QATAR"),
                new KeyValuePair<string, string>("RE", "REUNION"),
                new KeyValuePair<string, string>("RO", "ROMANIA"),
                new KeyValuePair<string, string>("RS", "SERBIA"),
                new KeyValuePair<string, string>("RU", "RUSSIAN FEDERATION"),
                new KeyValuePair<string, string>("RW", "RWANDA"),
                new KeyValuePair<string, string>("SA", "SAUDI ARABIA"),
                new KeyValuePair<string, string>("SB", "SOLOMON ISLANDS"),
                new KeyValuePair<string, string>("SC", "SEYCHELLES"),
                new KeyValuePair<string, string>("SD", "SUDAN"),
                new KeyValuePair<string, string>("SE", "SWEDEN"),
                new KeyValuePair<string, string>("SG", "SINGAPORE"),
                new KeyValuePair<string, string>("SH", "SAINT HELENA, ASCENSION AND TRISTAN DA CUNHA"),
                new KeyValuePair<string, string>("SI", "SLOVENIA"),
                new KeyValuePair<string, string>("SJ", "SVALBARD AND JAN MAYEN"),
                new KeyValuePair<string, string>("SK", "SLOVAKIA"),
                new KeyValuePair<string, string>("SL", "SIERRA LEONE"),
                new KeyValuePair<string, string>("SM", "SAN MARINO"),
                new KeyValuePair<string, string>("SN", "SENEGAL"),
                new KeyValuePair<string, string>("SO", "SOMALIA"),
                new KeyValuePair<string, string>("SR", "SURINAME"),
                new KeyValuePair<string, string>("SS", "SOUTH SUDAN"),
                new KeyValuePair<string, string>("ST", "SAO TOME AND PRINCIPE"),
                new KeyValuePair<string, string>("SV", "EL SALVADOR"),
                new KeyValuePair<string, string>("SX", "SINT MAARTEN (DUTCH PART)"),
                new KeyValuePair<string, string>("SY", "SYRIAN ARAB REPUBLIC"),
                new KeyValuePair<string, string>("SZ", "SWAZILAND"),
                new KeyValuePair<string, string>("TC", "TURKS AND CAICOS ISLANDS"),
                new KeyValuePair<string, string>("TD", "CHAD"),
                new KeyValuePair<string, string>("TF", "FRENCH SOUTHERN TERRITORIES"),
                new KeyValuePair<string, string>("TG", "TOGO"),
                new KeyValuePair<string, string>("TH", "THAILAND"),
                new KeyValuePair<string, string>("TJ", "TAJIKISTAN"),
                new KeyValuePair<string, string>("TK", "TOKELAU"),
                new KeyValuePair<string, string>("TL", "TIMOR-LESTE"),
                new KeyValuePair<string, string>("TM", "TURKMENISTAN"),
                new KeyValuePair<string, string>("TN", "TUNISIA"),
                new KeyValuePair<string, string>("TO", "TONGA"),
                new KeyValuePair<string, string>("TR", "TURKEY"),
                new KeyValuePair<string, string>("TT", "TRINIDAD AND TOBAGO"),
                new KeyValuePair<string, string>("TV", "TUVALU"),
                new KeyValuePair<string, string>("TW", "TAIWAN, PROVINCE OF CHINA"),
                new KeyValuePair<string, string>("TZ", "TANZANIA, UNITED REPUBLIC OF"),
                new KeyValuePair<string, string>("UA", "UKRAINE"),
                new KeyValuePair<string, string>("UG", "UGANDA"),
                new KeyValuePair<string, string>("UM", "UNITED STATES MINOR OUTLYING ISLANDS"),
                new KeyValuePair<string, string>("US", "UNITED STATES"),
                new KeyValuePair<string, string>("UY", "URUGUAY"),
                new KeyValuePair<string, string>("UZ", "UZBEKISTAN"),
                new KeyValuePair<string, string>("VA", "HOLY SEE (VATICAN CITY STATE)"),
                new KeyValuePair<string, string>("VC", "SAINT VINCENT AND THE GRENADINES"),
                new KeyValuePair<string, string>("VE", "VENEZUELA"),
                new KeyValuePair<string, string>("VG", "VIRGIN ISLANDS, BRITISH"),
                new KeyValuePair<string, string>("VI", "VIRGIN ISLANDS, U.S."),
                new KeyValuePair<string, string>("VN", "VIET NAM"),
                new KeyValuePair<string, string>("VU", "VANUATU"),
                new KeyValuePair<string, string>("WF", "WALLIS AND FUTUNA"),
                new KeyValuePair<string, string>("WS", "SAMOA"),
                new KeyValuePair<string, string>("XZ", "INSTALLATIONS IN INTERNATIONAL WATERS"),
                new KeyValuePair<string, string>("YE", "YEMEN"),
                new KeyValuePair<string, string>("YT", "MAYOTTE"),
                new KeyValuePair<string, string>("ZA", "SOUTH AFRICA"),
                new KeyValuePair<string, string>("ZM", "ZAMBIA"),
                new KeyValuePair<string, string>("ZW", "ZIMBABWE"),
            };




            foreach (var country in countries)
            {
                this._dbContext.Countries.Add(new Data.Country
                {
                    Guid = Guid.NewGuid(),
                    Abbr = country.Key.Trim(),
                    Name = country.Value.Trim(),
                });
            }
            SaveChanges();

            //return countries;
        }

    }
}
