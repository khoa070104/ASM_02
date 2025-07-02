using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagement.Models;

[Table("NewsTag")]
public class NewsTag
{
    [Key, Column(Order = 0)]
    public string NewsArticleId { get; set; } = string.Empty;

    [Key, Column(Order = 1)]
    public int TagId { get; set; }

    // Navigation properties
    [ForeignKey("NewsArticleId")]
    public virtual NewsArticle NewsArticle { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; } = null!;
}
