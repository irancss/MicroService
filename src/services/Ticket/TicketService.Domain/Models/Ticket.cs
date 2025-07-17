
using TicketService.Domain.Common;
using TicketService.Domain.Enums;

namespace TicketService.Domain.Models
{
    public class Ticket : AuditableEntity
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public TicketPriority TicketPriority { get; set; } = TicketPriority.Normal;
        public TicketStatus TicketStatus { get; set; } = TicketStatus.Open;

        public string TagId { get; set; }
        public Tag Tag { get; set; }

        public Ticket(string userId, string title, string description, string tagId)
        {
            UserId = userId;
            Title = title;
            Description = description;
            TagId = tagId;
            TicketStatus = TicketStatus.Open;
        }

    }
}
