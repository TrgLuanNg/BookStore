using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    [Table("suppliers")]
    public class Supplier
    {
        [Key][Column("id")] public int Id { get; set; }
        [Required][Column("name")] public string Name { get; set; } = "";
        [Column("email")] public string Email { get; set; } = "";
        [Column("address")] public string Address { get; set; } = "";
        [Column("number_phone")] public string NumberPhone { get; set; } = ""; // Sửa: Khớp tên biến
        [Column("status")] public int Status { get; set; } = 1;
    }

    // 3. Mã giảm giá (Discounts)
    [Table("discounts")]
    public class Discount
    {
        [Key][Column("discount_code")] public string Code { get; set; } = "";
        [Column("discount_value")] public int Value { get; set; }
        [Column("quantity")] public int Quantity { get; set; }
        [Column("date_start")] public DateTime DateStart { get; set; }
        [Column("date_end")] public DateTime DateEnd { get; set; }
        [Column("status")] public int Status { get; set; } = 1;
    }

    // 4. Quyền hạn (Functions) - Danh sách các chức năng hệ thống
    [Table("functions")]
    public class Function
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }

    // 5. Chi tiết phân quyền (FunctionDetails) - Role nào được làm gì
    [Table("function_details")]
    public class FunctionDetail
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }

        [Column("function_id")]
        public int FunctionId { get; set; }

        [Column("action")]
        public bool Action { get; set; } // 1: Được phép, 0: Cấm
    }

    // 6. Phiếu nhập hàng (GoodsReceipts)
    [Table("goodsreceipts")]
    public class GoodsReceipt
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("staff_id")] // Lưu username của admin/staff nhập hàng
        public string StaffId { get; set; }

        [Column("total_price")]
        public double TotalPrice { get; set; }

        [Column("date_create")]
        public DateTime DateCreate { get; set; } = DateTime.Now;
    }

    // 7. Chi tiết phiếu nhập (GoodsReceiptDetails)
    [Table("goodsreceipt_details")]
    [PrimaryKey(nameof(GoodsReceiptId), nameof(ProductId))] // Khóa chính phức hợp
    public class GoodsReceiptDetail
    {
        [Column("goodsreceipt_id")]
        public int GoodsReceiptId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("input_price")]
        public double InputPrice { get; set; }
    }

    // 8. DTO (Data Transfer Object) - Class phụ để nhận dữ liệu từ Form (Không tạo bảng trong DB)
    public class ProductCreateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImagePath { get; set; }
        public int Quantity { get; set; }
        public int PublisherId { get; set; }
        public int SupplierId { get; set; }
        public string Description { get; set; }
        public List<int> AuthorIds { get; set; }    // Nhận danh sách ID tác giả
        public List<int> CategoryIds { get; set; }  // Nhận danh sách ID thể loại
    }

    // DTO cho nhập hàng
    public class ReceiptDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

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