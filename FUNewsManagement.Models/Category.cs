using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagement.Models;

[Table("Category")]
public class Category
{
    [Key]
    public short CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? CategoryDescription { get; set; }

    public short? ParentCategoryId { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey("ParentCategoryId")]
    public virtual Category? ParentCategory { get; set; }

    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
