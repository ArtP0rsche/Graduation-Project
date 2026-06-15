using CourseProject.Data;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly EmploymentServiceContext _context;

        public NotificationsController(EmploymentServiceContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
