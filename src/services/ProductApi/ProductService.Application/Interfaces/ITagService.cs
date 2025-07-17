
using ProductService.Application.DTOs.Tag;

public interface ITagService
{
    Task<TagDto> CreateTagAsync(TagDto tagDto);
    Task<TagDto?> GetTagByIdAsync(string tagId);
    Task<IEnumerable<TagDto>> GetAllTagsAsync();
    Task<TagDto?> UpdateTagAsync(string tagId, TagDto tagDto);
    Task<bool> DeleteTagAsync(string tagId);
}