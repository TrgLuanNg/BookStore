using BookStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly BookStoreContext _context;
        public DashboardController(BookStoreContext context) => _context = context;

        public IActionResult Index() => View();

        // API lấy số liệu thống kê
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            // Status 2 giả định là đơn hàng thành công
            var income = await _context.Orders.Where(o => o.StatusId == 2).SumAsync(o => o.TotalPrice);
            var orders = await _context.Orders.CountAsync();
            var products = await _context.Books.CountAsync();
            var users = await _context.Accounts.CountAsync();

            return Json(new { totalIncome = income, totalOrders = orders, totalProducts = products, totalAccounts = users });
        }
    }
}