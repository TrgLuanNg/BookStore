using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class DiscountController : Controller
	{
		private readonly BookStoreContext _context;
		public DiscountController(BookStoreContext context) => _context = context;

		public async Task<IActionResult> Index()
		{
			return View(await _context.Discounts.ToListAsync());
		}

		[HttpPost]
		public async Task<IActionResult> Create(Discount discount)
		{
			if (await _context.Discounts.AnyAsync(d => d.Code == discount.Code))
				return Json(new { success = false, message = "Mã gi?m giá ?ã t?n t?i" });

			_context.Discounts.Add(discount);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Discount discount)
		{
			_context.Discounts.Update(discount);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id) // id ? ?ây là discount_code (string)
		{
			var d = await _context.Discounts.FindAsync(id);
			if (d == null) return Json(new { success = false });

			_context.Discounts.Remove(d);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}
	}
}