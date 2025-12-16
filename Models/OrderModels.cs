using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
	// Bảng thông tin giao hàng
	[Table("delivery_infoes")]
	public class DeliveryInfo
	{
		[Key]
		[Column("user_info_id")]
		public int Id { get; set; }

		[Column("user_id")]
		public string UserId { get; set; } = "guest";

		[Column("fullname")]
		public string FullName { get; set; }

		[Column("phone_number")]
		public string PhoneNumber { get; set; }

		[Column("address")]
		public string Address { get; set; }

		// Các cột bắt buộc khác trong DB
		[Column("city")] public string City { get; set; } = "";
		[Column("district")] public string District { get; set; } = "";
		[Column("ward")] public string Ward { get; set; } = "";
	}

	// Bảng đơn hàng
	[Table("orders")]
	public class Order
	{
		[Key]
		[Column("id")]
		public int Id { get; set; }

		[Column("delivery_info_id")]
		public int DeliveryInfoId { get; set; }

		[Column("date_create")]
		public DateTime DateCreate { get; set; } = DateTime.Now;

		[Column("total_price")]
		public double TotalPrice { get; set; }

		[Column("status_id")]
		public int StatusId { get; set; } = 1; // 1 = Chờ duyệt
	}

	// Bảng chi tiết đơn hàng
	[Table("order_details")]
	[Microsoft.EntityFrameworkCore.PrimaryKey(nameof(OrderId), nameof(ProductId))]
	public class OrderDetail
	{
		[Column("order_id")]
		public int OrderId { get; set; }

		[Column("product_id")]
		public int ProductId { get; set; }

		[Column("quantity")]
		public int Quantity { get; set; }

		[Column("price")]
		public double Price { get; set; }
	}

	// Class nhận dữ liệu từ Javascript (DTO)
	public class CheckoutRequest
	{
		public string FullName { get; set; }
		public string Phone { get; set; }
		public string Address { get; set; }
		public string Email { get; set; } // Để gửi mail
		public List<CartItem> Items { get; set; }
	}

	public class CartItem
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
		public double Price { get; set; }
	}
}