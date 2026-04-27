using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class ConcurrencyTests
{
    private readonly HttpClient _client;

    public ConcurrencyTests()
    {
        var factory = new WebApplicationFactory<Program>();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Only_One_Maker_Can_Assign_Claim()
    {
        // Create claim
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

        // Simulate two makers assigning at same time
        var task1 = _client.PostAsJsonAsync($"/api/v1/claims/{id}/assign", new { makerId = 1 });
        var task2 = _client.PostAsJsonAsync($"/api/v1/claims/{id}/assign", new { makerId = 2 });

        await Task.WhenAll(task1, task2);

        var res1 = await task1;
        var res2 = await task2;

        // One should succeed, one should fail
        Assert.True(
            res1.StatusCode == HttpStatusCode.OK && res2.StatusCode == HttpStatusCode.Conflict ||
            res2.StatusCode == HttpStatusCode.OK && res1.StatusCode == HttpStatusCode.Conflict
        );
    }
}

