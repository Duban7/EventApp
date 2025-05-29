using Data.Models;


namespace Data.Interfaces
{
    public interface IEventRepository
    {
        public Task CreateEvent(Event newEvent);
        public Task<Event?> UpdateEvent(Event updatedEvent);
        public Task DeleteEvent(int eventId);
        public Task<Event?> GetEventById(int eventId);
        public Task<Event?> GetEventByName(string eventName);
        public Task<PaginatedList<Event>> GetEvents(int pageIndex, int pageSize);
        public Task<PaginatedList<Event>> GetEventsFiltered(Event filter,int pageIndex, int pageSize);
        public Task<int> GetEventParticipantsCount(int eventId);
        public Task<List<Event>> GetUserEvents(string userId);
    }
}
