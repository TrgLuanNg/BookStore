using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class CategoryController : Controller
	{
		private readonly BookStoreContext _context;
		public CategoryController(BookStoreContext context) => _context = context;

		public async Task<IActionResult> Index()
		{
			return View(await _context.Categories.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> Create(Category category)
		{
			if (await _context.Categories.AnyAsync(x => x.Name.ToLower() == category.Name.ToLower()))
				return Json(new { success = false, message = "Tên thể loại đã tồn tại" });

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Category category)
		{
			_context.Categories.Update(category);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var cat = await _context.Categories.FindAsync(id);
			if (cat == null) return Json(new { success = false });

			// Kiểm tra ràng buộc
			bool hasBooks = await _context.CategoryDetails.AnyAsync(x => x.CategoryId == id);
			if (hasBooks) return Json(new { success = false, message = "Thể loại đang được sử dụng!" });

			_context.Categories.Remove(cat);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		// API Check trùng tên (checkNameExists)
		[HttpGet]
		public IActionResult CheckNameExists(string name, int? id)
		{
			var query = _context.Categories.AsQueryable();
			if (id.HasValue) query = query.Where(x => x.Id != id);

			bool exists = query.Any(x => x.Name.ToLower() == name.ToLower());
			return Json(new { exists });
		}
	}
}