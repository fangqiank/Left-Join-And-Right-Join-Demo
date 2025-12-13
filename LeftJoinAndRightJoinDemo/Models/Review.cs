using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeftJoinAndRightJoinDemo.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(100)]
        public string ReviewerName { get; set; } = string.Empty;

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        // 外键
        public int ProductId { get; set; }

        // 导航属性
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
