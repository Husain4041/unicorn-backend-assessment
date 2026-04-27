namespace UnicornBackend.DTOs;

public record CreateClaimDto(
    string PatientName,
    int PatientAge,
    string PatientNationality,
    int InsuranceCompanyId,
    string ClaimType,
    string MedicalReason,
    decimal ClaimAmount
);
