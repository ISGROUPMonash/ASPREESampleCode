using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using System.Data.Entity;

namespace Aspree.Provider.Provider
{
    public class EmailTemplateProvider : IEmailTemplateProvider
    {

        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IPushEmailEventProvider _pushEmailEventProvider;
        private readonly AspreeEntities _dbContext;


        public EmailTemplateProvider(AspreeEntities dbContext, IUserLoginProvider userLoginProvider, IPushEmailEventProvider pushEmailEventProvider)//, IPushEmailEventProvider pushEmailEventProvider
        {
            this._dbContext = dbContext;
            this._userLoginProvider = userLoginProvider;
            this._pushEmailEventProvider = pushEmailEventProvider;
        }

        public EmailTemplateViewModel Create(EmailTemplateViewModel model)
        {
            throw new NotImplementedException();
        }

        public EmailTemplateViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public EmailTemplateViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmailTemplateViewModel> GetAll()
        {
            return _dbContext.EmailTemplates
                 .Select(ToModel)
                 .ToList();
        }


        public EmailTemplateViewModel GetByGuid(Guid guid)
        {
            var Email = _dbContext.EmailTemplates
             .FirstOrDefault(fs => fs.Guid == guid);

            if (Email != null)
                return ToEditModel(Email);

            return null;
        }

        public EmailTemplateViewModel GetById(int id)
        {
            var Email = _dbContext.EmailTemplates
                .FirstOrDefault(fs => fs.Id == id);

            if (Email != null)
                return ToModel(Email);

            return null;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }


        public EmailTemplateViewModel ToModel(EmailTemplate entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            List<PushEmailEventViewModel> eventTypes = new List<PushEmailEventViewModel>();
            return new EmailTemplateViewModel
            {
                Id = entity.Id,
                PushEmailEventID = entity.PushEmailEvent.Guid,
                MailBody = entity.MailBody,
                EMailKeywords = entity.EmailKeywords,
                IsActive = entity.IsActive.Value,
                Subject = entity.Subject,
                CreatedBy = createdBy.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                Guid = entity.Guid,
                EventName = entity.PushEmailEvent.DisplayName,
            };
        }

        public EmailTemplateViewModel ToEditModel(EmailTemplate entity)
        {
            var createdBy = _userLoginProvider.GetById(entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? _userLoginProvider.GetById(entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? _userLoginProvider.GetById(entity.DeactivatedBy.Value) : null;
            List<PushEmailEventViewModel> eventTypes = new List<PushEmailEventViewModel>();
            return new EmailTemplateViewModel
            {
                Id = entity.Id,
                PushEmailEventID = entity.PushEmailEvent.Guid,
                MailBody = entity.MailBody,
                EMailKeywords = entity.EmailKeywords,
                IsActive = entity.IsActive.Value,
                Subject = entity.Subject,
                CreatedBy = createdBy.Guid,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                CreatedDate = entity.CreatedDate,
                ModifiedDate = entity.ModifiedDate,
                Guid = entity.Guid,
                EventName = entity.PushEmailEvent.DisplayName,
                EventList = _pushEmailEventProvider.GetAll().ToList()
            };
        }

        public EmailTemplateViewModel Update(EmailTemplateViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.ModifiedBy.Value);

            var EditEmailTemplate = _dbContext.EmailTemplates
               .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (EditEmailTemplate != null)
            {
                EditEmailTemplate.MailBody = model.MailBody;
                EditEmailTemplate.ModifiedBy = modifiedBy.Id;
                EditEmailTemplate.ModifiedDate = DateTime.UtcNow;
                EditEmailTemplate.Subject = model.Subject;
                SaveChanges();
                return ToModel(EditEmailTemplate);
            }
            return null;
        }
    }
}
