using System.Net;
using System.Net.Mail;

namespace TalepWebProject.WebAPI.Bussines
{
    public class MailService
    {
        public static void Send(string To, string Subject, string Body)
        {
            try
            {
                using (SmtpClient sc = new SmtpClient())
                {
                    sc.Port = 587;
                    sc.Host = "smtp.live.com";
                    sc.EnableSsl = true;
                    sc.Timeout = 50000;
                    sc.Credentials = new NetworkCredential("denemeadmn23@hotmail.com", "lzsekjshgefdalbk");
                    var mail = new MailMessage();
                    mail.From = new MailAddress("denemeadmn23@hotmail.com", "Talep Web Project");
                    mail.To.Add(new MailAddress(To));
                    mail.Subject = Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = Body;
                    sc.Send(mail);
                }
            }
            catch
            {
            }
        }
    }
}
