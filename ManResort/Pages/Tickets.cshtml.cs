using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ManResort.Data;
using ManResort.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using QRCoder.Core;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


namespace ManResort.Pages
{
    public class TicketsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly TicketDBContext _dbContext;

        public TicketsModel(IConfiguration configuration, TicketDBContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [BindProperty]
        public Ticket Ticket { get; set; }

        public decimal CalculateTotalPrice()
        {
            if (Ticket == null)
            {
                Ticket = new Ticket(); // Ensure Ticket is initialized
            }

            return (Ticket.Adults * 80) + (Ticket.Kids * 50) +
                   (Ticket.HolidayAdults * 100) + (Ticket.HolidayKids * 70) +
                   (Ticket.CoolerBoxes * 20) + (Ticket.BusinessEntrance * 120) +
                   (Ticket.BusinessElectricEntrance * 220);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ticket.TotalPrice = CalculateTotalPrice();

            // Store the ticket in TempData to pass it to the confirmation page
            TempData["Ticket"] = System.Text.Json.JsonSerializer.Serialize(Ticket);

            return RedirectToPage("/TicketConfirmation");
        }

        public async Task<IActionResult> OnPostProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            string yocoSecretKey = _configuration["Yoco:SecretKey"] ?? _configuration["YocoSecretKey"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", yocoSecretKey);

            var payload = new
            {
                token = paymentRequest.Token,
                amountInCents = paymentRequest.Amount
            };

            var response = await httpClient.PostAsJsonAsync("https://online.yoco.com/v1/charges/", payload);

            if (response.IsSuccessStatusCode)
            {
                // Save the payment reference
                var responseData = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseData);

                Ticket.PaymentReference = jsonResponse["id"]; // Or appropriate field in Yoco's API response
                Ticket.TotalPrice = paymentRequest.Amount / 100M;
                Ticket.PaymentStatus = "Paid";
                Ticket.PurchaseDate = DateTime.Now;

                string qrCodePath = GenerateQRCode(Ticket);
                Ticket.QRCodePath = qrCodePath;

                await SaveTicketToDatabase(Ticket);
                SendConfirmationEmail(Ticket);

                return new JsonResult(new { success = true });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new JsonResult(new { success = false, message = error });
            }
        }


        private async Task SaveTicketToDatabase(Ticket ticket)
        {
            _dbContext.Tickets.Add(ticket);
            await _dbContext.SaveChangesAsync();
        }

        private string GenerateQRCode(Ticket ticket)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode($"TicketID: {ticket.TicketId}\nName: {ticket.CustomerName}\nTotal: {ticket.TotalPrice:C}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new QRCode(qrData);

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrCodes");
            Directory.CreateDirectory(uploadsFolder);

            string qrFilePath = Path.Combine(uploadsFolder, $"Ticket_{ticket.TicketId}.png");
            using var qrImage = qrCode.GetGraphic(20);
            qrImage.Save(qrFilePath);

            return qrFilePath;
        }

        private async Task SaveToBlobStorage(string filePath)
        {
            string connectionString = _configuration["ConnectionStrings:AzureBlobStorage"];
            string containerName = "ticketqrcodes";

            // Create BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Create container client
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            string blobName = Path.GetFileName(filePath);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using FileStream fs = System.IO.File.OpenRead(filePath); // Fully qualified to avoid conflicts
            await blobClient.UploadAsync(fs, true);
        }


        private void SendConfirmationEmail(Ticket ticket)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPHost"])
            {
                Port = int.Parse(_configuration["EmailSettings:SMTPPort"]),
                Credentials = new System.Net.NetworkCredential(_configuration["EmailSettings:FromEmail"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Mangam Resort - Ticket Confirmation",
                Body = $@"
Dear {ticket.CustomerName},

Thank you for your purchase! Your ticket details:
- Total: {ticket.TotalPrice:C}
- Redeem Date: {ticket.RedeemDate.ToShortDateString()}

Attached is your QR code for entry.

Best regards,
Mangam Resort",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(ticket.CustomerEmail);

            if (!string.IsNullOrEmpty(ticket.QRCodePath))
            {
                mailMessage.Attachments.Add(new Attachment(ticket.QRCodePath));
            }

            smtpClient.Send(mailMessage);
        }
    }


    public class PaymentRequest
    {
        public string Token { get; set; }
        public int Amount { get; set; }
    }
}
