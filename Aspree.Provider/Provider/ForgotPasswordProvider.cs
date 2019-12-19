using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class ForgotPasswordProvider : IForgotPasswordProvider
    {

        private readonly AspreeEntities dbContext;
        public ForgotPasswordProvider(AspreeEntities _dbContext)
        {
            this.dbContext = _dbContext;
        }
        public Aspree.Data.UserLogin checkUser(ResetPasswordViewModel resetPasswordViewModel, bool isTestSite = false)
        {
            UserLogin user = null;
            if (isTestSite)
            {
                user = dbContext.UserLogins
                .FirstOrDefault(x => x.UserName.ToLower() == resetPasswordViewModel.ToEMail.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test);
            }
            else
            {
                user = dbContext.UserLogins
                .FirstOrDefault(x => x.UserName.ToLower() == resetPasswordViewModel.ToEMail.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test);
            }
            if (user == null)
            {
                //throw new Core.NotFoundException("User was not found");
            }
            if (user != null && !user.IsUserApprovedBySystemAdmin)
            {
                return user;
            }
            if (user != null && user.Status != (int)Core.Enum.Status.Active)
            {
                return user;
            }

            if (user != null && user.SecurityQuestionId.HasValue && !string.IsNullOrEmpty(user.Answer))
            {
                user.ModifiedDate = DateTime.UtcNow;
                user.TempGuid = Guid.NewGuid();
                SaveChanges();
            }

            return user;
        }

        public bool CheckSecurityQuestionAnswer(CheckSecurityQuestionAnswerViewModels model)
        {
            var user = dbContext.UserLogins
                .FirstOrDefault(x => x.Guid == model.UserId && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password);

            if (user == null)
            {
                throw new Core.NotFoundException("User was not found");
            }
            if (!user.SecurityQuestionId.HasValue || string.IsNullOrEmpty(user.Answer))
            {
                throw new Core.BadRequestException("User is not active");
            }
            if (user.SecurityQuestion.Guid != model.QuestionGuid)
            {
                throw new Core.BadRequestException("Incorrect security question", "QuestionGuid");
            }
            if (user.Answer != model.Answer)
            {
                throw new Core.BadRequestException("Incorrect security answer", "Answer");
            }
            return true;
        }

        public string EmailTemplate()
        {
            return System.IO.File.ReadAllText($"{System.AppDomain.CurrentDomain.BaseDirectory}/Resources/email-template.html");
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }
    }
}
