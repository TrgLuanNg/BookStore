using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
        {
        }
        // Các bảng cũ (đã có)
        public DbSet<Book> Books { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<DeliveryInfo> DeliveryInfos { get; set; }

        // CÁC BẢNG MỚI CẦN THÊM VÀO ĐÂY:
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorDetail> AuthorDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryDetail> CategoryDetails { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }         // <--- Mới
        public DbSet<Discount> Discounts { get; set; }         // <--- Mới
        public DbSet<Function> Functions { get; set; }         // <--- Mới
        public DbSet<FunctionDetail> FunctionDetails { get; set; } // <--- Mới
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; } // <--- Mới
        public DbSet<GoodsReceiptDetail> GoodsReceiptDetails { get; set; } // <--- Mới
    }
}