using Microsoft.Extensions.Logging;

namespace TicketService.Infrastructure.Persistence
{
    public class ApplicationDbContextInitialiser
    {
        private ILogger<ApplicationDbContextInitialiser> _logger;
        private TicketDbContext _context;
        public ApplicationDbContextInitialiser(TicketDbContext context, ILogger<ApplicationDbContextInitialiser> logger)
        {
            _context = context;
            _logger = logger;
        }
        public void Initialise()
        {
            try
            {
                _context.Database.EnsureCreated();
                _logger.LogInformation("Database created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            if (!_context.Tickets.Any())
            {
                _context.Tickets.AddRange(

                    //new Ticket { Name = "Ticket 2", Description = "Description 2", BasePrice = 20.00m },
                    //new Ticket { Name = "Ticket 3", Description = "Description 3", BasePrice = 30.00m }
                );
                await _context.SaveChangesAsync();
                _logger.LogInformation("Database seeded successfully.");
            }
            else
            {
                _logger.LogInformation("Database already contains data, skipping seeding.");
            }
        }
    }
}
