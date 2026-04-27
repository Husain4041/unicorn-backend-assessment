namespace UnicornBackend.DTOs;

public record MakerReviewDto(
    int MakerId,
    string Recommendation,
    string Feedback
);
