namespace Loans.Domain.Dtos
{
    public record UserDto(Guid Id, string FirstName, string LastName, string Username, int Age, decimal MonthlyIncome, string Email, bool IsBlocked, string Role);
    public record BlockRequestDto(int Days);
    public record BlockUserDto(int BlockDays);

}
