using Microsoft.EntityFrameworkCore;
using UnicornBackend.Data;
using UnicornBackend.DTOs;
using UnicornBackend.Models;

public class ClaimService : IClaimService
{
    private readonly AppDbContext _db;

    public ClaimService(AppDbContext db)
    {
        _db = db;
    }

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
            CreatedAt = DateTime.UtcNow
        };

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();
        return claim;
    }

    public async Task<Claim> Assign(int id, int makerId)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) throw new Exception("Not found");

        if (claim.Status != ClaimStatus.New)
            throw new Exception("Already assigned");

        claim.Status = ClaimStatus.AssignedToMaker;
        await _db.SaveChangesAsync();

        return claim;
    }

    public async Task<Claim> MakerReview(int id, MakerReviewDto dto)
    {
        var claim = await _db.Claims.FindAsync(id);

        if (claim.Status != ClaimStatus.AssignedToMaker)
            throw new Exception("Invalid state");

        _db.MakerReviews.Add(new MakerReview
        {
            ClaimId = id,
            MakerId = dto.MakerId,
            Recommendation = dto.Recommendation,
            Feedback = dto.Feedback,
            CreatedAt = DateTime.UtcNow
        });

        claim.Status = ClaimStatus.MakerReviewed;
        await _db.SaveChangesAsync();

        return claim;
    }

    public async Task<Claim> CheckerReview(int id, CheckerReviewDto dto)
    {
        var claim = await _db.Claims.FindAsync(id);

        if (claim.Status != ClaimStatus.MakerReviewed)
            throw new Exception("Invalid state");

        _db.CheckerReviews.Add(new CheckerReview
        {
            ClaimId = id,
            CheckerId = dto.CheckerId,
            Decision = dto.Decision,
            Feedback = dto.Feedback,
            CreatedAt = DateTime.UtcNow
        });

        claim.Status = ClaimStatus.Completed;
        await _db.SaveChangesAsync();

        return claim;
    }
}
