using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Cần thêm dòng này
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly BookStoreContext _context;
        public ProductController(BookStoreContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            // Include Publisher để hiển thị tên NXB
            var books = await _context.Books.OrderByDescending(x => x.Id).ToListAsync();
            return View(books);
        }

        // --- 1. API Lấy thông tin 1 sách để Sửa ---
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return Json(new { success = false, message = "Không tìm thấy sách" });

            // Lấy danh sách ID tác giả và thể loại của sách này (nếu đã cài đặt bảng quan hệ)
            // Tạm thời giả định bảng quan hệ là AuthorDetails và CategoryDetails
            var authorIds = await _context.AuthorDetails
                                .Where(x => x.ProductId == id).Select(x => x.AuthorId).ToListAsync();
            var categoryIds = await _context.CategoryDetails
                                .Where(x => x.ProductId == id).Select(x => x.CategoryId).ToListAsync();

            return Json(new
            {
                success = true,
                data = book,
                authors = authorIds,
                categories = categoryIds
            });
        }

        // --- 2. Xử lý Thêm mới ---
        // --- Xử lý Thêm mới ---
        [HttpPost]
        public async Task<IActionResult> Create(Book book, int[] authorIds, int[] categoryIds)
        {
            try
            {
                // Kiểm tra dữ liệu bắt buộc
                if (book.PublisherId == 0) return Json(new { success = false, message = "Vui lòng chọn Nhà xuất bản!" });

                // 1. Lưu sách trước để lấy ID
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // 2. Lưu Tác giả (Dùng Distinct để loại bỏ trùng lặp nếu lỡ chọn 2 lần)
                if (authorIds != null && authorIds.Length > 0)
                {
                    foreach (var authId in authorIds.Distinct())
                    {
                        _context.AuthorDetails.Add(new AuthorDetail { ProductId = book.Id, AuthorId = authId });
                    }
                }

                // 3. Lưu Thể loại
                if (categoryIds != null && categoryIds.Length > 0)
                {
                    foreach (var catId in categoryIds.Distinct())
                    {
                        _context.CategoryDetails.Add(new CategoryDetail { ProductId = book.Id, CategoryId = catId });
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm thành công!" });
            }
            catch (Exception ex)
            {
                // Ghi chi tiết lỗi ra để dễ sửa
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi SQL: " + msg });
            }
        }

        // --- Xử lý Cập nhật ---
        [HttpPost]
        public async Task<IActionResult> Edit(Book book, int[] authorIds, int[] categoryIds)
        {
            try
            {
                // Kiểm tra
                if (book.PublisherId == 0) return Json(new { success = false, message = "Vui lòng chọn Nhà xuất bản!" });

                _context.Books.Update(book);

                // Xóa Tác giả & Thể loại cũ
                var oldAuthors = _context.AuthorDetails.Where(x => x.ProductId == book.Id);
                _context.AuthorDetails.RemoveRange(oldAuthors);

                var oldCats = _context.CategoryDetails.Where(x => x.ProductId == book.Id);
                _context.CategoryDetails.RemoveRange(oldCats);

                // Lưu tạm để xóa cũ đã
                await _context.SaveChangesAsync();

                // Thêm lại cái mới (Có lọc trùng)
                if (authorIds != null)
                {
                    foreach (var authId in authorIds.Distinct())
                        _context.AuthorDetails.Add(new AuthorDetail { ProductId = book.Id, AuthorId = authId });
                }
                if (categoryIds != null)
                {
                    foreach (var catId in categoryIds.Distinct())
                        _context.CategoryDetails.Add(new CategoryDetail { ProductId = book.Id, CategoryId = catId });
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi SQL: " + msg });
            }
        }

        // --- 4. Xóa ---
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.Books.FindAsync(id);
                if (item == null) return Json(new { success = false, message = "Không tìm thấy sản phẩm" });

                // 1. CHECK AN TOÀN: Kiểm tra xem sách có thực sự nằm trong Đơn hàng nào không?
                // Nếu có thì TUYỆT ĐỐI KHÔNG XÓA để tránh làm hỏng lịch sử mua hàng.
                // (Giả sử bảng chi tiết đơn hàng tên là OrderDetails và cột là ProductId)
                var inOrder = await _context.OrderDetails.AnyAsync(x => x.ProductId == id);
                if (inOrder)
                {
                    return Json(new { success = false, message = "Không thể xóa: Sách này đã phát sinh đơn hàng!" });
                }

                // 2. Nếu an toàn, tiến hành dọn dẹp các bảng phụ (Tác giả, Thể loại)
                // SQL không tự xóa giùm nên ta phải code xóa tay trước.

                var relatedAuthors = _context.AuthorDetails.Where(x => x.ProductId == id);
                _context.AuthorDetails.RemoveRange(relatedAuthors);

                var relatedCats = _context.CategoryDetails.Where(x => x.ProductId == id);
                _context.CategoryDetails.RemoveRange(relatedCats);

                // 3. Cuối cùng mới xóa Sách
                _context.Books.Remove(item);

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Ghi rõ lỗi hệ thống để debug
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi SQL: " + errorMessage });
            }
        }

        public IActionResult GetAuxData()
        {
            return Json(new
            {
                categories = _context.Categories.Select(x => new { x.Id, x.Name }).ToList(),
                authors = _context.Authors.Select(x => new { x.Id, x.Name }).ToList(),
                publishers = _context.Publishers.Select(x => new { x.Id, x.Name }).ToList(),

                // --- THÊM DÒNG NÀY ---
                suppliers = _context.Suppliers.Select(x => new { x.Id, x.Name }).ToList()
            });
        }
    }
}