using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Data;
using System.Linq.Expressions;
using Aspree.Core.ViewModels;

namespace Aspree.Provider.Provider
{
    public class ProjectStatusProvider : IProjectStatusProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;
        public ProjectStatusProvider(AspreeEntities _dbContext,IUserLoginProvider userLoginProvider)
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
        }

        public ProjectStatusViewModel Create(ProjectStatusViewModel model)
        {
            if (dbContext.ProjectStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()))
            {
                throw new Core.AlreadyExistsException("Project status already exists.");
            }

            var projectStatus = new ProjectStatu
            {
                Status = model.Status,
                Guid = Guid.NewGuid(),
                IsActive = true
            };

            dbContext.ProjectStatus.Add(projectStatus);

            SaveChanges();

            return ToModel(projectStatus);
        }

        public ProjectStatusViewModel DeleteById(int id, Guid DeletedBy)
        {
            var projectStatus = dbContext.ProjectStatus
                .FirstOrDefault(fs => fs.Id == id);

            if (projectStatus != null)
            {
                dbContext.ProjectStatus.Remove(projectStatus);
                return ToModel(projectStatus);
            }

            return null;
        }

        public ProjectStatusViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var projectStatus = dbContext.ProjectStatus
                .FirstOrDefault(fs => fs.Guid == guid);

            if (projectStatus != null)
            {
                dbContext.ProjectStatus.Remove(projectStatus);
                return ToModel(projectStatus);
            }

            return null;
        }

        public IEnumerable<ProjectStatusViewModel> GetAll()
        {
            return dbContext.ProjectStatus
                .Select(ToModel)
                .ToList();
        }

        public ProjectStatusViewModel GetByGuid(Guid guid)
        {
            var projectStatus = dbContext.ProjectStatus
                .FirstOrDefault(fs => fs.Guid == guid);

            if (projectStatus != null)
                return ToModel(projectStatus);

            return null;
        }

        public ProjectStatusViewModel GetById(int id)
        {
            var projectStatus = dbContext.ProjectStatus
                .FirstOrDefault(fs => fs.Id == id);

            if (projectStatus != null)
                return ToModel(projectStatus);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public ProjectStatusViewModel ToModel(ProjectStatu entity)
        {
            return new ProjectStatusViewModel
            {
                Guid = entity.Guid,
                Id = entity.Id,
                IsActive = entity.IsActive,
                Status = entity.Status
            };
        }

        public ProjectStatusViewModel Update(ProjectStatusViewModel model)
        {
            if (dbContext.ProjectStatus.Any(est => est.Status.ToLower() == model.Status.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Project status already exists.");
            }

            var projectStatus = dbContext.ProjectStatus
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (projectStatus != null)
            {
                projectStatus.Status = model.Status;
                SaveChanges();
                return ToModel(projectStatus);
            }

            return null;
        }
    }
}
