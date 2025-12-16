using BookStore.Data;
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookStoreContext _context;

        public HomeController(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();

            // Vòng lặp lấy tên tác giả đầu tiên cho từng cuốn sách để hiển thị ở trang chủ
            foreach (var book in books)
            {
                var authorName = await (from ad in _context.AuthorDetails
                                        join a in _context.Authors on ad.AuthorId equals a.Id
                                        where ad.ProductId == book.Id
                                        select a.Name).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(authorName))
                {
                    book.Author = authorName;
                }
            }

            return View(books);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            var publisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Id == book.PublisherId);

            var authors = await (from ad in _context.AuthorDetails
                                 join a in _context.Authors on ad.AuthorId equals a.Id
                                 where ad.ProductId == id
                                 select a.Name).ToListAsync();

            var categories = await (from cd in _context.CategoryDetails
                                    join c in _context.Categories on cd.CategoryId equals c.Id
                                    where cd.ProductId == id
                                    select c.Name).ToListAsync();

            var viewModel = new BookDetailViewModel
            {
                Book = book,
                PublisherName = publisher?.Name ?? "Đang cập nhật",
                Authors = authors,
                Categories = categories
            };

            return View(viewModel);
        }

        public IActionResult Cart()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            if (request == null || request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { success = false, message = "Giỏ hàng trống" });
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // 1. Lưu thông tin giao hàng
                var delivery = new DeliveryInfo
                {
                    UserId = "guest", // Map vào user guest đã tạo ở Bước 1
                    FullName = request.FullName,
                    PhoneNumber = request.Phone,
                    Address = request.Address
                };
                _context.DeliveryInfos.Add(delivery);
                _context.SaveChanges(); // Để lấy delivery.Id

                // 2. Lưu đơn hàng
                double totalPrice = request.Items.Sum(x => x.Price * x.Quantity);
                var order = new Order
                {
                    DeliveryInfoId = delivery.Id,
                    DateCreate = DateTime.Now,
                    TotalPrice = totalPrice,
                    StatusId = 1 // Chờ duyệt
                };
                _context.Orders.Add(order);
                _context.SaveChanges(); // Để lấy order.Id

                // 3. Lưu chi tiết đơn hàng
                foreach (var item in request.Items)
                {
                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.Id,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _context.OrderDetails.Add(detail);
                }
                _context.SaveChanges();

                transaction.Commit();

                // 4. Gửi email (Chạy background để không làm đơ web)
                Task.Run(() => {
                    try
                    {
                        new EmailService().SendOrderConfirmation(request.Email, order.Id.ToString(), totalPrice, request.FullName);
                    }
                    catch { /* Ghi log lỗi mail nếu cần */ }
                });

                return Ok(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}