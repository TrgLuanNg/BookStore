using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    // 1. Nhà cung cấp (Suppliers)
    [Table("suppliers")]
    public class Supplier
    {
        [Key][Column("id")] public int Id { get; set; }
        [Required][Column("name")] public string Name { get; set; } = "";
        [Column("email")] public string? Email { get; set; }
        [Column("address")] public string? Address { get; set; }
        [Column("number_phone")] public string? NumberPhone { get; set; }
        [Column("status")] public int Status { get; set; } = 1;
    }

    // 2. Mã giảm giá (Discounts)
    [Table("discounts")]
    public class Discount
    {
        [Key][Column("discount_code")] public string Code { get; set; } = "";
        [Column("discount_value")] public int Value { get; set; }
        [Column("quantity")] public int Quantity { get; set; }
        [Column("date_start")] public DateTime? DateStart { get; set; }
        [Column("date_end")] public DateTime? DateEnd { get; set; }
        [Column("status")] public int Status { get; set; } = 1;
    }

    // 3. Quyền hạn
    [Table("functions")]
    public class Function
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("name")] public string Name { get; set; } = "";
        [Column("description")] public string? Description { get; set; }
    }

    [Table("function_details")]
    public class FunctionDetail
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("role_id")] public int RoleId { get; set; }
        [Column("function_id")] public int FunctionId { get; set; }
        [Column("action")] public bool Action { get; set; }
    }

    // 4. Phiếu nhập hàng
    [Table("goodsreceipts")]
    public class GoodsReceipt
    {
        [Key][Column("id")] public int Id { get; set; }
        [Column("staff_id")] public string StaffId { get; set; } = "admin";
        [Column("total_price")] public double TotalPrice { get; set; }
        [Column("date_create")] public DateTime DateCreate { get; set; } = DateTime.Now;
    }

    [Table("goodsreceipt_details")]
    [PrimaryKey(nameof(GoodsReceiptId), nameof(ProductId))]
    public class GoodsReceiptDetail
    {
        [Column("goodsreceipt_id")] public int GoodsReceiptId { get; set; }
        [Column("product_id")] public int ProductId { get; set; }
        [Column("quantity")] public int Quantity { get; set; }
        [Column("input_price")] public double InputPrice { get; set; }
    }

    // 5. DTOs
    public class ProductCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public string ImagePath { get; set; } = "";
        public int Quantity { get; set; }
        public int PublisherId { get; set; }
        public int SupplierId { get; set; }
        public string? Description { get; set; }
        public List<int> AuthorIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
    }

    public class ReceiptDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    // --- BỔ SUNG CLASS NÀY ĐỂ FIX LỖI ---
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
        public int Status { get; set; }
    }
}