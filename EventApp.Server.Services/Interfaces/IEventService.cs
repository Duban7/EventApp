using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.DTOs;

namespace Services.Interfaces
{
    public interface IEventService
    {
        public Task<EventDTO> CreateEvent(CreateEventDTO newEventDTO);
        public Task<EventDTO> UpdateEvent(UpdateEventDTO updatedEventDTO);
        public Task DeleteEvent(int eventId);
        public Task<EventDTO> GetEventById(int eventId);
        public Task<EventDTO> GetEventByName(string name);
        public Task<PaginatedList<EventDTO>> GetEvents(int pageIndex, int PageSize);
        public Task<PaginatedList<EventDTO>> GetEventsFiltered(EventFilterDTO filter, int pageIndex, int PageSize);
        public Task AddImageToEvent(int eventId, IFormFile imageFile);
        public Task RemoveImageFromEvent(int eventId);
        public Task<(FileStream, string)> GetImage(int eventId);
        public Task<List<EventDTO>> GetUserEvents(string userId);
    }
}
