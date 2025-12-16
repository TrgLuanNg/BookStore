using BookStore.Data;
using BookStore.Models; // Chứa class Book
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly BookStoreContext _context;
        public ProductController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // Load danh sách sản phẩm (có thể thêm .Include(x => x.Publisher) nếu cần hiện tên NXB)
            return View(await _context.Books.OrderByDescending(x => x.Id).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            // Trong thực tế cần xử lý thêm categories/authors (mối quan hệ nhiều-nhiều)
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Thêm sản phẩm thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Books.FindAsync(id);
            if (item == null) return Json(new { success = false });

            _context.Books.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // API lấy dữ liệu để đổ vào Dropdown (Select box) khi thêm/sửa
        public IActionResult GetAuxData()
        {
            return Json(new
            {
                categories = _context.Categories.Select(x => new { x.Id, x.Name }).ToList(),
                authors = _context.Authors.Select(x => new { x.Id, x.Name }).ToList(),
                publishers = _context.Publishers.Select(x => new { x.Id, x.Name }).ToList()
            });
        }
    }
}