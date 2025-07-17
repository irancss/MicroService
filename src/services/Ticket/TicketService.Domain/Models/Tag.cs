using TicketService.Domain.Common;

namespace TicketService.Domain.Models
{
    public class Tag : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // لیستی از تیکت‌ها که به این تگ مرتبط هستند
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public Tag() { }

        public Tag(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
