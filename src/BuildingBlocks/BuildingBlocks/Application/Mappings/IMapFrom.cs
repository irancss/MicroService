using AutoMapper;

namespace BuildingBlocks.Application.Mappings;

/// <summary>
/// An interface to automatically create AutoMapper profiles for classes that implement it.
/// </summary>
/// <typeparam name="T">The source type to map from.</typeparam>
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}