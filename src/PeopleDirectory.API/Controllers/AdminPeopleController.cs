using System.Security.Claims;

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleDirectory.Application.DTOs;
using PeopleDirectory.Application.Interfaces;

namespace PeopleDirectory.API.Controllers;

[ApiController]
[Route("api/admin/people")]
[Authorize(Roles = "Admin")]
public class AdminPeopleController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IValidator<PersonCreateDto> _createValidator;
    private readonly IValidator<PersonUpdateDto> _updateValidator;
    private readonly IWebHostEnvironment _env;

    public AdminPeopleController(
        IPersonService personService,
        IValidator<PersonCreateDto> createValidator,
        IValidator<PersonUpdateDto> updateValidator,
        IWebHostEnvironment env)
    {
        _personService = personService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _personService.GetFilteredAsync(query, null, null, null, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var person = await _personService.GetByIdAsync(id);
        if (person == null)
            return NotFound(new { message = $"Person with ID {id} not found." });
        return Ok(person);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] PersonCreateDto dto, IFormFile? profilePicture)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        string? pictureUrl = null;
        if (profilePicture != null)
            pictureUrl = await SaveProfilePictureAsync(profilePicture);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "admin";
        var result = await _personService.CreateAsync(dto, pictureUrl, username);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromForm] PersonUpdateDto dto, IFormFile? profilePicture)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

        string? pictureUrl = null;
        if (profilePicture != null)
            pictureUrl = await SaveProfilePictureAsync(profilePicture);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "admin";
        var result = await _personService.UpdateAsync(id, dto, pictureUrl, username);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var username = User.FindFirstValue(ClaimTypes.Name) ?? "admin";
        await _personService.DeleteAsync(id, username);
        return NoContent();
    }

    private async Task<string> SaveProfilePictureAsync(IFormFile file)
    {
        // Validate type
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            throw new InvalidOperationException($"Unsupported image type '{ext}'. Allowed: {string.Join(", ", allowed)}.");

        // Validate size (max 5 MB)
        const long maxBytes = 5 * 1024 * 1024;
        if (file.Length <= 0 || file.Length > maxBytes)
            throw new InvalidOperationException("Profile picture must be between 1 byte and 5 MB.");

        var webRoot = _env.WebRootPath;
        if (string.IsNullOrEmpty(webRoot))
            webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");

        var uploadsDir = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }
}
