using BookStore.Data;
using BookStore.Models; // Cần tạo Model GoodsReceipt và GoodsReceiptDetail
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    public class ReceiptDTO { public int ProductId; public int Quantity; public double Price; }

    [Area("Admin")]
    public class ReceiptController : Controller
    {
        private readonly BookStoreContext _context;
        public ReceiptController(BookStoreContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> Create(int supplierId, List<ReceiptDTO> details)
        {
            using var trans = _context.Database.BeginTransaction();
            try
            {
                // 1. Tạo phiếu nhập
                var receipt = new GoodsReceipt
                {
                    StaffId = User.Identity.Name ?? "admin",
                    DateCreate = DateTime.Now,
                    TotalPrice = details.Sum(x => x.Quantity * x.Price)
                };
                _context.GoodsReceipts.Add(receipt);
                await _context.SaveChangesAsync();

                // 2. Lưu chi tiết & cập nhật kho
                foreach (var item in details)
                {
                    _context.GoodsReceiptDetails.Add(new GoodsReceiptDetail
                    {
                        GoodsReceiptId = receipt.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        InputPrice = item.Price
                    });

                    // CỘNG KHO
                    var product = await _context.Books.FindAsync(item.ProductId);
                    if (product != null) product.Quantity += item.Quantity;
                }

                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return Json(new { success = true });
            }
            catch
            {
                await trans.RollbackAsync();
                return Json(new { success = false });
            }
        }
    }
}