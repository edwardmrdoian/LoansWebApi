using AutoMapper;
using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Application.Settings;
using Loans.Contracts;
using Loans.Domain;
using Loans.Domain.Dtos;
using Loans.Domain.Entities;
using Loans.Domain.Errors;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Loans.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwt;
        private readonly ILoggerManager _logger;

        public AuthService(IRepositoryManager repo, IMapper mapper, IOptions<JwtSettings> jwtOptions, ILoggerManager logger)
        {
            _repo = repo;
            _mapper = mapper;
            _jwt = jwtOptions.Value;
            _logger = logger;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            var exist = await _repo.User.GetByUsernameAsync(dto.Username, false);
            if (exist != null) throw new BadRequestException($"Username '{dto.Username}' is already taken.", ErrorCodes.BAD_REQUEST);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Email = dto.Email,
                Age = dto.Age,
                MonthlyIncome = dto.MonthlyIncome,
                IsBlocked = false,
                Role = Role.User,
                PasswordHash = BC.EnhancedHashPassword(dto.Password, 13)
            };

            _repo.User.Create(user);
            await _repo.SaveAsync();

            _logger.LogInfo($"User registered: {user.Username} ({user.Id})");

            return new UserDto(user.Id, user.FirstName, user.LastName, user.Username, user.Age, user.MonthlyIncome, user.Email, user.IsBlocked, user.Role.ToString());
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _repo.User.GetByUsernameAsync(dto.Username, false)
                    ?? throw new NotFoundException("User not found", ErrorCodes.USER_NOT_FOUND);
            if (!BC.EnhancedVerify(dto.Password, user.PasswordHash)) throw new BadRequestException("Invalid credentials", ErrorCodes.BAD_REQUEST);

            var token = GenerateJwtToken(user);
            var userDto = new UserDto(user.Id, user.FirstName, user.LastName, user.Username, user.Age, user.MonthlyIncome, user.Email, user.IsBlocked, user.Role.ToString());

            _logger.LogInfo($"User logged in: {user.Username} ({user.Id})");

            return new AuthResponseDto(token, userDto);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
