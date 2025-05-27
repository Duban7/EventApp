using AutoFixture;
using Data.Context;
using Data.Models;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class EventRepositoryTests
    {

        private readonly Fixture _fixture;
        private readonly DbContextOptions<EventAppDbContext> _dbOptions;
        public EventRepositoryTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _dbOptions = new DbContextOptionsBuilder<EventAppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        [Fact]
        public async Task GetOneByIdTest()
        {
            // Arrange
            using var context = new EventAppDbContext(_dbOptions);
            var repository = new EventRepository(context);

            var expectedEvent = _fixture.Build<Event>()
                .Without(e => e.Participants)
                .Create();

            context.Events.Add(expectedEvent);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetEventById(expectedEvent.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEvent.Id, result.Id);
            Assert.Equal(expectedEvent.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEvent_WhenExists()
        {
            // Arrange
            using var context = new EventAppDbContext(_dbOptions);
            var repository = new EventRepository(context);

            var newEvent = _fixture.Build<Event>()
                .Without(e => e.Participants)
                .Without(e => e.Id) 
                .Create();

            // Act
            await repository.CreateEvent(newEvent);
            await context.SaveChangesAsync();

            // Assert
            var result = await context.Events.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(newEvent.Name, result.Name);
            Assert.True(result.Id > 0); 
        }
    }
}
