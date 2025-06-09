using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.DTOs;

namespace Services.Interfaces
{
    public interface IEventService
    {
        public Task<EventDTO> CreateEvent(CreateEventDTO newEventDTO, CancellationToken cancellationToken = default);
        public Task<EventDTO> UpdateEvent(UpdateEventDTO updatedEventDTO, CancellationToken cancellationToken = default);
        public Task DeleteEvent(int eventId, CancellationToken cancellationToken = default);
        public Task<EventDTO> GetEventById(int eventId, CancellationToken cancellationToken = default);
        public Task<EventDTO> GetEventByName(string name, CancellationToken cancellationToken = default);
        public Task<PaginatedList<EventDTO>> GetEvents(int pageIndex, int PageSize, CancellationToken cancellationToken = default);
        public Task<PaginatedList<EventDTO>> GetEventsFiltered(EventFilterDTO filter, int pageIndex, int PageSize, CancellationToken cancellationToken = default);
        public Task AddImageToEvent(int eventId, IFormFile imageFile, CancellationToken cancellationToken = default);
        public Task RemoveImageFromEvent(int eventId, CancellationToken cancellationToken = default);
        public Task<(FileStream, string)> GetImage(int eventId, CancellationToken cancellationToken = default);
        public Task<List<EventDTO>> GetUserEvents(string userId, CancellationToken cancellationToken = default);
    }
}
