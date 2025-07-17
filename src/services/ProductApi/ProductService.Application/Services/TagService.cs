
using AutoMapper;
using ProductService.Application.DTOs.Tag;
using ProductService.Domain.Interfaces;
using ProductService.Domain.Models;

public class TagService: ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly IMapper _mapper;

    public TagService(ITagRepository tagRepository, IMapper mapper)
    {
        _tagRepository = tagRepository;
        _mapper = mapper;
    }

    public async Task<TagDto> CreateTagAsync(TagDto tagDto)
    {
        var tag = _mapper.Map<Tag>(tagDto);
        var createdTag = await _tagRepository.AddAsync(tag);
        return _mapper.Map<TagDto>(createdTag);
    }

    public async Task<TagDto?> GetTagByIdAsync(string tagId)
    {
        var tag = await _tagRepository.GetByIdAsync(tagId);
        return tag != null ? _mapper.Map<TagDto>(tag) : null;
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<TagDto?> UpdateTagAsync(string tagId, TagDto tagDto)
    {
        var existingTag = await _tagRepository.GetByIdAsync(tagId);
        if (existingTag == null) return null;

        var updatedTag = _mapper.Map(tagDto, existingTag);
        await _tagRepository.UpdateAsync(updatedTag);
        return _mapper.Map<TagDto>(updatedTag);
    }


    public async Task<bool> DeleteTagAsync(string tagId)
    {
        var existingTag = await _tagRepository.GetByIdAsync(tagId);
        if (existingTag == null) return false;
        await _tagRepository.DeleteAsync(tagId);
        return true;
    }
}