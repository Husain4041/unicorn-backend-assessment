using UnicornBackend.DTOs;
using UnicornBackend.Models;

public interface IClaimService
{
    Task<Claim> CreateClaim(CreateClaimDto dto);
    Task<Claim> Assign(int id, int makerId);
    Task<Claim> MakerReview(int id, MakerReviewDto dto);
    Task<Claim> CheckerReview(int id, CheckerReviewDto dto);
}
