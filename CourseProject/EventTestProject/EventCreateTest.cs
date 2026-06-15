using CourseProject.Controllers;
using CourseProject.Data;
using CourseProject.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventTestProject
{
    public class EventCreateTest
    {
        private readonly EmploymentServiceContext _context;
        private readonly EventsController _controller;

        public EventCreateTest()
        {
            var options = new DbContextOptionsBuilder<EmploymentServiceContext>()
                .UseMySql("Server=localhost;User ID=root;Password=root;Database=russian_work",
                    ServerVersion.Parse("8.0.44-mysql"))
                .Options;

            _context = new EmploymentServiceContext(options);
            _controller = new EventsController(_context);
        }

        [Fact]
        public async Task Create_ValidModel_SavesToDb_And_RedirectsToIndex()
        {
            // Подготовка тестовых данных
            var eventToCreate = new Event
            {
                Title = "Test Event",
                Description = "Test description",
                AvailableSpace = 25,
                EventDate = DateTime.Now.AddDays(7)
            };

            // Вызываем метод контроллера
            var result = await _controller.Create(eventToCreate, null);

            // Проверяем редирект
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            // Проверяем сохранение в БД
            var savedEvent = await _context.Events.FirstOrDefaultAsync(e => e.Title == "Test Event");
            Assert.NotNull(savedEvent);
            Assert.Equal(eventToCreate.Title, savedEvent.Title);
            Assert.Equal(eventToCreate.AvailableSpace, savedEvent.AvailableSpace);
            Assert.Equal(DateOnly.FromDateTime(DateTime.Now), savedEvent.UpdatedOn);
        }
    }
}