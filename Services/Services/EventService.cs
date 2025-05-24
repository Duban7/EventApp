
using AutoMapper;
using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Services.DTOs;
using Services.Interfaces;

namespace Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IValidator<Event> _eventValidator;
        private readonly IMapper _mapper;
        public EventService(IEventRepository eventRepository,
                            IValidator<Event> eventValidator,
                            IMapper mapper) 
        {
            _eventRepository = eventRepository;
            _eventValidator = eventValidator;
            _mapper = mapper;
        }
        public Task<string> AddImageToEvent(string eventId, object obj)
        {
            throw new NotImplementedException();
        }

        public async Task<Event> CreateEvent(Event newEvent)
        {
            _eventValidator.ValidateAndThrow(newEvent);

            Event? foundEvent = await _eventRepository.GetEventByName(newEvent.Name!);

            if (foundEvent != null) throw new Exception("Event already exists");

            Event createdEvent = (await _eventRepository.CreateEvent(newEvent)) ?? throw new Exception("event wasn't created");

            return createdEvent;
        }

        public async Task DeleteEvent(string eventId)
        {
            Event? foundEvent = (await _eventRepository.GetEventById(eventId)) ?? throw new Exception("Event not found");

            await _eventRepository.DeleteEvent(eventId); 
        }

        public async Task<Event> GetEventById(string eventId)=>
            (await _eventRepository.GetEventById(eventId)) ?? throw new Exception("Event not found");


        public async Task<Event> GetEventByName(string name) =>
            (await _eventRepository.GetEventByName(name)) ?? throw new Exception("Event not found");

        public async Task<PaginatedList<Event>> GetEventsFiltered(EventFilterDTO filterDTO, int pageIndex, int PageSize)
        {
            Event filter = _mapper.Map<Event>(filterDTO);

            var res = await _eventRepository.GetEventsFiltered(filter, pageIndex, PageSize);
            if (res == null) throw new Exception("Events not found");

            return res;
        }

        public async Task<PaginatedList<Event>> GetEvents(int pageIndex, int PageSize) =>
            (await _eventRepository.GetEvents(pageIndex, PageSize)) ?? throw new Exception("Event not found");

        public Task RemoveImageFromEvent(string eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<Event> UpdateEvent(Event updatedEvent)
        {
            _eventValidator.ValidateAndThrow(updatedEvent);

            if (updatedEvent.Id == null) throw new Exception("Event id wan't sent");

            Event? foundEvent = await _eventRepository.GetEventById(updatedEvent.Id);

            if (foundEvent == null) throw new Exception("Event not found");

            if (updatedEvent.Description != null)
                foundEvent.Description = updatedEvent.Description;
            if (updatedEvent.StartDate != null)
                foundEvent.StartDate = updatedEvent.StartDate;
            if (updatedEvent.Category != null)
                foundEvent.Category = updatedEvent.Category;
            if (updatedEvent.EventPlace != null)
                foundEvent.EventPlace = updatedEvent.EventPlace;
            if (updatedEvent.MaxParticipantsCount != null)
                foundEvent.MaxParticipantsCount = updatedEvent.MaxParticipantsCount;

            Event? resEvent = (await _eventRepository.UpdateEvent(foundEvent)) ?? throw new Exception("event wasn't created"); ;

            return resEvent;
        }
    }
}
