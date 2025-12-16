using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AccountController : Controller
	{
		private readonly BookStoreContext _context;
		public AccountController(BookStoreContext context) => _context = context;

		public async Task<IActionResult> Index() => View(await _context.Accounts.ToListAsync());

		[HttpPost]
		public async Task<IActionResult> Create(Account account)
		{
			// Logic hash password nên đặt ở đây
			_context.Accounts.Add(account);
			await _context.SaveChangesAsync();
			return Json(new { success = true });
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword(string username, string newPass)
		{
			var acc = await _context.Accounts.FirstOrDefaultAsync(x => x.Username == username);
			if (acc != null)
			{
				acc.Password = newPass; // Nhớ hash password nếu cần
				await _context.SaveChangesAsync();
				return Json(new { success = true });
			}
			return Json(new { success = false });
		}
	}
}