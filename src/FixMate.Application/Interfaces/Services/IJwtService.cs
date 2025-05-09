using System.Threading.Tasks;
using FixMate.Application.DTOs;

namespace FixMate.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user);
    }
} 