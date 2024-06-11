using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace CursoMod165.Services
{
     // as iniciadas por são classes interface ...
    // temos de fazer implement interface right click   Ctrl +
    public class EmailSender : IEmailSender   // ":" corresponde a uma herança
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage) 
        {
            // throw new NotImplementedException();
            // Aqui vamos mandar um email ...
            // criamos um cliente smtp. que é um cliente que envia emails
            // Credentials = new NetworkCredential("cursoMOD165DiogoSilva@gmail.com","djrwpbxmhurvsgug"),
            // From = new MailAddress("cursoMOD165DiogoSilva@gmail.com", "Seguro Saúde Municipal"),
            // cursoMOD165PADO@gmail.com     gcuq qbzl mhze epxe  -> Criado acesso APP password
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential("oliveiraxpedro@gmail.com", "gcuqqbzlmhzeepxe"),  // aqui coloca-se chave que so permite envio de emails (toquen)
                Port = 587,
                EnableSsl=true,  // activar uso secure socket layer
            };

            // aqui envia-se o email
            MailMessage mailMessage = new MailMessage()
            {
                // pode ser feito como se fosse telnet; cursoMOD165PADO@gmail.com;   gcuqqbzlmhzeepxe
                // cursoMOD165Pado2@gmail.com;  dgfm orbt zzgu yslr
                From = new MailAddress("oliveiraxpedro@gmail.com", "Seguro Saúde Municipal"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,

            };

            mailMessage.To.Add(email);

            mailMessage.Bcc.Add("pedroxoliveira@sapo.pt");  // sempre bcc para ter uma copia sempre "cursoMOD165DiogoSilva@gmail.com"

            smtpClient.Send(mailMessage);

            return Task.CompletedTask;  

        }

    }
}
