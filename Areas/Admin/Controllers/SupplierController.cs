using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class SupplierController : Controller
	{
		private readonly BookStoreContext _context;
		public SupplierController(BookStoreContext context) => _context = context;

		public async Task<IActionResult> Index() => View(await _context.Suppliers.ToListAsync());

		[HttpPost]
		public async Task<IActionResult> Create(Supplier supplier)
		{
			_context.Suppliers.Add(supplier);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Supplier supplier)
		{
			_context.Suppliers.Update(supplier);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await _context.Suppliers.FindAsync(id);
			if (item != null)
			{
				_context.Suppliers.Remove(item);
				await _context.SaveChangesAsync();
				return Json(new { success = true });
			}
			return Json(new { success = false });
		}
	}
}