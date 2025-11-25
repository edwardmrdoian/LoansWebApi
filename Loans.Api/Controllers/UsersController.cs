using Loans.Application.Interfaces;
using Loans.Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Loans.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// USER can get their own profile 
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var id = GetUserId();
            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        /// <summary>
        /// Get user by ID.
        /// - USER can ONLY get his own profile
        /// - ACCOUNTANT can get ANY user's profile
        /// </summary>
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var callerId = GetUserId();
            var isAccountant = User.IsInRole("Accountant");

            if (!isAccountant && callerId != id)
                return Forbid();

            var user = await _userService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);

        }

        /// <summary>
        /// Accountant can block any user.
        /// </summary>
        [Authorize(Roles = "Accountant")]
        [HttpPost("{id:guid}/block")]
        public async Task<IActionResult> BlockUser(Guid id, [FromBody] BlockUserDto dto)
        {
            await _userService.BlockUserAsync(id, dto.BlockDays);
            return Ok(new { Message = "User has been blocked successfully." });
        }

        private Guid GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value
                        ?? throw new UnauthorizedAccessException();
            return Guid.Parse(idClaim);
        }
    }

}
