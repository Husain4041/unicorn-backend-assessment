namespace UnicornBackend.Models;

public class CheckerReview
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int CheckerId { get; set; }
    public string Decision { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
