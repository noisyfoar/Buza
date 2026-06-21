using Buza.Dtos;

namespace Buza.Services;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
}
