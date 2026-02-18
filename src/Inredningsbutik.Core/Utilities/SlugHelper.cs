using System.Text.RegularExpressions;

namespace Inredningsbutik.Core.Utilities;

public static class SlugHelper
{
    public static string Slugify(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";

        var s = input.Trim().ToLowerInvariant();
        s = Regex.Replace(s, @"\s+", "-");
        s = Regex.Replace(s, @"[^a-z0-9\-]", "");
        s = Regex.Replace(s, @"-+", "-").Trim('-');
        return s;
    }
}
