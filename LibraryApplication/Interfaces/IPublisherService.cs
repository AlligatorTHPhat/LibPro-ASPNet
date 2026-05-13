using LibraryApplication.DTOs;

public interface IPublisherService
{
    Task <PublisherCreateDto> CreateAsync(PublisherCreateDto dto);
    Task <PublisherUpdateDto> UpdateAsync(PublisherUpdateDto dto);
}
