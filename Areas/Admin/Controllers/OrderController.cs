using BookStore.Data;
using BookStore.Models; // Cần namespace này để dùng OrderViewModel
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Cần để dùng LINQ
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly BookStoreContext _context;
        public OrderController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // Join bảng Order và DeliveryInfo để lấy tên khách, sđt
            var list = await (from o in _context.Orders
                              join d in _context.DeliveryInfos on o.DeliveryInfoId equals d.Id
                              orderby o.DateCreate descending
                              select new OrderViewModel
                              {
                                  Id = o.Id,
                                  FullName = d.FullName,
                                  Phone = d.PhoneNumber,
                                  OrderDate = o.DateCreate,
                                  TotalAmount = o.TotalPrice,
                                  Status = o.StatusId
                              }).ToListAsync();

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = status; // Cập nhật StatusId

                // Nếu duyệt đơn (status = 2), cần trừ tồn kho (Logic phức tạp, tạm thời chỉ update status)
                // Bạn có thể thêm logic trừ kho ở đây nếu muốn

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
        }

        // API Duyệt đơn (Approve) riêng biệt nếu dùng nút Duyệt
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return Json(new { success = false, message = "Đơn hàng không tồn tại" });

            // 1. Kiểm tra tồn kho trước khi duyệt
            var details = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync();
            foreach (var item in details)
            {
                var book = await _context.Books.FindAsync(item.ProductId);
                if (book == null || book.Quantity < item.Quantity)
                    return Json(new { success = false, message = $"Sách ID {item.ProductId} không đủ tồn kho!" });
            }

            // 2. Trừ kho
            foreach (var item in details)
            {
                var book = await _context.Books.FindAsync(item.ProductId);
                if (book != null) book.Quantity -= item.Quantity;
            }

            // 3. Cập nhật trạng thái
            order.StatusId = 2; // 2 = Đã duyệt
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.StatusId = 3; // 3 = Hủy
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            // Trả về JSON để hiển thị trong Modal hoặc Partial View
            var details = await (from od in _context.OrderDetails
                                 join p in _context.Books on od.ProductId equals p.Id
                                 where od.OrderId == id
                                 select new
                                 {
                                     p.Name,
                                     p.ImagePath,
                                     od.Quantity,
                                     od.Price
                                 }).ToListAsync();

            // Trả về chuỗi HTML đơn giản để hiển thị nhanh trong Modal (hoặc trả về JSON rồi JS render)
            return Json(details);
        }
    }
}