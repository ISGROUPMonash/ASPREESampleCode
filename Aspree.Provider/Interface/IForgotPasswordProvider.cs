using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IForgotPasswordProvider
    {
        Data.UserLogin checkUser(ResetPasswordViewModel resetPasswordViewModel, bool isTestSite = false);
        void SaveChanges();

        string EmailTemplate();

        bool CheckSecurityQuestionAnswer(CheckSecurityQuestionAnswerViewModels model);
    }
}
