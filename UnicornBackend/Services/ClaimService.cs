using Microsoft.EntityFrameworkCore;
using UnicornBackend.Data;
using UnicornBackend.DTOs;
using UnicornBackend.Models;

namespace UnicornBackend.Services;

public class ClaimService : IClaimService
{
    private readonly AppDbContext _db;

    public ClaimService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Claim?> GetById(int id)
    {
        return await _db.Claims.FindAsync(id);
    }

    public async Task<IEnumerable<Claim>> GetClaims(
        string? status,
        int? insuranceCompanyId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize)
    {
        var query = _db.Claims.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<ClaimStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(c => c.Status == parsedStatus);
            }
        }

        if (insuranceCompanyId.HasValue)
        {
            query = query.Where(c => c.InsuranceCompanyId == insuranceCompanyId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(c => c.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(c => c.CreatedAt <= to.Value);
        }

        // Pagination
        query = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return await query.ToListAsync();
    }

    // (keep your existing methods below)
    public async Task<Claim> CreateClaim(CreateClaimDto dto)
    {
        var claim = new Claim
        {
            PatientName = dto.PatientName,
            PatientAge = dto.PatientAge,
            PatientNationality = dto.PatientNationality,
            InsuranceCompanyId = dto.InsuranceCompanyId,
            ClaimType = dto.ClaimType,
            MedicalReason = dto.MedicalReason,
            ClaimAmount = dto.ClaimAmount,
            Status = ClaimStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim> Assign(int claimId, int makerId)
    {
        var claim = await _db.Claims.FindAsync(claimId);

        if (claim == null)
            throw new Exception("Claim not found");

        if (claim.Status != ClaimStatus.New)
            throw new Exception("Claim already assigned");

        claim.MakerId = makerId;
        claim.Status = ClaimStatus.AssignedToMaker;
        claim.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim> MakerReview(int claimId, MakerReviewDto dto)
    {
        var claim = await _db.Claims.FindAsync(claimId);

        if (claim == null)
            throw new Exception("Claim not found");

        if (claim.Status != ClaimStatus.AssignedToMaker)
            throw new Exception("Invalid state");

        var review = new MakerReview
        {
            ClaimId = claimId,
            MakerId = dto.MakerId,
            Recommendation = dto.Recommendation,
            Feedback = dto.Feedback,
            CreatedAt = DateTime.UtcNow
        };

        _db.MakerReviews.Add(review);

        claim.Status = ClaimStatus.MakerReviewed;
        claim.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim> CheckerReview(int claimId, CheckerReviewDto dto)
    {
        var claim = await _db.Claims.FindAsync(claimId);

        if (claim == null)
            throw new Exception("Claim not found");

        if (claim.Status != ClaimStatus.MakerReviewed)
            throw new Exception("Invalid state");

        var review = new CheckerReview
        {
            ClaimId = claimId,
            CheckerId = dto.CheckerId,
            Decision = dto.Decision,
            Feedback = dto.Feedback,
            CreatedAt = DateTime.UtcNow
        };

        _db.CheckerReviews.Add(review);

        claim.Status = ClaimStatus.Completed;
        claim.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return claim;
    }
}
