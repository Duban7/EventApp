using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEventService
    {
        public Task<Event?> CreateEvent(Event newEvent);
        public Task<Event?> UpdateEvent(Event updatedEvent);
        public Task DeleteEven(string eventId);
        public Task<Event> GetEventById(string eventId);
        public Task<Event> GetEventByName(string name);
        public Task<PaginatedList<Event>> GetEvents(int pageIndex, int PageSize);
        public Task<PaginatedList<Event>> GetEventFiltered(Event filter, int pageIndex, int PageSize);
        public Task<string> AddImageToEvent(string eventId, object obj);
        public Task RemoveImageFromEvent(string eventId);
    }
}
