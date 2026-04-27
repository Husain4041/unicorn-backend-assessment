using UnicornBackend.Models;

namespace UnicornBackend.Services;

public interface IClaimService
{
    Task<Claim> CreateClaim(DTOs.CreateClaimDto dto);

    Task<Claim> Assign(int claimId, int makerId);

    Task<Claim> MakerReview(int claimId, DTOs.MakerReviewDto dto);

    Task<Claim> CheckerReview(int claimId, DTOs.CheckerReviewDto dto);

    Task<IEnumerable<Claim>> GetClaims(
        string? status,
        int? insuranceCompanyId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize
    );

    Task<Claim?> GetById(int id);
}

