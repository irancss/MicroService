using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Services;

public class TagRepository : ITagRepository
{
    private readonly ProductDbContext _context;

    public TagRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<Tag> GetByIdAsync(string id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<Tag> GetByNameAsync(string name)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task<Tag> AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}