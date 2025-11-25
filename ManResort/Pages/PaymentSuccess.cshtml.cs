using Microsoft.AspNetCore.Mvc.RazorPages;
using ManResort.Model; // Ensure the namespace matches your Ticket model

namespace ManResort.Pages
{
    public class PaymentSuccessModel : PageModel
    {
        public Ticket Ticket { get; set; }

        public void OnGet(Ticket ticket)
        {
            // Simulate receiving the ticket details after payment
            Ticket = ticket;
        }
    }
}
