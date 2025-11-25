using FluentAssertions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Loans.Tests.Integration
{
    public class LoansIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public LoansIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndGetTokenAsync(string username = "user1")
        {
            var register = new
            {
                FirstName = "T",
                LastName = "U",
                Username = username,
                Email = $"{username}@example.com",
                Password = "Password123!",
                Age = 30,
                MonthlyIncome = 1000
            };

            var http = new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json");
            var res = await _client.PostAsync("/api/auth/register", http);
            res.EnsureSuccessStatusCode();

            var login = new { Username = register.Username, Password = register.Password };
            var http2 = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var loginRes = await _client.PostAsync("/api/auth/login", http2);
            loginRes.EnsureSuccessStatusCode();

            var body = await loginRes.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(body);
            return json.GetProperty("token").GetString();
        }

        [Fact]
        public async Task CreateLoan_AuthorizedUser_CreatesLoan()
        {
            var token = await RegisterAndGetTokenAsync("loanuser1");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var create = new { LoanType = 0, Amount = 1000, Currency = "USD", PeriodMonths = 12 };
            var http = new StringContent(JsonSerializer.Serialize(create), Encoding.UTF8, "application/json");

            var res = await _client.PostAsync("/api/loans", http);
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var body = await res.Content.ReadAsStringAsync();
            body.Should().Contain("\"status\"");
        }

        [Fact]
        public async Task CreateLoan_Unauthorized_Returns401()
        {
            var create = new { LoanType = 0, Amount = 1000, Currency = "USD", PeriodMonths = 12 };
            var http = new StringContent(JsonSerializer.Serialize(create), Encoding.UTF8, "application/json");

            var res = await _client.PostAsync("/api/loans", http);
            res.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
