using Microsoft.AspNetCore.Mvc.RazorPages;
using OrchardCore.Email;

namespace iClose.Pages
{
    public class SendModel : PageModel
    {
        private readonly ISmtpService _smtpService;

        public SendModel(ISmtpService smtpService) {
            _smtpService = smtpService;
        }
        public void OnGet() {
            var query = HttpContext.Request.Query;
            var template = "";
            foreach (var key in query.Keys) {
                if (query[key] != "" && key != "project_name" && key != "admin_email" && key != "form_subject" && key != "g-recaptcha-response") {
                    template += "<tr style='background-color: #f8f8f8;'>";
                    template += $"<td style='padding: 10px; border: #e9e9e9 1px solid;'><b>{key}</b></td><td style='padding: 10px; border: #e9e9e9 1px solid;'>{query[key]}</td>";
                    template += "</tr>";
                }
            }

            var body = $"<table style='width: 100%;'>{template}</table>";
            var mailMessage = new MailMessage() {
                To = query["admin_email"],
                From = query["email"],
                Body = body,
                IsBodyHtml = true,
                Subject = query["form_subject"]
            };
            _smtpService.SendAsync(mailMessage);

        }

        public void OnPost() {

        }
    }
}