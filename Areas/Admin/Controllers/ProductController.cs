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
            var item = await _context.Books.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy" });

            // Cần xóa các bảng phụ trước nếu không có Cascade Delete trong DB
            // (Tuỳ thuộc vào cấu hình DB của bạn, ở đây xoá book nếu DB có set cascade sẽ tự bay bảng phụ)
            _context.Books.Remove(item);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        public IActionResult GetAuxData()
        {
            return Json(new
            {
                categories = _context.Categories.Select(x => new { x.Id, x.Name }).ToList(),
                authors = _context.Authors.Select(x => new { x.Id, x.Name }).ToList(),
                publishers = _context.Publishers.Select(x => new { x.Id, x.Name }).ToList(),
                suppliers = _context.Suppliers.Select(x => new { x.Id, x.Name }).ToList() // <--- Thêm dòng này
            });
        }
    }
}