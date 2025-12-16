using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BookStore.Models
{
    // --- 1. Model Sách ---
    [Table("products")]
    public class Book
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = "";

        [Column("image_path")]
        public string ImagePath { get; set; } = "";

        [Column("price")]
        public double Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("publisher_id")]
        public int PublisherId { get; set; }

        // --- BỔ SUNG: SupplierId (Nhà cung cấp) ---
        [Column("supplier_id")]
        public int SupplierId { get; set; }

        // --- SỬA: Description (Cho phép Null để tránh lỗi DBNull) ---
        [Column("description")]
        public string? Description { get; set; }

        [NotMapped]
        public string Author { get; set; } = "Đang cập nhật";
    }

    // --- 2. Model Nhà Xuất Bản ---
    [Table("publishers")]
    public class Publisher
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";

        [Column("email")]
        public string Email { get; set; } = "";
    }

    [Table("authors")]
    public class Author
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";
    }

    [Table("author_details")]
    [PrimaryKey(nameof(ProductId), nameof(AuthorId))]
    public class AuthorDetail
    {
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("author_id")]
        public int AuthorId { get; set; }
    }

    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = "";
    }

    [Table("category_details")]
    [PrimaryKey(nameof(ProductId), nameof(CategoryId))]
    public class CategoryDetail
    {
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }
    }

    public class BookDetailViewModel
    {
        public Book Book { get; set; } = new Book();
        public string PublisherName { get; set; } = "";
        public List<string> Authors { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}