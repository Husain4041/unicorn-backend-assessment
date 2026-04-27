using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

public class ClaimFlowTests
{
    private readonly HttpClient _client;

    public ClaimFlowTests()
    {
        var app = new WebApplicationFactory<Program>();
        _client = app.CreateClient();
    }

    [Fact]
    public async Task Full_Claim_Flow_Works()
    {
        // Create
        var createRes = await _client.PostAsJsonAsync("/api/v1/claims", new
        {
            patientName = "John",
            patientAge = 40,
            patientNationality = "UAE",
            insuranceCompanyId = 1,
            claimType = "Surgery",
            medicalReason = "Appendix",
            claimAmount = 1000
        });

        var claim = await createRes.Content.ReadFromJsonAsync<dynamic>();
        int id = claim.id;

        // Assign
        await _client.PostAsJsonAsync($"/api/v1/claims/{id}/assign", new { makerId = 1 });

        // Maker review
        await _client.PostAsJsonAsync($"/api/v1/claims/{id}/maker-review", new
        {
            makerId = 1,
            recommendation = "Approve",
            feedback = "ok"
        });

        // Checker review
        var res = await _client.PostAsJsonAsync($"/api/v1/claims/{id}/checker-review", new
        {
            checkerId = 2,
            decision = "Approve",
            feedback = "ok"
        });

        res.EnsureSuccessStatusCode();
    }
}
