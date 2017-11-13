namespace CBHWA.Clases
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Web;
    using Utilidades;

    public class MailHelper
    {
        public void SendMail(MailMessage msg)
        {
            /*
             * Cliente SMTP
             * Gmail:  smtp.gmail.com  puerto:587
             * Hotmail: smtp.liva.com  puerto:25
             */
            SmtpClient client = new SmtpClient(Properties.Settings.Default.SMTP_SERVER, Properties.Settings.Default.SMTP_PORT);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Properties.Settings.Default.SMTP_EMAIL, Properties.Settings.Default.SMTP_PWD); 
            client.Send(msg);
        }
    }
}