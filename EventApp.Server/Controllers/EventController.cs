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
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [Route("get-all/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginatedList<EventDTO>>> GetEvents(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var res = await _eventService.GetEvents(pageIndex, pageSize, cancellationToken);

            return Ok(res);
        }

        [HttpGet]
        [Route("get-filtered/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginatedList<EventDTO>>> GetEvents([FromQuery]EventFilterDTO filterDTO, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var res = await _eventService.GetEventsFiltered(filterDTO,pageIndex, pageSize, cancellationToken);

            return Ok(res);
        }

        [HttpGet]
        [Route("{eventId}")]
        public async Task<ActionResult<EventDTO>> GetEventById(int eventId, CancellationToken cancellationToken)
        {
            EventDTO res = await _eventService.GetEventById(eventId, cancellationToken);

            return Ok(res);
        }

        [HttpGet]
        [Route("name-{eventName}")]
        public async Task<ActionResult<EventDTO>> GetEventByName(string eventName, CancellationToken cancellationToken)
        {
            EventDTO res = await _eventService.GetEventByName(eventName, cancellationToken);

            return Ok(res);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("add")]
        public async Task<ActionResult<EventDTO>> AddEvent([FromForm] CreateEventDTO newEventDTO, CancellationToken cancellationToken)
        {
            EventDTO res = await _eventService.CreateEvent(newEventDTO, cancellationToken);

            return Ok(res);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("update")]
        public async Task<ActionResult<EventDTO>> UpdateEvent([FromForm] UpdateEventDTO updatedEventDTO, CancellationToken cancellationToken)
        {
            EventDTO res = await _eventService.UpdateEvent(updatedEventDTO, cancellationToken);

            return Ok(res);
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}")]
        public async Task<ActionResult> DeleteEvent(int eventId, CancellationToken cancellationToken)
        {
            await _eventService.RemoveImageFromEvent(eventId, cancellationToken);
            await _eventService.DeleteEvent(eventId, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}/image")]
        public async Task<ActionResult> AddImageToEvent([FromForm] ImageDTO imageDTO, int eventId, CancellationToken cancellationToken)
        {
            await _eventService.AddImageToEvent(eventId, imageDTO.imageFile, cancellationToken);

            return Ok();
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        [Route("{eventId}/image")]
        public async Task<ActionResult> RemoveImageFromEvent(int eventId, CancellationToken cancellationToken)
        {
            await _eventService.RemoveImageFromEvent(eventId, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("{eventId}/image")]
        public async Task<ActionResult> GetEventImage(int eventId, CancellationToken cancellationToken)
        {
            string cType;
            FileStream imageFile;

            (imageFile, cType) = await _eventService.GetImage(eventId, cancellationToken);

            return File(imageFile, cType);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "UserPolicy")]
        [Route("get-user-events/{userId}")]
        public async Task<ActionResult<List<EventDTO>>> GetUserEvents(string userId, CancellationToken cancellationToken)
        {
            var res = await _eventService.GetUserEvents(userId, cancellationToken);

            return Ok(res);
        }
    }
}
