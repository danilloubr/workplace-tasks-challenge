using WorkplaceTasks.Application.DTOs;

namespace WorkplaceTasks.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginRequestDto loginRequestDto);
    }
}