using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace SupplementaryAPI.Smtp
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : SMTP Handler</para>
    /// </summary>
    public class Smtp
    {
        MailMessage mail;
        string sHost;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="sTo"></param>
        public Smtp(string sHost, string sFrom, string sTo)
        {
            mail = new MailMessage(sFrom, sTo);
            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="sTo"></param>
        /// <param name="sCC"></param>
        public Smtp(string sHost, string sFrom, string sTo, string sCC)
        {
            mail = new MailMessage(sFrom, sTo);
            mail.CC.Add(sCC);

            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="lTo"></param>
        public Smtp(string sHost, string sFrom, List<string> lTo)
        {
            mail = new MailMessage();
            mail.From = new MailAddress(sFrom, sFrom);

            foreach (var sTo in lTo)
            {
                mail.To.Add(sTo);
            }

            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="sFromDisplayName"></param>
        /// <param name="lTo"></param>
        public Smtp(string sHost, string sFrom, string sFromDisplayName, List<string> lTo)
        {
            mail = new MailMessage();
            mail.From = new MailAddress(sFrom, sFromDisplayName);

            foreach (var sTo in lTo)
            {
                mail.To.Add(sTo);
            }

            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="lTo"></param>
        /// <param name="lCc"></param>
        public Smtp(string sHost, string sFrom, List<string> lTo, List<string> lCc)
        {
            mail = new MailMessage();
            mail.From = new MailAddress(sFrom, sFrom);

            foreach (var sTo in lTo)
            {
                mail.To.Add(sTo);
            }

            foreach (var sCc in lCc)
            {
                mail.CC.Add(sCc);
            }

            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost"></param>
        /// <param name="sFrom"></param>
        /// <param name="sFormDisplayName"></param>
        /// <param name="lTo"></param>
        /// <param name="lCc"></param>
        public Smtp(string sHost, string sFrom, string sFormDisplayName, List<string> lTo, List<string> lCc)
        {
            mail = new MailMessage();
            mail.From = new MailAddress(sFrom, sFormDisplayName);

            foreach (var sTo in lTo)
            {
                mail.To.Add(sTo);
            }

            foreach (var sCc in lCc)
            {
                mail.CC.Add(sCc);
            }

            this.sHost = sHost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priority"></param>
        public void SetPriority(MailPriority priority)
        {
            mail.Priority = priority;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSubject"></param>
        /// <param name="sBody"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public bool Send(string sSubject, string sBody, bool isHtml)
        {
            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = sHost;

                    MailMessage mail = this.mail;
                    mail.IsBodyHtml = isHtml;
                    mail.Subject = sSubject;
                    mail.Body = sBody;

                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSubject"></param>
        /// <param name="sBody"></param>
        /// <param name="isHtml"></param>
        /// <param name="pathAttached"></param>
        /// <returns></returns>
        public bool SendWithAttachment(string sSubject, string sBody, bool isHtml, string pathAttached)
        {
            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = sHost;

                    MailMessage mail = this.mail;
                    mail.IsBodyHtml = isHtml;
                    mail.Subject = sSubject;
                    mail.Body = sBody;

                    if (File.Exists(pathAttached))
                    {
                        mail.Attachments.Add(new Attachment(pathAttached));
                    }

                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sSubject"></param>
        /// <param name="sBody"></param>
        /// <param name="isHtml"></param>
        /// <param name="attachments"></param>
        /// <returns></returns>
        public bool SendWithAttachment(string sSubject, string sBody, bool isHtml, List<string> attachments)
        {
            using (SmtpClient client = new SmtpClient())
            {
                try
                {
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Host = sHost;

                    MailMessage mail = this.mail;
                    mail.IsBodyHtml = isHtml;
                    mail.Subject = sSubject;
                    mail.Body = sBody;

                    foreach (var att in attachments)
                    {
                        mail.Attachments.Add(new Attachment(att));
                    }

                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
