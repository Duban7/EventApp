using AutoFixture;
using AutoMapper;
using Data.Interfaces;
using Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Services.DTOs;
using Services.Interfaces;
using Services.Mapper;
using Services.Services;
using Services.Validators;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tests
{
    public class EventUseCasesTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private EventService _eventService;
        private readonly IMapper _mapper;
        private readonly IValidator<Event> _validator;
        private readonly Mock<IImageService> _imageService;
        private readonly IFixture _fixture;

        public EventUseCasesTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mapper = (new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>())).CreateMapper();
            _validator = new EventValidator();
            _imageService = new Mock<IImageService>();
            _fixture = new Fixture();
        }
        [Fact]
        public async Task GetAllEventsUseCase() 
        {
            //Arrange
            var events = new PaginatedList<Event>
                (new List<Event>
                {
                    _fixture.Build<Event>().Without(e => e.Participants)
                    .Without(e => e.EventParticipations).Create(),
                    _fixture.Build<Event>().Without(e => e.Participants)
                    .Without(e => e.EventParticipations).Create()
                }, 1,0 );
            _mockEventRepository.Setup(r=>r.GetEvents(1,10).Result).Returns(events);
            _eventService = new EventService(_mockEventRepository.Object,_validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.GetEvents(1,10);

            //Assert
            Assert.Equal(res.items.Count,2);
        }
        [Fact]
        public async void GetFiltredEventsUseCase()
        {
            //Arrange
            var events = new PaginatedList<Event>
                (new List<Event>
                {
                    _fixture.Build<Event>().Without(e => e.Participants)
                    .Without(e => e.EventParticipations).Create(),
                    _fixture.Build<Event>().Without(e => e.Participants)
                    .Without(e => e.EventParticipations).Create()
                }, 1, 0);
            _mockEventRepository.Setup(r => r.GetEventsFiltered(null,1, 10).Result).Returns(events);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.GetEventsFiltered(null,1, 10);

            //Assert
            Assert.Equal(res.items.Count, 2);
        }
        [Fact]
        public async void GetEventByIdUseCase()
        {
            //Arrange
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e => e.Id, 1).Create();
                   
            _mockEventRepository.Setup(r => r.GetEventById(singleEvent.Id).Result).Returns(singleEvent);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.GetEventById(1);

            //Assert
            Assert.NotNull(res);
            Assert.Equal(res.Name, singleEvent.Name);
            Assert.Equal(res.Description, singleEvent.Description);
            Assert.Equal(res.EventPlace, singleEvent.EventPlace);
            Assert.Equal(res.Category, singleEvent.Category);
        }
        [Fact]
        public async void GetEventByNameUseCase()
        {
            //Arrange
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e => e.Id, 1).Create();

            _mockEventRepository.Setup(r => r.GetEventByName(singleEvent.Name).Result).Returns(singleEvent);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.GetEventByName(singleEvent.Name);

            //Assert
            Assert.NotNull(res);
            Assert.Equal(res.Name, singleEvent.Name);
            Assert.Equal(res.Description, singleEvent.Description);
            Assert.Equal(res.EventPlace, singleEvent.EventPlace);
            Assert.Equal(res.Category, singleEvent.Category);
        }
        //Repo unit test
        [Fact]
        public async void CreateEventUseCase()
        {
            //Arrange
            var EventDTO = _fixture.Build<CreateEventDTO>()
                .With(e => e.Name, "eventName")
                .With(e => e.StartDate, DateTime.Now.AddDays(1))
                .With(e => e.MaxParticipantsCount, 4).Create();
            var singleEvent = _mapper.Map<Event>(EventDTO);

            _mockEventRepository.SetupSequence(r => r.GetEventByName(singleEvent.Name))
                .ReturnsAsync((Event?)null)
                .ReturnsAsync(singleEvent);
            _mockEventRepository.Setup(r => r.CreateEvent(singleEvent)).Returns(Task.CompletedTask);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.CreateEvent(EventDTO);

            //Assert
            Assert.NotNull(res);
            Assert.Equal(res.StartDate, EventDTO.StartDate);
            Assert.Equal(res.Description, EventDTO.Description);
            Assert.Equal(res.EventPlace, EventDTO.EventPlace);
            Assert.Equal(res.Category, EventDTO.Category);
            Assert.Equal(res.MaxParticipantsCount, EventDTO.MaxParticipantsCount);
            _mockEventRepository.Verify(r => r.CreateEvent(It.IsAny<Event>()), Times.Once);
        }
        [Fact]
        public async void UpdateEventUseCase()
        {
            //Arrange
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e=>e.Name,"eventName")
                    .With(e => e.Id, 1)
                    .Create();
            var EventDTO = _fixture.Build<UpdateEventDTO>()
                    .With(e=>e.StartDate, DateTime.Now.AddDays(1))
                    .With(e=>e.MaxParticipantsCount, 4)
                    .With(e => e.Id, 1).Create();

            _mockEventRepository.Setup(r => r.GetEventById(singleEvent.Id)).ReturnsAsync(singleEvent);
            _mockEventRepository.Setup(r => r.GetEventParticipantsCount(singleEvent.Id).Result).Returns(2);
            _mockEventRepository.Setup(r => r.UpdateEvent(singleEvent).Result).Returns(singleEvent);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            var res = await _eventService.UpdateEvent(EventDTO);

            //Assert
            Assert.NotNull(res);
            Assert.Equal(res.StartDate, EventDTO.StartDate);
            Assert.Equal(res.Description, EventDTO.Description);
            Assert.Equal(res.EventPlace, EventDTO.EventPlace);
            Assert.Equal(res.Category, EventDTO.Category);
            Assert.Equal(res.MaxParticipantsCount, EventDTO.MaxParticipantsCount);
        }
        [Fact]
        public async void DeleteEventUseCase()
        {
            //Arrange
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e => e.Id, 1).Create();

            _mockEventRepository.Setup(r => r.GetEventById(singleEvent.Id)).ReturnsAsync(singleEvent);
            _mockEventRepository.Setup(r => r.DeleteEvent(singleEvent.Id)).Returns(Task.CompletedTask);
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            await _eventService.DeleteEvent(1);

            //Assert
            _mockEventRepository.Verify(mrp => mrp.DeleteEvent(1), Times.Once());
            _mockEventRepository.Verify(mrp => mrp.GetEventById(1), Times.Once());
        }
        [Fact]
        public async void AddImageToEventUseCase()
        {
            //Arrange
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e => e.Id, 1).Create();
            IFormFile formFile = null;
            _mockEventRepository.Setup(r => r.GetEventById(singleEvent.Id).Result).Returns(singleEvent);
            _mockEventRepository.Setup(r => r.UpdateEvent(singleEvent).Result).Returns(singleEvent);
            _imageService.Setup(i => i.SaveImageAsync(formFile).Result).Returns("imagePath.png");
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            await _eventService.AddImageToEvent(singleEvent.Id, formFile);

            //Assert
            _mockEventRepository.Verify(mrp => mrp.GetEventById(singleEvent.Id), Times.Once());
            _mockEventRepository.Verify(mrp => mrp.UpdateEvent(singleEvent), Times.Once());
            _imageService.Verify(imgs=>imgs.SaveImageAsync(formFile), Times.Once());
        }
        [Fact]
        public async void RemoveImageFromEventUseCase()
        {
            //Arrange
            string imagePahvalue = "imagepath.png";
            var singleEvent = _fixture.Build<Event>()
                    .Without(e => e.Participants)
                    .Without(e => e.EventParticipations)
                    .With(e => e.ImagePath, imagePahvalue)
                    .With(e => e.Id, 2).Create();

            _mockEventRepository.Setup(r => r.GetEventById(singleEvent.Id).Result).Returns(singleEvent);
            _mockEventRepository.Setup(r => r.UpdateEvent(singleEvent).Result).Returns(singleEvent);
            _imageService.Setup(i => i.DeleteImage(singleEvent.ImagePath));
            _eventService = new EventService(_mockEventRepository.Object, _validator, _mapper, _imageService.Object);

            //Act
            await _eventService.RemoveImageFromEvent(singleEvent.Id);

            //Assert
            _mockEventRepository.Verify(mrp => mrp.GetEventById(singleEvent.Id), Times.Once());
            _mockEventRepository.Verify(mrp => mrp.UpdateEvent(singleEvent), Times.Once());
            _imageService.Verify(imgs => imgs.DeleteImage(imagePahvalue), Times.Once());
        }
    }
}
