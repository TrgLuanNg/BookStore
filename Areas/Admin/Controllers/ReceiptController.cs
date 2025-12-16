using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ReceiptController : Controller
    {
        private readonly BookStoreContext _context;
        public ReceiptController(BookStoreContext context) => _context = context;

        // 1. Danh sách lịch sử nhập hàng (Action Index)
        public async Task<IActionResult> Index()
        {
            var list = await _context.GoodsReceipts
                                     .OrderByDescending(x => x.DateCreate)
                                     .ToListAsync();
            return View(list);
        }

        // 2. Giao diện trang Tạo phiếu nhập (Action Create - GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách NCC để hiển thị trong dropdown
            ViewBag.Suppliers = _context.Suppliers.ToList();
            return View(); // Trả về View Create.cshtml bạn đã tạo
        }

        // 3. Xử lý lưu phiếu nhập (Action Create - POST)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReceiptCreateRequest model)
        {
            using var trans = _context.Database.BeginTransaction();
            try
            {
                if (model.Details == null || !model.Details.Any())
                    return Json(new { success = false, message = "Chưa chọn sách nào!" });

                // A. Tạo phiếu nhập (Master)
                var receipt = new GoodsReceipt
                {
                    StaffId = User.Identity?.Name ?? "admin",
                    DateCreate = DateTime.Now,
                    TotalPrice = model.Details.Sum(x => x.Quantity * x.Price)
                };
                _context.GoodsReceipts.Add(receipt);
                await _context.SaveChangesAsync(); // Lưu để lấy ID

                // B. Lưu chi tiết & Cộng kho (Detail)
                foreach (var item in model.Details)
                {
                    // Lưu bảng chi tiết
                    _context.GoodsReceiptDetails.Add(new GoodsReceiptDetail
                    {
                        GoodsReceiptId = receipt.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        InputPrice = item.Price
                    });

                    // Cập nhật số lượng tồn kho trong bảng Book
                    var book = await _context.Books.FindAsync(item.ProductId);
                    if (book != null)
                    {
                        book.Quantity += item.Quantity;
                    }
                }

                await _context.SaveChangesAsync();
                await trans.CommitAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Class phụ để hứng dữ liệu JSON từ Ajax gửi lên
    public class ReceiptCreateRequest
    {
        public int SupplierId { get; set; }
        public string Notes { get; set; }
        public List<ReceiptDTO> Details { get; set; }
    }
}