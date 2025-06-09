using Data.Models;

namespace Data.Interfaces
{
    public interface IEventRepository
    {
        public Task CreateEvent(Event newEvent, CancellationToken cancellationToken = default);
        public Task<Event?> UpdateEvent(Event updatedEvent, CancellationToken cancellationToken = default);
        public Task DeleteEvent(int eventId, CancellationToken cancellationToken = default);
        public Task<Event?> GetEventById(int eventId, CancellationToken cancellationToken = default);
        public Task<Event?> GetEventByName(string eventName, CancellationToken cancellationToken = default);
        public Task<PaginatedList<Event>> GetEvents(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        public Task<PaginatedList<Event>> GetEventsFiltered(Event filter,int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        public Task<int> GetEventParticipantsCount(int eventId, CancellationToken cancellationToken = default);
        public Task<List<Event>> GetUserEvents(string userId, CancellationToken cancellationToken = default);
    }
}
