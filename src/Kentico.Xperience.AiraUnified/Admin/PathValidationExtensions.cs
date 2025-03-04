using System.Text.RegularExpressions;

namespace Kentico.Xperience.AiraUnified.Admin;

internal static class PathValidationExtensions
{
    public static bool IsValidSubpath(this string path) =>
        !string.IsNullOrEmpty(path)
        && path[0] == '/'
        && !path.Contains("//")
        // Ensure the path does not contain traversal sequences
        && !path.Contains("..")
        && !path.Contains("%2e%2e")
        && !path.Contains("%2E%2E")
        // Allow only alphanumeric, hyphen, underscore, and single forward slashes
        && Regex.IsMatch(path, @"^\/[a-zA-Z0-9-_\/]*$")
        // Prevent trailing slashes to maintain consistency
        && (path.Length <= 1 || !path.EndsWith('/'));
}
