using System.Text.RegularExpressions;

namespace API.Helpers;

public static partial class YouTubeValidator
    {
        private static readonly Regex VideoIdRegex = MyRegex();

        public static bool IsValidYouTubeUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
                return false;

            var host = uriResult.Host.ToLowerInvariant();

            var isYouTubeHost =
                host == "youtube.com"   ||
                host == "www.youtube.com" ||
                host == "youtu.be";

            if (!isYouTubeHost)
                return false;
            
            if (host == "youtu.be")
            {
                // Path is like “/dQw4w9WgXcQ” or maybe “/dQw4w9WgXcQ?t=30s”
                var path = uriResult.AbsolutePath.Trim('/');
                
                return !string.IsNullOrEmpty(path) && VideoIdRegex.IsMatch(path);
            }
            
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uriResult.Query);
            if (!query.TryGetValue("v", out var vValues))
                return false;
            
            var videoId = vValues.ToString().Trim();
            
            return VideoIdRegex.IsMatch(videoId);
        }

    [GeneratedRegex(@"^[a-zA-Z0-9_-]{11}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}