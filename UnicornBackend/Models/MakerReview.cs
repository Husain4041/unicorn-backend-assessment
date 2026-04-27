namespace UnicornBackend.Models;

public class MakerReview
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int MakerId { get; set; }
    public string Recommendation { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
