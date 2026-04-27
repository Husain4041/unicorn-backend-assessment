using Microsoft.AspNetCore.Mvc;
using UnicornBackend.DTOs;
using UnicornBackend.Services;

namespace UnicornBackend.Controllers;

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
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/assign")]
    public async Task<IActionResult> Assign(int id, AssignDto dto)
    {
        try
        {
            var result = await _service.Assign(id, dto.MakerId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/maker-review")]
    public async Task<IActionResult> MakerReview(int id, MakerReviewDto dto)
    {
        try
        {
            var result = await _service.MakerReview(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/checker-review")]
    public async Task<IActionResult> CheckerReview(int id, CheckerReviewDto dto)
    {
        try
        {
            var result = await _service.CheckerReview(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int? insuranceCompanyId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetClaims(status, insuranceCompanyId, from, to, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetById(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}

