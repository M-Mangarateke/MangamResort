using ManResort.Model;
using Microsoft.EntityFrameworkCore;

namespace ManResort.Data
{
    public class TicketDBContext : DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }

        public TicketDBContext(DbContextOptions<TicketDBContext> options)
            : base(options)
        {
        }
    
    }
}
