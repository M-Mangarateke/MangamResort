using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

namespace ManResort.Pages
{
    public class ContactModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public ContactModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Surname { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Message { get; set; }

        public string FeedbackMessage { get; set; }

        public void OnGet()
        {
            // Initialize any variables or state here if needed
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                FeedbackMessage = "Please fill in all fields correctly.";
                return Page();
            }

            try
            {
                // Fetch email settings from configuration
                var emailSettings = _configuration.GetSection("EmailSettings");
                string smtpHost = emailSettings["SMTPHost"];
                int smtpPort = int.Parse(emailSettings["SMTPPort"]);
                string fromEmail = emailSettings["FromEmail"];
                string password = emailSettings["Password"];
                string toEmail = "mangaratekethe1@gmail.com"; // Your business email address

                // Configure SMTP client
                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                // Create email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "New Contact Form Submission",
                    Body = $"Name: {Name}\nEmail: {Email}\nMessage: {Message}",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(toEmail);

                // Send the email
                smtpClient.Send(mailMessage);

                FeedbackMessage = "Your message has been sent successfully!";
            }
            catch (Exception ex)
            {
                FeedbackMessage = "Failed to send your message. Please try again later. Error: " + ex.Message;
            }

            return Page();
        }
    }
}
