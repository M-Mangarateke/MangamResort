using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ManResort.Model;

namespace ManResort.Pages
{
    public class TicketConfirmationModel : PageModel
    {
        [BindProperty]
        public Ticket Ticket { get; set; }

        public string YocoPublicKey { get; private set; } = string.Empty;

        private readonly IConfiguration _configuration;

        public TicketConfirmationModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            // Retrieve ticket data from TempData
            if (TempData["Ticket"] == null)
            {
                return RedirectToPage("/Tickets");
            }

            Ticket = System.Text.Json.JsonSerializer.Deserialize<Ticket>((string)TempData["Ticket"]);
            TempData.Keep("Ticket"); // Preserve TempData for the next request
            YocoPublicKey = _configuration["Yoco:PublicKey"] ?? _configuration["YocoPublicKey"] ?? string.Empty;

            return Page();
        }

        public IActionResult OnPost()
        {
            // Proceed to payment
            return RedirectToPage("/Tickets");
        }
    }
}
