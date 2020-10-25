using AutoFixture;
using AutoFixture.AutoMoq;

namespace dngrep.core.xunit.TestHelpers
{
    public static class AutoFixtureFactory
    {
        public static IFixture Default()
        {
            return new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        }
    }
}
