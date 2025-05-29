using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs;
using Services.Interfaces;

namespace EventApp.Controllers
{
    [ApiController]
    [Route("EventApi/event")]
    public class EventContorller : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventContorller> _logger;
        public EventContorller(IEventService eventService,
                               ILogger<EventContorller> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        [Route("get-all/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginatedList<EventDTO>>> GetEvents(int pageIndex, int pageSize)
        {
            var res = await _eventService.GetEvents(pageIndex, pageSize);

            return Ok(res);
        }

        [HttpGet]
        [Route("get-filtered/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginatedList<EventDTO>>> GetEvents([FromQuery]EventFilterDTO filterDTO, int pageIndex, int pageSize)
        {
            var res = await _eventService.GetEventsFiltered(filterDTO,pageIndex, pageSize);

            return Ok(res);
        }

        [HttpGet]
        [Route("{eventId}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int eventId)
        {
            EventDTO res = await _eventService.GetEventById(eventId);

            return Ok(res);
        }

        [HttpGet]
        [Route("name-{eventName}")]
        public async Task<ActionResult<EventDTO>> GetEventByName(string eventName)
        {
            EventDTO res = await _eventService.GetEventByName(eventName);

            return Ok(res);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("add")]
        public async Task<ActionResult<EventDTO>> AddEvent([FromForm] CreateEventDTO newEventDTO)
        {
            EventDTO res = await _eventService.CreateEvent(newEventDTO);

            return Ok(res);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("update")]
        public async Task<ActionResult<EventDTO>> UpdateEvent([FromForm] UpdateEventDTO updatedEventDTO)
        {
            EventDTO res = await _eventService.UpdateEvent(updatedEventDTO);

            return Ok(res);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}")]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            await _eventService.RemoveImageFromEvent(eventId);
            await _eventService.DeleteEvent(eventId);

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}/image")]
        public async Task<ActionResult> AddImageToEvent([FromForm] ImageDTO imageDTO, int eventId)
        {
            await _eventService.AddImageToEvent(eventId, imageDTO.imageFile);

            return Ok();
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}/image")]
        public async Task<ActionResult> RemoveImageFromEvent(int eventId)
        {
            await _eventService.RemoveImageFromEvent(eventId);

            return Ok();
        }

        [HttpGet]
        [Route("{eventId}/image")]
        public async Task<ActionResult> GetEventImage(int eventId)
        {
            string cType;
            FileStream imageFile;

            (imageFile, cType) = await _eventService.GetImage(eventId);

            return File(imageFile, cType);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("get-user-events/{userId}")]
        public async Task<ActionResult<List<EventDTO>>> GetUserEvents(string userId)
        {
            var res = await _eventService.GetUserEvents(userId);

            return Ok(res);
        }
    }
}
