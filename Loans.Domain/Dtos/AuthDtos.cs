namespace Loans.Domain.Dtos
{
    public record RegisterDto(string FirstName, string LastName, string Username, string Email, string Password, int Age, decimal MonthlyIncome);
    public record LoginDto(string Username, string Password);
    public record AuthResponseDto(string Token, UserDto User);
}
