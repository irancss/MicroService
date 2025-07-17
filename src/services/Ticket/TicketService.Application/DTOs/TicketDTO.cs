using TicketService.Application.Mappings;
using TicketService.Domain.Enums;
using TicketService.Domain.Models;

namespace TicketService.Application.DTOs;

public class TicketDto : IMapFrom<Ticket>
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string TicketPriority { get; set; }
    public string TicketStatus { get; set; } 
    public string TagId { get; set; }
}

public class CreateTagRequest : IMapFrom<Ticket>
{
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public class UpdateTagRequest 
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public class DeleteTagRequest 
{
    public string Id { get; set; }
}

public class UpdateTagDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}