using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUNewsManagement.Models;

[Table("Tag")]
public class Tag
{
    [Key]
    public int TagId { get; set; }

    [Required]
    [StringLength(50)]
    public string TagName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Note { get; set; }

    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
