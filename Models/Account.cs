using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; } // 1: Admin, 2: Staff, 3: Customer

        [Column("status")]
        public int Status { get; set; } = 1; // 1: Active, 0: Blocked
    }
}