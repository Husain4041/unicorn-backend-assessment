using Microsoft.AspNetCore.Mvc;
using UnicornBackend.DTOs;

[ApiController]
[Route("api/v1/claims")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _service;

    public ClaimsController(IClaimService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClaimDto dto)
    {
        var result = await _service.CreateClaim(dto);
        return Created("", result);
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(int id, AssignDto dto)
    {
        var result = await _service.Assign(id, dto.MakerId);
        return Ok(result);
    }
}
