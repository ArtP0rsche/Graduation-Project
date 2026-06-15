using CourseProject.Data;
using CourseProject.DataModels;
using CourseProject.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CourseProject.Controllers
{
    public class RequestsController : Controller
    {
        private readonly EmploymentServiceContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RequestsController(EmploymentServiceContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var employmentServiceContext = _context.Requests.Include(r => r.Event).Include(r => r.User);
            ViewBag.StatusOptions = new List<string> { "На рассмотрении", "Отклонена", "Принята" };
            return View(await employmentServiceContext.ToListAsync());
        }

        // GET: Requests/Create
        public IActionResult Create(int? eventId, int? userId)
        {
            if (eventId == null || userId == null)
            {
                return BadRequest();
            }

            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int eventId, int userId, [Bind("RequestId,UserId,EventId,Content,Status,UpdatedOn,Institution,PeopleNumber")] Request request)
        {
            if (request.EventId != null && request.UserId != null)
            {
                try
                {
                    var targetEvent = await _context.Events.FindAsync(request.EventId);
                    if (targetEvent == null)
                    {
                        return Json(new { success = false, message = "Мероприятие не найдено" });
                    }
                    if (targetEvent.Status == "Отменено")
                    {
                        return Json(new { success = false, message = "Ошибка: Мероприятие отменено!" });
                    }
                    if (targetEvent.AvailableSpace < request.PeopleNumber)
                    {
                        return Json(new { success = false, message = $"Недостаточно мест! Доступно только: {targetEvent.AvailableSpace}" });
                    }

                    request.UpdatedOn = DateOnly.FromDateTime(DateTime.Now);
                    _context.Add(request);

                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Заявка успешно создана" });
                }
                catch (DbUpdateException ex)
                {
                    var innerMessage = ex.InnerException?.Message ?? "";

                    if (innerMessage.Contains("Недостаточно") || innerMessage.Contains("отменено"))
                    {
                        return Json(new { success = false, message = "Ошибка базы данных: мест больше нет или статус изменился." });
                    }

                    return Json(new { success = false, message = "Произошла ошибка при сохранении данных в БД." });
                }
            }

            return Json(new { success = false, message = "Ошибка валидации данных" });
        }

        [HttpPost, ActionName("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var request = _context.Requests.Include(r => r.Event).FirstOrDefault(r => r.RequestId == id);

            request.Status = status;
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Update(request);

            var notification = new Notification
            {
                UserId = request.UserId,
                Message = $"Статус вашей заявки по мероприятию \"{request.Event.Title}\" изменен на: \"{status}\".",
                IsRead = false
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(request.UserId.ToString()).SendAsync("ReceiveNotificationObj", new
            {
                id = notification.NotificationId,
                message = notification.Message,
                isRead = notification.IsRead
            });

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
            Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Ok();
            }

            return RedirectToAction("index", "Requests");
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,UserId,EventId,Content,Status,Institution,PeopleNumber")] Request request)
        {
            if (id != request.RequestId)
            {
                return Json(new { success = false, message = "Заявка не найдена." });
            }

            try
            {
                request.UpdatedOn = DateOnly.FromDateTime(DateTime.Now);
                _context.Update(request);
                await _context.SaveChangesAsync();

                var currentEvent = await _context.Events.FindAsync(request.EventId);
                string eventTitle = currentEvent?.Title ?? "выбранное мероприятие";

                TempData["SuccessMessage"] = $"Заявка на мероприятие \"{eventTitle}\" успешно обновлена!";
                return Json(new { success = true, message = "Заявка успешно обновлена!" });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is MySqlException mySqlException)
                {
                    return Json(new { success = false, message = mySqlException.Message });
                }

                return Json(new { success = false, message = "Ошибка при обновлении базы данных." });
            }
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Users", new { id = Convert.ToInt32(HttpContext.Session.GetString("UserId")) });
        }
    }
}
