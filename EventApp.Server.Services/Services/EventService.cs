using AutoMapper;
using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Services.DTOs;
using Services.Exeptions;
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
        public async Task<EventDTO> CreateEvent(CreateEventDTO newEventDTO, CancellationToken cancellationToken)
        {
            Event newEvent = _mapper.Map<Event>(newEventDTO);

            _eventValidator.ValidateAndThrow(newEvent);
            if (newEvent.StartDate < DateTime.Now) throw new BadRequestException("Event cannot start in past");
            if (newEvent.MaxParticipantsCount < 3 || newEvent.MaxParticipantsCount > 300)
                throw new BadRequestException("Partitipants count should be more than 3 and less than 300");

            Event? foundEvent = await _eventRepository.GetEventByName(newEvent.Name!,  cancellationToken);
            if (foundEvent != null) throw new ConflictException("Event already exists");

            await _eventRepository.CreateEvent(newEvent, cancellationToken);

            Event? createdEvent = await _eventRepository.GetEventByName(newEvent.Name!, cancellationToken);

            EventDTO eventDTO = _mapper.Map<EventDTO>(createdEvent);

            return eventDTO;
        }

        public async Task DeleteEvent(int eventId, CancellationToken cancellationToken)
        {
            Event? foundEvent = (await _eventRepository.GetEventById(eventId, cancellationToken)) ?? throw new NotFoundException("Event not found");

            await _eventRepository.DeleteEvent(eventId, cancellationToken); 
        }

        public async Task<EventDTO> GetEventById(int eventId, CancellationToken cancellationToken)
        {
            Event foundEvent = (await _eventRepository.GetEventById(eventId, cancellationToken)) ?? throw new NotFoundException("Event not found");

            EventDTO eventDTO = _mapper.Map<EventDTO>(foundEvent);

            return eventDTO;
        }

        public async Task<EventDTO> GetEventByName(string name, CancellationToken cancellationToken)
        {
            Event foundEvent = (await _eventRepository.GetEventByName(name, cancellationToken)) ?? throw new NotFoundException("Event not found");

            EventDTO eventDTO = _mapper.Map<EventDTO>(foundEvent);

            return eventDTO;
        }
           

        public async Task<PaginatedList<EventDTO>> GetEventsFiltered(EventFilterDTO filterDTO, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            Event filter = _mapper.Map<Event>(filterDTO);

            var res = await _eventRepository.GetEventsFiltered(filter, pageIndex, pageSize, cancellationToken);

            var dtos = res.items?.Select(_mapper.Map<EventDTO>).ToList();

            return new PaginatedList<EventDTO>(dtos, pageIndex, res.TotalPages);
        }

        public async Task<PaginatedList<EventDTO>> GetEvents(int pageIndex, int PageSize, CancellationToken cancellationToken = default)
        {
            var res = await _eventRepository.GetEvents(pageIndex, PageSize, cancellationToken);

            var dtos = res.items?.Select(_mapper.Map<EventDTO>).ToList();

            return new PaginatedList<EventDTO>(dtos, pageIndex, res.TotalPages);
        }
           
        public async Task<EventDTO> UpdateEvent(UpdateEventDTO updatedEventDTO, CancellationToken cancellationToken)
        {
            Event updatedEvent = _mapper.Map<Event>(updatedEventDTO);

            _eventValidator.ValidateAndThrow(updatedEvent);
            if (updatedEvent.StartDate < DateTime.Now) throw new BadRequestException("Event cannot start in past");
            if (updatedEvent.MaxParticipantsCount < 3 || updatedEvent.MaxParticipantsCount > 300)
                throw new BadRequestException("Partitipants count should be more than 3 and less than 300");

            Event? foundEvent = await _eventRepository.GetEventById(updatedEvent.Id, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");

            if (updatedEvent.Description != null)
                foundEvent.Description = updatedEvent.Description;
            if (updatedEvent.StartDate != null)
                foundEvent.StartDate = updatedEvent.StartDate;
            if (updatedEvent.Category != null)
                foundEvent.Category = updatedEvent.Category;
            if (updatedEvent.EventPlace != null)
                foundEvent.EventPlace = updatedEvent.EventPlace;
            if (updatedEvent.MaxParticipantsCount != null) 
            {
                int ParticipantsCount = await _eventRepository.GetEventParticipantsCount(updatedEvent.Id, cancellationToken);
                if (updatedEvent.MaxParticipantsCount < ParticipantsCount)
                    throw new BadRequestException("You couldn't set max participant count less than real participants count");
                if (updatedEvent.MaxParticipantsCount == ParticipantsCount)
                    foundEvent.IsFull = true;
                if (foundEvent.IsFull && updatedEvent.MaxParticipantsCount > ParticipantsCount)
                    foundEvent.IsFull = false;
                foundEvent.MaxParticipantsCount = updatedEvent.MaxParticipantsCount; 
            }

            Event? resEvent = (await _eventRepository.UpdateEvent(foundEvent, cancellationToken));

            EventDTO resEventDTO = _mapper.Map<EventDTO>(resEvent);

            return resEventDTO;
        }
        public async Task RemoveImageFromEvent(int eventId, CancellationToken cancellationToken)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.ImagePath == null) return;

            _imageService.DeleteImage(foundEvent.ImagePath);

            foundEvent.ImagePath = null;

            await _eventRepository.UpdateEvent(foundEvent);
        }
        public async Task AddImageToEvent(int eventId, IFormFile imageFile, CancellationToken cancellationToken)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");

            foundEvent.ImagePath = await _imageService.SaveImageAsync(imageFile, cancellationToken);

            await _eventRepository.UpdateEvent(foundEvent, cancellationToken);
        }

        public async Task<(FileStream, string)> GetImage(int eventId, CancellationToken cancellationToken)
        {

            Event? foundEvent = await _eventRepository.GetEventById(eventId, cancellationToken);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.ImagePath == null) throw new BadRequestException("Event doesn't have image");

            string ext;
            FileStream imageFile;
         
            (imageFile,ext) = _imageService.GetImageStream(foundEvent.ImagePath);
            if (ext == null || imageFile == null) throw new InternalErrorException("Couldn't open filestream or get file extention");
            
            ext = $"image/{ext.TrimStart('.').ToLower()}";
            
            return (imageFile,ext);
        }


        public async Task<List<EventDTO>> GetUserEvents(string userId, CancellationToken cancellationToken)
        {
            List<Event> foundEvents = await _eventRepository.GetUserEvents(userId, cancellationToken);

            List<EventDTO> foundEventsDTOs = foundEvents.Select(e=>_mapper.Map<EventDTO>(e)).ToList();

            return foundEventsDTOs;
        }
    }
}
