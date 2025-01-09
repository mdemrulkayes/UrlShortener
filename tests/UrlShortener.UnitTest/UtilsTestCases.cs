using FluentAssertions;
using UrlShortener.Api;

namespace UrlShortener.UnitTest;

public class UtilsTestCases
{
    [Fact]
    public void Generate_SixDigitUnique_ShortCode_ShouldReturnTrue()
    {
        var shortCode = Utils.GenerateShortCode(6);

        shortCode.Length.Should().Be(6);
    }
}
