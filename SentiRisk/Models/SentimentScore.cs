using System.ComponentModel.DataAnnotations;

namespace SentiRisk.Models
{
    public class SentimentScore
    {
        public int Id { get; set; }
        [Required]
        public decimal Score { get; set; }
        [Required]
        public decimal Confidence { get; set; }
            public int NewsId { get; set; }
        public virtual News? News { get; set; }
    }
}
