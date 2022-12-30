using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace bridgefluence.Tools
{
    public static class UtilityMethods
    {
        private static readonly String BrandAssetUrl =
            "https://assets.x.com/images/brands/";

        private static readonly String AppStorageUrl =
            "https://storage.x.com/";

        public static String GetAvatarUrl(String newAvatarKey, String currentAvatarUrl)
        {
            return newAvatarKey?.Length > 0 ? $"{AppStorageUrl}{newAvatarKey}" : currentAvatarUrl;
        }

        public static int? ExtractUserId(string authQueryString)
        {
            var queryParams = authQueryString
                .Split('&');

            var userId = int.Parse(queryParams
                .Single(x => x.StartsWith("user_id="))
                .Replace("user_id=", ""));

            return userId;
        }
    }
}