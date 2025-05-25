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
    [Route("EventApi")]
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
        [Route("/event/get-all/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginatedList<Event>>> GetEvents(int pageIndex, int pageSize)
        {
            var res = await _eventService.GetEvents(pageIndex, pageSize);

            return Ok(res);
        }

        [HttpGet]
        [Route("/event/get-filtered")]
        public async Task<ActionResult<PaginatedList<Event>>> GetEvents([FromQuery]EventFilterDTO filterDTO, int pageIndex, int pageSize)
        {
            var res = await _eventService.GetEventsFiltered(filterDTO,pageIndex, pageSize);

            return Ok(res);
        }

        [HttpGet]
        [Route("/event/{eventId}")]
        public async Task<ActionResult<Event>> GetEventById(int eventId)
        {
            Event res = await _eventService.GetEventById(eventId);

            return Ok(res);
        }

        [HttpGet]
        [Route("/event/name-{eventName}")]
        public async Task<ActionResult<Event>> GetEventByName(string eventName)
        {
            Event res = await _eventService.GetEventByName(eventName);

            return Ok(res);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("/event/add")]
        public async Task<ActionResult<Event>> AddEvent(Event newEvent)
        {
            Event res = await _eventService.CreateEvent(newEvent);

            return Ok(res);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("/event/update")]
        public async Task<ActionResult<Event>> UpdateEvent(Event updatedEvent)
        {
            Event res = await _eventService.UpdateEvent(updatedEvent);

            return Ok(res);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("/event/{eventId}")]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            await _eventService.DeleteEvent(eventId);

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("event/{eventId}/image")]
        public async Task<ActionResult> AddImageToEvent([FromForm] ImageDTO imageDTO, int eventId)
        {
            await _eventService.AddImageToEvent(eventId, imageDTO.imageFile);

            return Ok();
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("event/{eventId}/image")]
        public async Task<ActionResult> RemoveImageFromEvent(int eventId)
        {
            await _eventService.RemoveImageFromEvent(eventId);

            return Ok();
        }

        [HttpGet]
        [Route("event/{eventId}/image")]
        public async Task<ActionResult> GetEventImage(int eventId)
        {
            string cType;
            FileStream imageFile;

            (imageFile, cType) = await _eventService.GetImage(eventId);

            return File(imageFile, cType);
        }
    }
}
