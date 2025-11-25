using FluentAssertions;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Loans.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_Then_Login_Succeeds()
        {
            var register = new
            {
                FirstName = "Test",
                LastName = "User",
                Username = "inttestuser",
                Email = "intuser@example.com",
                Password = "Password123!",
                Age = 28,
                MonthlyIncome = 2000
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

            json.GetProperty("token").GetString().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Login_InvalidUser_ReturnsNotFound()
        {
            var login = new { Username = "doesnotexist", Password = "pass" };
            var http2 = new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json");
            var loginRes = await _client.PostAsync("/api/auth/login", http2);

            loginRes.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
