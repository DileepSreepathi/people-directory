using PeopleDirectory.Application.DTOs;

namespace PeopleDirectory.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
}
