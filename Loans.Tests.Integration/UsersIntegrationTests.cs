using FluentAssertions;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Loans.Tests.Integration
{
    public class UsersIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public UsersIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetMyProfile_AfterRegister_ReturnsOk()
        {
            var register = new
            {
                FirstName = "X",
                LastName = "Y",
                Username = "profileuser",
                Email = "p@example.com",
                Password = "Password123!",
                Age = 25,
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
            var token = json.GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var profileRes = await _client.GetAsync("/api/users/me");
            profileRes.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
