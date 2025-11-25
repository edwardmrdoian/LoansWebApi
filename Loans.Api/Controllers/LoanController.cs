using Loans.Application.Exceptions;
using Loans.Application.Interfaces;
using Loans.Domain.Dtos;
using Loans.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Loans.Api.Controllers
{
    [ApiController]
    [Route("api/loans")]
    [Authorize]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoansController(ILoanService loanService) => _loanService = loanService;

        private Guid GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub)?.Value
                 ?? throw new UnauthorizedException("", ErrorCodes.UNAUTHORIZED);
            return Guid.Parse(idClaim);
        }

        [Authorize(Roles = "User")]
        [HttpPost("createLoan")]
        public async Task<IActionResult> CreateLoan([FromBody] LoanCreateDto dto)
        {
            var userId = GetUserId();
            var loan = await _loanService.CreateLoanAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
        }
        [Authorize(Roles = "User")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = GetUserId();
            var loansList = await _loanService.GetLoansForUserAsync(userId);
            return Ok(loansList);
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var callerId = GetUserId();
            var isAccountant = User.IsInRole("Accountant");
            var loan = await _loanService.GetLoanByIdAsync(id, callerId, isAccountant);
            return Ok(loan);
        }

        [Authorize(Roles = "Accountant")]
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetLoansForUser(Guid userId)
        {
            var loans = await _loanService.GetLoansByUserIdAsync(userId);
            return Ok(loans);
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LoanUpdateDto dto)
        {
            var callerId = GetUserId();
            var isAccountant = User.IsInRole("Accountant");
            await _loanService.UpdateLoanAsync(id, callerId, dto, isAccountant);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var callerId = GetUserId();
            var isAccountant = User.IsInRole("Accountant");
            await _loanService.DeleteLoanAsync(id, callerId, isAccountant);
            return NoContent();
        }

        [Authorize(Roles = "Accountant")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var all = await _loanService.GetAllLoansAsync();
            return Ok(all);
        }

        [Authorize(Roles = "Accountant")]
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> SetStatus(Guid id, [FromBody] LoanStatusUpdateDto dto)
        {
            await _loanService.OverrideLoanStatusAsync(id, dto.Status);
            return Ok();
        }
    }

}