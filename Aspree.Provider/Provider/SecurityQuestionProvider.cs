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
    public class SecurityQuestionProvider : ISecurityQuestionProvider
    {
        private readonly AspreeEntities dbContext;
        public SecurityQuestionProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public SecurityQuestionViewModel Create(SecurityQuestionViewModel model)
        {
            if (dbContext.SecurityQuestions.Any(est => est.Question.ToLower() == model.Question.ToLower()))
            {
                throw new Core.AlreadyExistsException("Security question already exists.");
            }

            var securityQuestion = new SecurityQuestion
            {
                Question = model.Question,
                Guid = Guid.NewGuid()
            };

            dbContext.SecurityQuestions.Add(securityQuestion);

            SaveChanges();

            return ToModel(securityQuestion);
        }

        public SecurityQuestionViewModel DeleteById(int id, Guid DeletedBy)
        {
            var securityQuestion = dbContext.SecurityQuestions.FirstOrDefault(fs => fs.Id == id);
            if (securityQuestion != null)
            {
                var question = ToModel(securityQuestion);

                dbContext.SecurityQuestions.Remove(securityQuestion);
                return question;
            }

            return null;
        }

        public SecurityQuestionViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var securityQuestion = dbContext.SecurityQuestions.FirstOrDefault(fs => fs.Guid == guid);
            if (securityQuestion != null)
            {
                var question = ToModel(securityQuestion);

                dbContext.SecurityQuestions.Remove(securityQuestion);
                return question;
            }

            return null;
        }

        public IEnumerable<SecurityQuestionViewModel> GetAll()
        {
            return dbContext.SecurityQuestions.Select(ToModel).ToList();
        }

        public SecurityQuestionViewModel GetByGuid(Guid guid)
        {
            var securityQuestion = dbContext.SecurityQuestions
                .FirstOrDefault(fs => fs.Guid == guid);

            if (securityQuestion != null)
                return ToModel(securityQuestion);

            return null;
        }

        public SecurityQuestionViewModel GetById(int id)
        {
            var securityQuestion = dbContext.SecurityQuestions
                .FirstOrDefault(fs => fs.Id == id);

            if (securityQuestion != null)
                return ToModel(securityQuestion);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public SecurityQuestionViewModel ToModel(SecurityQuestion entity)
        {
            return new SecurityQuestionViewModel
            {
                Guid = entity.Guid,
                Id = entity.Id,
                Question = entity.Question
            };
        }

        public SecurityQuestionViewModel Update(SecurityQuestionViewModel model)
        {
            if (dbContext.SecurityQuestions.Any(est => est.Question.ToLower() == model.Question.ToLower()
                && est.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Security question already exists.");
            }

            var securityQuestion = dbContext.SecurityQuestions
                .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (securityQuestion != null)
            {
                securityQuestion.Question = model.Question;

                SaveChanges();

                return ToModel(securityQuestion);
            }

            return null;
        }
    }
}
