using Data.Models;
using Microsoft.AspNetCore.Http;
using Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEventService
    {
        public Task<Event> CreateEvent(Event newEvent);
        public Task<Event> UpdateEvent(Event updatedEvent);
        public Task DeleteEvent(int eventId);
        public Task<Event> GetEventById(int eventId);
        public Task<Event> GetEventByName(string name);
        public Task<PaginatedList<Event>> GetEvents(int pageIndex, int PageSize);
        public Task<PaginatedList<Event>> GetEventsFiltered(EventFilterDTO filter, int pageIndex, int PageSize);
        public Task AddImageToEvent(int eventId, IFormFile imageFile);
        public Task RemoveImageFromEvent(int eventId);
        public Task<(FileStream, string)> GetImage(int eventId); 
    }
}
