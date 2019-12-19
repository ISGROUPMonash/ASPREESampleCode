using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Data;

namespace Aspree.Provider.Provider
{
    public class EmailProvider : IEmailProvider
    {
        public EmailViewModel Create(EmailViewModel model)
        {
            throw new NotImplementedException();
        }

        public EmailViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public EmailViewModel DeleteById(int id, Guid DeletedBy)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EmailViewModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public EmailViewModel GetByGuid(Guid guid)
        {
            throw new NotImplementedException();
        }

        public EmailViewModel GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public EmailViewModel ToModel(Email entity)
        {
            throw new NotImplementedException();
        }

        public EmailViewModel Update(EmailViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
