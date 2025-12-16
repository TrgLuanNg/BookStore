using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PublisherController : Controller
    {
        private readonly BookStoreContext _context;
        public PublisherController(BookStoreContext context) => _context = context;

        // 1. Danh sách (Index)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Publishers.ToListAsync());
        }

        // 2. Thêm mới (Create)
        [HttpPost]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (await _context.Publishers.AnyAsync(x => x.Email == publisher.Email))
                return Json(new { success = false, message = "Email đã tồn tại!" });

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Thêm thành công" });
        }

        // 3. Chỉnh sửa (Edit)
        [HttpPost]
        public async Task<IActionResult> Edit(Publisher publisher)
        {
            _context.Publishers.Update(publisher);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        // 4. Xóa (Delete)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var pub = await _context.Publishers.FindAsync(id);
            if (pub == null) return Json(new { success = false, message = "Không tìm thấy" });

            // Kiểm tra ràng buộc (nếu NXB này đã có sách thì không cho xóa)
            bool hasBooks = await _context.Books.AnyAsync(b => b.PublisherId == id);
            if (hasBooks) return Json(new { success = false, message = "NXB này đang có sách, không thể xóa!" });

            _context.Publishers.Remove(pub);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // 5. Kiểm tra trùng (CheckEmailAndNameExists)
        [HttpPost]
        public IActionResult CheckExistence(string email, string name)
        {
            bool emailExists = _context.Publishers.Any(x => x.Email == email);
            bool nameExists = _context.Publishers.Any(x => x.Name.ToLower() == name.ToLower());

            return Json(new { emailExists, nameExists });
        }
    }
}