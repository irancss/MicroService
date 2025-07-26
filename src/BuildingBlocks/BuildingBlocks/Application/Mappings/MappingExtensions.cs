using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Application.Mappings;

/// <summary>
/// Provides extension methods for AutoMapper projections.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Projects an IQueryable to a list of destination types and executes the query asynchronously.
    /// </summary>
    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync();
}