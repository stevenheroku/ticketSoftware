using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace Bdl_Grupo2_ProyectoFinal_A.Conexion
{
    public class SendEmail
    {
        public bool sendMail(string to, string asunto, string email, string code, string title, bool link = false)
        {  
            string body = @"
                        <div style='background-color:lightblue;border:1px#000000;-moz-border-radius:7px;-webkit-border-radius:7px;padding:10px;text-align:center;'>
                        <img src='https://dsm04pap002files.storage.live.com/y4m-poBEYGGMk9Z9hx0ezfn_7pnsSMOwDZedjegdicErQNpJq3dsRktFJNKob8P-eQJutYKN-gX25i5F92Ge0tCxCMrJh7IfPFJ_wE0KMw-tUpjy7KkZ9iW4ml1u8i7AiJw-12Q6Uo-FySMu3QdAbyq8ZHmVr0uw8IEiDOvhrKO0M4TTbxhvPU-vgXif95Q5Y3w?width=1080&height=720&cropmode=none' style='width:250PX;heigth:250px'>
                        <h1 style='color:#7c795d;font-family:'Trocchi',serif;font-size:45px;font-weight:normal;line-height:48px;margin:0;'>Notificación DevSolutions</h1>
                        <p style='width:1000px;color:#4c4a37;font-family:'Source Sans Pro',sans-serif;font-size:18px;line-height:32px;margin:24px;'>" + email + @"</p>
                        <h2 style='color:#7c795d;font-family:'Source Sans Pro',sans-serif;font-size:28px;font-weight:400;line-height:32px;margin:24px;'>" + title + @" <b>" + code + @"</b></h2>          
                        <a href='https://localhost:44368/Ticket/Crear' role='button'>Crear Nuevo Ticket</a></div>
                        <hr style='margin-top:20px;'>
                        <p style='color:#a0a6b5;font-size:12px;padding-bottom:10px;text-align:center;line-height:18px;'>Has recibido este e - mail porque eres usuario registrado en DevSolutions al amparo de nuestra Política de Privacidad.Este e-mail se ha enviado desde DevSolutions
                        (DGNET Ltd, con número de registro 189977 y domicilio en 64A Cumberland Street, Edimburgo EH3 6RE, Reino Unido)</p>";
            if (link)
            {
                body = @"
                        <div style='background-color:lightblue;border:1px#000000;-moz-border-radius:7px;-webkit-border-radius:7px;padding:10px;text-align:center;'>
                        <img src='https://i.ibb.co/KGnscbL/Logo-Dev-Solutions.png' style='width:400PX;'>
                        <h1 style='color:#7c795d;font-family:'Trocchi',serif;font-size:45px;font-weight:normal;line-height:48px;margin:0;'>Notificación DevSolutions</h1>
                        <p style='width:1000px;color:#4c4a37;font-family:'Source Sans Pro',sans-serif;font-size:18px;line-height:32px;margin:24px;'>" + email + @"</p>
                        <h2 style='color:#7c795d;font-family:'Source Sans Pro',sans-serif;font-size:28px;font-weight:400;line-height:32px;margin:24px;'>" + title + @" <b>" + code + @"</b></h2>          
                        <a href='https://localhost:44368/Ticket/ListTickets' role='button'>Ver tickets</a></div>
                        <hr style='margin-top:20px;'>
                        <p style='color:#a0a6b5;font-size:12px;padding-bottom:10px;text-align:center;line-height:18px;'>Has recibido este e - mail porque eres usuario registrado en DevSolutions al amparo de nuestra Política de Privacidad.Este e-mail se ha enviado desde DevSolutions
                        (DGNET Ltd, con número de registro 189977 y domicilio en 64A Cumberland Street, Edimburgo EH3 6RE, Reino Unido)</p>";
            }
            
            
            bool msge;
            string from = "josue360cjpimentel@gmail.com";
            string displayName = "DevSolutions";
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from, displayName);
                mail.To.Add(to);
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                client.Credentials = new NetworkCredential(from, "cesarin360");
                client.EnableSsl = true;
                client.Send(mail);
                msge = true;

            }
            catch(Exception e)//validad permisos del correo
            {
                msge = false;
            }

            return msge;
        }
    }
}