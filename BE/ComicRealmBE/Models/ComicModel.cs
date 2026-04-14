using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicRealmBE.Models
{
    /// <summary>
    /// Comic entity representing a comic book instance
    /// 
    /// Security Considerations:
    /// - CreatedBy tracks ownership for RBAC
    /// - Database RLS (Row-Level Security) enforces who can view/edit
    /// - Friends can only see comics (Admin creates/edits)
    /// </summary>
    [Table("comics")]
    public class Comic
    {
        [Key]
        [Column("comic_id")]
        public int ComicId { get; set; }

        [Required]
        [Column("serie")]
        [StringLength(255)]
        public string Serie { get; set; } = string.Empty;

        [Required]
        [Column("number")]
        [StringLength(50)]
        public string Number { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("created_by")]
        [ForeignKey(nameof(CreatedByUser))]
        public int CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User CreatedByUser { get; set; } = null!;
    }
}
