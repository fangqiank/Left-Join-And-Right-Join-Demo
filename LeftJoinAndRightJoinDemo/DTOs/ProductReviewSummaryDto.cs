namespace LeftJoinAndRightJoinDemo.DTOs
{
    public class ProductReviewSummaryDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }
}
