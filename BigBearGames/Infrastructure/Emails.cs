using System.Net;
using BigBearGames.Models;
using System.Net.Mail;
using System.Threading.Tasks;



namespace BigBearGames.Infrastructure
{
    public class Emails
    {
        public async Task NewUserEmail(CreateModel model)
        {
            var body = "<p>Hello! <strong>" + model.Name + "</strong>, " + "Thank you for signing up! <br> You may now add comments for articles";
            var message = new MailMessage();
            message.To.Add(new MailAddress(model.Email));
            message.From = new MailAddress("mattsykesbbg@gmail.com"); //Change to local email
            message.Subject = "Thanks for Registering at Big Bear Games";
            message.Body = body; //need to add footer to email for contact links
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient()) //Smtp set up and send
            {
                var credential = new NetworkCredential()
                {
                    UserName = "mattsykesbbg@gmail.com", //change for bigbeargames user + pass
                    Password = "stunl0ck123" 
                };

                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
        }
    }
}