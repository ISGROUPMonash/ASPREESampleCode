using Aspree.Provider.Provider;
using Aspree.WebApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Aspree.WebApi.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailService
    {
        Data.AspreeEntities dbContext;
        /// <summary>
        /// 
        /// </summary>
        public EmailService()
        {
            dbContext = new Data.AspreeEntities();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SendEmail(string to, string subject, string message)
        {
            try
            {
                var mail = new MailMessage();

                mail.From = new MailAddress(ConfigSettings.EmailFrom, "Aspree");
                mail.To.Add(new MailAddress(to));
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(ConfigSettings.SmtpHost, ConfigSettings.Port);
                smtp.UseDefaultCredentials = true;
                smtp.Send(mail);

                dbContext.Emails.Add(new Data.Email()
                {
                    Body = message,
                    CreatedDate = DateTime.UtcNow,
                    From = ConfigSettings.EmailFrom,
                    Guid = Guid.NewGuid(),
                    IsImmediate = true,
                    IsSend = true,
                    SentDate = DateTime.UtcNow,
                    Subject = subject,
                    To = to,
                    SmtpServerId = 1,
                });

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    dbContext.Emails.Add(new Data.Email()
                    {
                        FailedAttempt = 1,
                        Body = "Exception: " + ex.Message + System.Environment.NewLine + "Mail Body: " + message,
                        CreatedDate = DateTime.UtcNow,
                        From = ConfigSettings.EmailFrom,
                        Guid = Guid.NewGuid(),
                        IsImmediate = true,
                        IsSend = false,
                        SentDate = DateTime.UtcNow,
                        Subject = subject,
                        To = to,
                        SmtpServerId = 1,
                    });
                    dbContext.SaveChanges();
                }
                catch (Exception exc) { }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="userName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool SendWelcomeEmail(string to, string userName, string url)
        {
            var template = dbContext.EmailTemplates.First(e => e.PushEmailEvent.EventName == Core.Enum.EmailTemplateTypes.Welcome.ToString());

            var body = template.MailBody;
            body = body.Replace("@UserName@", userName);
            body = body.Replace("@Url@", url);

            SendEmail(to, template.Subject, body);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="userName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool SendForgotPasswordEmail(string to, string userName, string url)
        {
            var template = dbContext.EmailTemplates.First(e => e.PushEmailEvent.EventName == Core.Enum.EmailTemplateTypes.Forgot_Password.ToString());

            var body = template.MailBody;
            body = body.Replace("@UserName@", userName);
            body = body.Replace("@Url@", url);

            SendEmail(to, template.Subject, body);
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string EmailTemplate()
        {
            return System.IO.File.ReadAllText($"{System.AppDomain.CurrentDomain.BaseDirectory}/Resources/email-template.html");
        }
    }
}