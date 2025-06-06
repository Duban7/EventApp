﻿
using AutoMapper;
using Data.Interfaces;
using Data.Models;
using Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
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
        public async Task<EventDTO> CreateEvent(CreateEventDTO newEventDTO)
        {
            Event newEvent = _mapper.Map<Event>(newEventDTO);

            _eventValidator.ValidateAndThrow(newEvent);

            Event? foundEvent = await _eventRepository.GetEventByName(newEvent.Name!);
            if (foundEvent != null) throw new ConflictException("Event already exists");

            await _eventRepository.CreateEvent(newEvent);

            Event createdEvent = (await _eventRepository.GetEventByName(newEvent.Name)) ?? throw new InternalErrorException("event wasn't created");

            EventDTO eventDTO = _mapper.Map<EventDTO>(createdEvent);

            return eventDTO;
        }

        public async Task DeleteEvent(int eventId)
        {
            Event? foundEvent = (await _eventRepository.GetEventById(eventId)) ?? throw new NotFoundException("Event not found");

            await _eventRepository.DeleteEvent(eventId); 
        }

        public async Task<EventDTO> GetEventById(int eventId)
        {
            Event foundEvent = (await _eventRepository.GetEventById(eventId)) ?? throw new NotFoundException("Event not found");

            EventDTO eventDTO = _mapper.Map<EventDTO>(foundEvent);

            return eventDTO;
        }

        public async Task<EventDTO> GetEventByName(string name)
        {
            Event foundEvent = (await _eventRepository.GetEventByName(name)) ?? throw new NotFoundException("Event not found");

            EventDTO eventDTO = _mapper.Map<EventDTO>(foundEvent);

            return eventDTO;
        }
           

        public async Task<PaginatedList<EventDTO>> GetEventsFiltered(EventFilterDTO filterDTO, int pageIndex, int pageSize)
        {
            Event filter = _mapper.Map<Event>(filterDTO);

            var res = await _eventRepository.GetEventsFiltered(filter, pageIndex, pageSize);
            if (res == null) throw new NotFoundException("Events not found");

            var dtos = res.items?.Select(_mapper.Map<EventDTO>).ToList();

            return new PaginatedList<EventDTO>(dtos, pageIndex, res.TotalPages);
        }

        public async Task<PaginatedList<EventDTO>> GetEvents(int pageIndex, int PageSize)
        {
            var res = (await _eventRepository.GetEvents(pageIndex, PageSize)) ?? throw new NotFoundException("Event not found");

            if (res == null) throw new NotFoundException("Events not found");

            var dtos = res.items?.Select(_mapper.Map<EventDTO>).ToList();

            return new PaginatedList<EventDTO>(dtos, pageIndex, res.TotalPages);
        }
           
        public async Task<EventDTO> UpdateEvent(UpdateEventDTO updatedEventDTO)
        {
            Event updatedEvent = _mapper.Map<Event>(updatedEventDTO);

            _eventValidator.ValidateAndThrow(updatedEvent);
            if (updatedEvent.Id == null) throw new BadRequestException("Event id wan't sent");

            Event? foundEvent = await _eventRepository.GetEventById(updatedEvent.Id);
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
                int ParticipantsCount = await _eventRepository.GetEventParticipantsCount(updatedEvent.Id);
                if (updatedEvent.MaxParticipantsCount < ParticipantsCount)
                    throw new BadRequestException("You couldn't set max participant count less than real participants count");
                if (updatedEvent.MaxParticipantsCount == ParticipantsCount)
                    foundEvent.IsFull = true;
                if (foundEvent.IsFull && updatedEvent.MaxParticipantsCount > ParticipantsCount)
                    foundEvent.IsFull = false;
                foundEvent.MaxParticipantsCount = updatedEvent.MaxParticipantsCount; 
            }

            Event? resEvent = (await _eventRepository.UpdateEvent(foundEvent)) ?? throw new InternalErrorException("event wasn't updated");

            EventDTO resEventDTO = _mapper.Map<EventDTO>(resEvent);

            return resEventDTO;
        }
        public async Task RemoveImageFromEvent(int eventId)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.ImagePath == null) throw new BadRequestException("Event doesn't have image");

            _imageService.DeleteImage(foundEvent.ImagePath);

            foundEvent.ImagePath = null;

            await _eventRepository.UpdateEvent(foundEvent);
        }
        public async Task AddImageToEvent(int eventId, IFormFile imageFile)
        {
            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new NotFoundException("Event not found");

            foundEvent.ImagePath = await _imageService.SaveImageAsync(imageFile);

            await _eventRepository.UpdateEvent(foundEvent);
        }

        public async Task<(FileStream, string)> GetImage(int eventId)
        {

            Event? foundEvent = await _eventRepository.GetEventById(eventId);
            if (foundEvent == null) throw new NotFoundException("Event not found");
            if (foundEvent.ImagePath == null) throw new BadRequestException("Event doesn't have image");

            string ext;
            FileStream imageFile;
         
            (imageFile,ext) = _imageService.GetImageStream(foundEvent.ImagePath);
            if (ext == null || imageFile == null) throw new InternalErrorException("Couldn't open filestream or get file extention");
            
            ext = $"image/{ext.TrimStart('.').ToLower()}";
            
            return (imageFile,ext);
        }


        public async Task<List<EventDTO>> GetUserEvents(string userId)
        {
            List<Event> foundEvents = await _eventRepository.GetUserEvents(userId);

            if (foundEvents == null || foundEvents.Count <= 0) return [];

            List<EventDTO> foundEventsDTOs = foundEvents.Select(e=>_mapper.Map<EventDTO>(e)).ToList();

            return foundEventsDTOs;
        }
    }
}
