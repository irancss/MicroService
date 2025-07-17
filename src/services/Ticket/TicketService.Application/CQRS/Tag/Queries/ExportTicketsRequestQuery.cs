using MediatR;
using TicketService.Application.Interfaces; // Assuming ITicketService is defined here

namespace TicketService.Application.CQRS.Tag.Queries;

public class ExportTicketsRequestQuery(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber = 1, int pageSize = 10)
    : IRequest<byte[]>
{
    public string? SearchTerm { get; } = searchTerm;
    public string? SortBy { get; } = sortBy;
    public bool SortDescending { get; } = sortDescending;
    public int PageNumber { get; } = pageNumber;
    public int PageSize { get; } = pageSize;

    public class ExportTicketsRequestQueryHandler(ITicketService ticketService)
        : IRequestHandler<ExportTicketsRequestQuery, byte[]>
    {
        private readonly ITicketService _ticketService = ticketService;

        public async Task<byte[]> Handle(ExportTicketsRequestQuery request, CancellationToken cancellationToken)
        {
            // This assumes ITicketService has a method like ExportTicketsAsync
            // that takes these parameters and returns the file content as a byte array.
            // Example:
            // public interface ITicketService
            // {
            //     Task<byte[]> ExportTicketsAsync(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber, int pageSize, CancellationToken cancellationToken);
            // }
            
            byte[] fileBytes = await _ticketService.ExportTicketsAsync(
                request.SearchTerm,
                request.SortBy,
                request.SortDescending,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
            
            return fileBytes;
        }
    }
}