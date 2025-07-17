
using System.Globalization;
using System.Threading;
using TicketService.Domain.Models;

namespace TicketService.Domain.Interfaces;

public interface ITicketRepository
{
    Task<Ticket> GetTicketByIdAsync(string ticketId);
    Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId);
    Task<string> AddTicketAsync(Ticket ticket);
    Task UpdateTicketAsync(Ticket ticket);
    Task DeleteTicketAsync(string ticketId);
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();

    Task<IEnumerable<Ticket>>
        SearchTicketsAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Ticket>> GetTicketsByTagIdAsync(string tagId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(string priority, CancellationToken cancellationToken = default);
}