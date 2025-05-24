
using AutoMapper;
using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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
        private readonly IImageService _imageService;
        public EventService(IEventRepository eventRepository,
                            IValidator<Event> eventValidator,
                            IMapper mapper,
                            IImageService imageService) 
        {
            _eventRepository = eventRepository;
            _eventValidator = eventValidator;
            _mapper = mapper;
            _imageService = imageService;
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
        public async Task RemoveImageFromEvent(string eventId)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new Exception("Event not found");
            if (foundEvent.ImagePath == null) throw new Exception("Event doesn't have image");

            _imageService.DeleteImage(foundEvent.ImagePath);

            foundEvent.ImagePath = null;

            await _eventRepository.UpdateEvent(foundEvent);
        }
        public async Task AddImageToEvent(string eventId, IFormFile imageFile)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new Exception("Event not found");

            foundEvent.ImagePath = await _imageService.SaveImageAsync(imageFile);

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task<(FileStream, string)> GetImage(string eventId)
        {

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new Exception("Event not found");
            if (foundEvent.ImagePath == null) throw new Exception("Event doesn't have image");

            string ext;
            FileStream imageFile;
         
            (imageFile,ext) = _imageService.GetImageStream(foundEvent.ImagePath);
            if (ext == null || imageFile == null) throw new Exception("Couldn't open filestream or get file extention");
            
            ext = $"image/{ext.TrimStart('.').ToLower()}";
            
            return (imageFile,ext);
        }
    }
}
