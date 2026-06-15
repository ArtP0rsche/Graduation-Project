using CourseProject.Data;
using CourseProject.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CourseProject.Controllers
{
    public class EventsController : Controller
    {
        private readonly EmploymentServiceContext _context;

        public EventsController(EmploymentServiceContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventImage(int? photoId)
        {
            if (photoId == null)
            {
                return File("~/resources/plug.png", "image/png");
            }

            var photo = await _context.Photos.FindAsync(photoId);

            if (photo == null || photo.ImageData == null || photo.ImageData.Length == 0)
            {
                return File("~/resources/plug.png", "image/png");
            }

            string contentType = "image/jpeg";
            if (photo.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) contentType = "image/png";

            return File(photo.ImageData, contentType);
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.Include(e => e.Requests).ToListAsync());
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("", "Недопустимый формат файла. Разрешены только изображения (jpg, jpeg, png).");
                        return View(@event);
                    }

                    if (imageFile.Length > 3 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Файл слишком большой. Максимальный размер — 16 МБ.");
                        return View(@event);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);

                        var photo = new Photo
                        {
                            Title = Path.GetFileNameWithoutExtension(imageFile.FileName),
                            FileName = $"{Guid.NewGuid()}{extension}",
                            ImageData = memoryStream.ToArray()
                        };

                        @event.Photo = photo;
                    }
                }

                @event.UpdatedOn = DateOnly.FromDateTime(DateTime.Now);
                @event.Status = "В планах";

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int eventId, string title, string description, int availableSpace, DateTime eventDate, string status, IFormFile? imageFile)
        {
            var myEvent = await _context.Events.Include(e => e.Photo).FirstOrDefaultAsync(e => e.EventId == eventId);

            if (myEvent == null)
            {
                return NotFound();
            }

            var oldPhoto = myEvent.Photo;

            myEvent.Title = title;
            myEvent.Description = description;
            myEvent.AvailableSpace = Convert.ToSByte(availableSpace);
            myEvent.EventDate = eventDate;
            myEvent.Status = status;

            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(imageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Недопустимый формат файла. Разрешены только изображения (jpg, jpeg, png).");
                }

                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("Файл слишком большой. Максимальный размер — 5 МБ.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);

                    if (myEvent.Photo != null)
                    {
                        _context.Photos.Remove(myEvent.Photo);
                    }

                    var newPhoto = new Photo
                    {
                        Title = Path.GetFileNameWithoutExtension(imageFile.FileName),
                        FileName = $"{Guid.NewGuid()}{extension}",
                        ImageData = memoryStream.ToArray()
                    };

                    myEvent.Photo = newPhoto;
                }
            }

            try
            {
                _context.Events.Update(myEvent);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Изменения успешно сохранены." });
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _context.Entry(myEvent).State = EntityState.Detached;

                if (myEvent.Photo != null && myEvent.Photo != oldPhoto)
                {
                    _context.Entry(myEvent.Photo).State = EntityState.Detached;
                }

                var mysqlMessage = ex.InnerException?.Message ?? "";

                if (mysqlMessage.Contains("Нельзя установить такое количество мест"))
                {
                    return Json(new { success = false, message = "Ошибка: Нельзя установить такое количество мест! На мероприятие уже записано больше человек." });
                }

                return Json(new { success = false, message = "Произошла ошибка при обновлении данных в базе." });
            }
        }

        [HttpPost, ActionName("ExportToXlsx")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportToXlsxAsync(string selectedMonth)
        {
            if (string.IsNullOrEmpty(selectedMonth) || !DateTime.TryParse($"{selectedMonth}-01", out DateTime parsedDate))
            {
                TempData["ErrorMessage"] = "Неверно выбран месяц.";
                return RedirectToAction(nameof(Index));
            }

            // Установка лицензии для личного пользования
            ExcelPackage.License.SetNonCommercialPersonal("CProj");

            // Получение данных из БД за выбранный месяц и год
            List<Event> events = await _context.Events
                .Where(e => e.EventDate.Month == parsedDate.Month && e.EventDate.Year == parsedDate.Year)
                .ToListAsync();

            if (events.Count == 0)
            {
                TempData["ErrorMessage"] = $"Нет мероприятий за {parsedDate.ToString("MMMM yyyy")}.";
                return RedirectToAction(nameof(Index));
            }

            using (var stream = new MemoryStream())
            {
                using (var package = new ExcelPackage(stream))
                {
                    // Создание рабочего листа с названием выбранного месяца
                    var worksheet = package.Workbook.Worksheets.Add($"{parsedDate.ToString("MMMM yyyy")}");

                    // Заголовки таблицы
                    worksheet.Cells[1, 1].Value = "№ п/п"; SetStyle(worksheet, 1, 1);
                    worksheet.Cells[1, 2].Value = "Мероприятие"; SetStyle(worksheet, 1, 2);
                    worksheet.Cells[1, 3].Value = "Описание"; SetStyle(worksheet, 1, 3);
                    worksheet.Cells[1, 4].Value = "Количество мест"; SetStyle(worksheet, 1, 4);
                    worksheet.Cells[1, 5].Value = "Статус"; SetStyle(worksheet, 1, 5);
                    worksheet.Cells[1, 6].Value = "Дата проведения"; SetStyle(worksheet, 1, 6);

                    // Заполнение ячеек данными мероприятий
                    for (int i = 0; i < events.Count; i++)
                    {
                        int row = i + 2;
                        worksheet.Cells[row, 1].Value = i + 1; SetStyle(worksheet, row, 1);
                        worksheet.Cells[row, 2].Value = events[i].Title; SetStyle(worksheet, row, 2);
                        worksheet.Cells[row, 3].Value = events[i].Description;
                        worksheet.Cells[row, 3].Style.WrapText = true; SetStyle(worksheet, row, 3);
                        worksheet.Cells[row, 4].Value = events[i].AvailableSpace; SetStyle(worksheet, row, 4);
                        worksheet.Cells[row, 5].Value = events[i].Status; SetStyle(worksheet, row, 5);
                        worksheet.Cells[row, 6].Value = events[i].EventDate; SetStyle(worksheet, row, 6);
                    }

                    // Настройка стилей
                    worksheet.Cells.AutoFitColumns();
                    worksheet.Column(3).Width = 60;
                    worksheet.Column(6).Style.Numberformat.Format = "dd.MM.yyyy HH:mm:ss";

                    package.Save();
                }

                stream.Position = 0;

                string fileName = $"Отчет_{parsedDate.ToString("MMMM_yyyy")}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Отправляем файл пользователю напрямую в браузер
                return File(stream.ToArray(), contentType, fileName);
            }
        }

        public void SetStyle(ExcelWorksheet ws, int row, int column)
        {
            ws.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[row, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells[row, column].Style.Border.BorderAround(ExcelBorderStyle.Thin);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}