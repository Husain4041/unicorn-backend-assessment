using System.ComponentModel.DataAnnotations;

namespace UnicornBackend.Models;

public class Claim
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string PatientNationality { get; set; } = string.Empty;
    public int InsuranceCompanyId { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string MedicalReason { get; set; } = string.Empty;
    public decimal ClaimAmount { get; set; }
    public ClaimStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }  = new byte[0];
}

