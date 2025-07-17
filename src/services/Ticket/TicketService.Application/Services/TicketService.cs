
using AutoMapper;
using TicketService.Application.DTOs;
using TicketService.Application.Interfaces;
using TicketService.Domain.Interfaces;
using TicketService.Domain.Models;

namespace TicketService.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IMapper _mapper;

    public TicketService(ITicketRepository ticketRepository, IMapper mapper)
    {
        _ticketRepository = ticketRepository;
        _mapper = mapper;
    }

    public async Task<TicketDto> GetTicketByIdAsync(string ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
        return _mapper.Map<TicketDto>(ticket);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetTicketsByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<string> AddTicketAsync(TicketDto ticketDto, CancellationToken cancellationToken)
    {
        var ticket = _mapper.Map<Ticket>(ticketDto);
        return await _ticketRepository.AddTicketAsync(ticket);
    }

    public async Task UpdateTicketAsync(TicketDto ticketDto, CancellationToken cancellationToken)
    {
        var ticket = _mapper.Map<Ticket>(ticketDto);
        await _ticketRepository.UpdateTicketAsync(ticket);
    }

    public async Task<bool> DeleteTicketAsync(string ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetTicketByIdAsync(ticketId);
        if (ticket != null)
        {
            await _ticketRepository.DeleteTicketAsync(ticketId);
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync(CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetAllTicketsAsync();
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<IEnumerable<TicketDto>> SearchTicketsAsync(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.SearchTicketsAsync(searchTerm, sortBy, sortDescending, pageNumber, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetTicketsByTagIdAsync(tagId, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetTicketsByStatusAsync(status, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<IEnumerable<TicketDto>> GetTicketsByPriorityAsync(string priority, CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetTicketsByPriorityAsync(priority, cancellationToken);
        return _mapper.Map<IEnumerable<TicketDto>>(tickets);
    }

    public async Task<byte[]> ExportTicketsAsync(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.SearchTicketsAsync(searchTerm, sortBy, sortDescending, pageNumber, pageSize, cancellationToken);
        var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

        // This is a simplified example of generating a CSV.
        // In a real application, you might use a library like CsvHelper or EPPlus for more robust CSV/Excel generation.
        var stringBuilder = new System.Text.StringBuilder();
        // Add CSV header
        stringBuilder.AppendLine("Id,Title,Description,Status,Priority,UserId,CreatedAt,UpdatedAt,AssignedTo,Resolution,ClosedAt");
        // Add ticket data
        foreach (var ticket in ticketDtos)
        {
            stringBuilder.AppendLine($"\"{ticket.Id}\",\"{ticket.Title}\",\"{ticket.Description}\",\"{ticket.TicketPriority}\",\"{ticket.UserId}\"");
        }

        return System.Text.Encoding.UTF8.GetBytes(stringBuilder.ToString());
    }
}