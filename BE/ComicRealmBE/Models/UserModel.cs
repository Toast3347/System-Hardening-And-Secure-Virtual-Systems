using ComicRealmBE.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicRealmBE.Models
{
    /// <summary>
    /// User entity for authentication and authorization
    /// 
    /// Security Considerations:
    /// - Password is stored as Argon2 hash, never plaintext
    /// - Email is unique and indexed for fast lookups
    /// - CreatedBy tracks user hierarchy (SuperAdmin -> Admin -> Friend)
    /// - IsActive allows soft deactivation without data loss
    /// </summary>
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("email")]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        public UserRole Role { get; set; }

        [Column("created_by")]
        [ForeignKey(nameof(CreatedByUser))]
        public int? CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual User? CreatedByUser { get; set; }
        public virtual ICollection<User> CreatedUsers { get; set; } = new List<User>();
        public virtual ICollection<Comic> Comics { get; set; } = new List<Comic>();
    }
}
