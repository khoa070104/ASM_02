using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagement.Models;

[Table("NewsArticle")]
public class NewsArticle
{
    [Key]
    public string NewsArticleId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string NewsTitle { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Headline { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Required]
    public string NewsContent { get; set; } = string.Empty;

    [StringLength(200)]
    public string? NewsSource { get; set; }

    [Required]
    public short CategoryId { get; set; }

    [Required]
    public bool NewsStatus { get; set; } = true;

    [Required]
    public short CreatedById { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("CreatedById")]
    public virtual SystemAccount CreatedBy { get; set; } = null!;

    [ForeignKey("UpdatedById")]
    public virtual SystemAccount? UpdatedBy { get; set; }

    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
