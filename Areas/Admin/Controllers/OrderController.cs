using BookStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly BookStoreContext _context;
        public OrderController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var list = await _context.Orders.OrderByDescending(o => o.DateCreate).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = status;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // Lấy chi tiết đơn hàng để hiện Modal
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var details = await (from od in _context.OrderDetails
                                 join p in _context.Books on od.ProductId equals p.Id
                                 where od.OrderId == id
                                 select new { p.Name, p.ImagePath, od.Quantity, od.Price })
                                 .ToListAsync();
            return Json(details);
        }
    }
}