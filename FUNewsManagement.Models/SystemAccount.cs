using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FUNewsManagement.Models
{
    public class SystemAccount
    {
        [Key]
        public short AccountID { get; set; }
        [Required]
        public string AccountName { get; set; } = null!;
        [Required]
        public string AccountEmail { get; set; } = null!;
        [Required]
        public short AccountRole { get; set; }
        [Required]
        public string AccountPassword { get; set; } = null!;
        public virtual ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
        public virtual ICollection<NewsArticle> UpdatedNewsArticles { get; set; } = new List<NewsArticle>();
    }
} 