using Data.Models;


namespace Data.Interfaces
{
    public interface IEventRepository
    {
        public Task<Event?> CreateEvent(Event newEvent);
        public Task<Event?> UpdateEvent(Event updatedEvent);
        public Task DeleteEvent(string eventId);
        public Task<Event?> GetEventById(string eventId);
        public Task<Event?> GetEventByName(string eventName);
        public Task<PaginatedList<Event>> GetEvents(int pageIndex, int pageSize);
        public Task<PaginatedList<Event>> GetEventsFiltered(Event filter,int pageIndex, int pageSize);
    }
}
