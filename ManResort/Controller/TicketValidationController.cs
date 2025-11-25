using ManResort.Data;
using Microsoft.AspNetCore.Mvc;

namespace ManResort.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketValidationController : ControllerBase
    {
        private readonly TicketDBContext _dbContext;

        public TicketValidationController(TicketDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("ValidateQRCode")]
        public async Task<IActionResult> ValidateQRCode([FromBody] int ticketId)
        {
            var ticket = await _dbContext.Tickets.FindAsync(ticketId);

            if (ticket == null)
            {
                return BadRequest("Ticket not found.");
            }

            if (ticket.IsRedeemed)
            {
                return BadRequest("Ticket has already been redeemed.");
            }

            ticket.IsRedeemed = true;
            await _dbContext.SaveChangesAsync();

            return Ok("Ticket validated successfully.");
        }
    }
}