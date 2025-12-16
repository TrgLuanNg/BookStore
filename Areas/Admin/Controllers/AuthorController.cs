using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly BookStoreContext _context;
        public AuthorController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index() => View(await _context.Authors.ToListAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Author author)
        {
            if (await _context.Authors.AnyAsync(x => x.Name == author.Name))
                return Json(new { success = false, message = "Tên đã tồn tại" });

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Authors.FindAsync(id);
            if (item == null) return Json(new { success = false });

            _context.Authors.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}