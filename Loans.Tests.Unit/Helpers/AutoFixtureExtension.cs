using AutoFixture;
using AutoFixture.AutoMoq;

namespace Loans.Tests.Unit.Helpers
{
    public static class AutoFixtureExtension
    {
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
            return fixture;
        }
    }
}
