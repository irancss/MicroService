

using Microsoft.EntityFrameworkCore;
using System.Linq;
using TicketService.Domain.Interfaces;
using TicketService.Domain.Models;

public class TicketRepository : ITicketRepository
{
    private readonly TicketDbContext _context;

    public TicketRepository(TicketDbContext context)
    {
        _context = context;
    }

    public async Task<Ticket> GetTicketByIdAsync(string id)
    {
        return await _context.Tickets.FindAsync(id);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId)
    {
        return await _context.Tickets.Where(ticket => ticket.UserId.ToString() == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets.ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> SearchTicketsAsync(string? searchTerm, string? sortBy, bool sortDescending, int pageNumber = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = _context.Tickets.AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            result = result.Where(ticket => ticket.Title.Contains(searchTerm) || ticket.Description.Contains(searchTerm));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            result = sortDescending ?
                result.OrderByDescending(ticket => EF.Property<object>(ticket, sortBy)) :
                result.OrderBy(ticket => EF.Property<object>(ticket, sortBy));
        }

        return await result.Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);
    }

    public async Task<string> AddTicketAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
        return ticket.Id; // Assuming Id is the primary key and is set after saving
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTicketAsync(string id)
    {
        var ticket = await GetTicketByIdAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<Ticket>> GetTicketsByTagIdAsync(string tagId, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(c=> c.Tag.Id == tagId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(c=>c.TicketStatus.ToString() == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByPriorityAsync(string priority, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .Where(c => c.TicketPriority.ToString() == priority)
            .ToListAsync(cancellationToken);
    }
}