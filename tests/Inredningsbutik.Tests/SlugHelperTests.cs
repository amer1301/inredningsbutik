using Inredningsbutik.Core.Utilities;
using Xunit;

public class SlugHelperTests
{
    [Theory]
    [InlineData("Ny kategori", "ny-kategori")]
    [InlineData("  Hej   världen!  ", "hej-vrlden")]
    [InlineData("Soffa!!!", "soffa")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Slugify_Works(string? input, string expected)
    {
        Assert.Equal(expected, SlugHelper.Slugify(input));
    }
}
