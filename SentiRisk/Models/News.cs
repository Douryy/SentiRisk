using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; } = string.Empty;
        [Required]
        public string? Content { get; set; } = string.Empty;
        [Required]
        public DateTime PublishedDate { get; set; }
        public int AssetId { get; set; }
        public virtual Asset? Asset { get; set; }
        public virtual ICollection<SentimentScore>? ListeSentimentScores { get; set; }
    }
}
