namespace UnicornBackend.DTOs;

public record CheckerReviewDto(
    int CheckerId,
    string Decision,
    string Feedback
);
