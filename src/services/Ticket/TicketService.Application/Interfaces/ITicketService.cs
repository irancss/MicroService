
using TicketService.Application.DTOs;

namespace TicketService.Application.Interfaces;

public interface ITicketService
{
    Task<TicketDto> GetTicketByIdAsync(string ticketId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<string> AddTicketAsync(TicketDto ticketDto, CancellationToken cancellationToken = default);
    Task UpdateTicketAsync(TicketDto ticketDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTicketAsync(string ticketId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> GetAllTicketsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TicketDto>> SearchTicketsAsync(
    string? searchTerm,
    string? sortBy,
    bool sortDescending,
    int pageNumber = 1,
    int pageSize = 10,
    CancellationToken cancellationToken = default);

    Task<IEnumerable<TicketDto>> GetTicketsByTagIdAsync(string tagId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> GetTicketsByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> GetTicketsByPriorityAsync(string priority, CancellationToken cancellationToken = default);
        Task<byte[]> ExportTicketsAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}