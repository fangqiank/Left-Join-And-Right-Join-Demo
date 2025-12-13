namespace LeftJoinAndRightJoinDemo.DTOs
{
    public class ProductReviewDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int? ReviewId { get; set; }
        public string? ReviewTitle { get; set; }
        public int? ReviewRating { get; set; }
        public string? ReviewerName { get; set; }
    }
}
