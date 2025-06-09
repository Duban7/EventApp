using Data.Context;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventAppDbContext _context;
        public EventRepository(EventAppDbContext context) 
        {
            _context = context;
        }

        public async Task CreateEvent(Event newEvent, CancellationToken cancellationToken)
        {
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteEvent(int eventId, CancellationToken cancellationToken)
        {
          await _context.Events.Where(e => e.Id == eventId).ExecuteDeleteAsync(cancellationToken);
        }

        public Task<Event?> GetEventById(int eventId, CancellationToken cancellationToken) =>
            _context.Events
            .Where(e => e.Id == eventId)
            .Include(e=>e.Participants)
            .FirstOrDefaultAsync(cancellationToken);

        public Task<Event?> GetEventByName(string eventName, CancellationToken cancellationToken) =>
            _context.Events
            .AsNoTracking()
            .Where(e => e.Name == eventName)
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(cancellationToken);

        public async Task<PaginatedList<Event>> GetEvents(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            List<Event>? events = await _context.Events
            .AsNoTracking()
            .OrderBy(e => e.StartDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

            var count = await _context.Events.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Event>(events, pageIndex, totalPages);
        }

        public async Task<PaginatedList<Event>> GetEventsFiltered(Event filter, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var query = _context.Events.AsQueryable<Event>();

            if (filter.Name != null)
                query = query.Where(x => x.Name.Contains(filter.Name));
            if (filter.Category != null)
                query = query.Where(x => x.Category.Contains(filter.Category));
            if (filter.StartDate.HasValue)
                query = query.Where(x => x.StartDate!.Value.Day == filter.StartDate.Value.Day);
            if (filter.EventPlace != null)
                query = query.Where(x => x.EventPlace.Contains(filter.EventPlace));

            query = query.OrderBy(e => e.StartDate)
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize);

            List<Event>? events = await query.AsNoTracking().ToListAsync(cancellationToken);

            var count = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PaginatedList<Event>(events, pageIndex, totalPages);
        }

        public async Task<Event?> UpdateEvent(Event updatedEvent, CancellationToken cancellationToken)
        {
            _context.Events.Update(updatedEvent);
            await _context.SaveChangesAsync(cancellationToken);

            return await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == updatedEvent.Name, cancellationToken);
        }

        public async Task<List<Event>> GetUserEvents(string userId, CancellationToken cancellationToken)=>
            await _context.Events.Where(e=>e.Participants.Any(u=>u.Id == userId))
                                 .AsNoTracking()
                                 .ToListAsync(cancellationToken);

        public async Task<int> GetEventParticipantsCount(int eventId, CancellationToken cancellationToken) =>
            await _context.Events.Where(e => e.Id == eventId)
                           .Select(e => e.Participants.Count)
                           .FirstOrDefaultAsync(cancellationToken);
    }
}
