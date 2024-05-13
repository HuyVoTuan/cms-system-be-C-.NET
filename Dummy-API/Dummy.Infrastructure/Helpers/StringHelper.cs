using NanoidDotNet;
using Slugify;
using System.Text.RegularExpressions;

namespace Dummy.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static bool IsValidString(string value)
        {
            // Regular expression to match only letters and whitespace
            Regex regex = new Regex("^[a-zA-Z\\s]*$");
            return regex.IsMatch(value);
        }

        public static string GenerateSlug(string memberName)
        {
            var alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var uniqueId = Nanoid.Generate(alphabet, size: 10);

            var config = new SlugHelperConfiguration();
            config.StringReplacements = new()
            {
                ["&"] = "-",
                [","] = "-",
                [" "] = "-",
            };
            config.ForceLowerCase = true;

            var helper = new SlugHelper(config);
            var result = helper.GenerateSlug($"{memberName} {uniqueId}");

            return result;
        }

        public static bool IsSlugContainFullname(string requestSlug, string existingMemberSlug)
        {
            var processedRequestSlug = requestSlug.Split("-");
            var processedExistingMemberSlug = existingMemberSlug.Split("-");

            if (processedRequestSlug.Length != processedExistingMemberSlug.Length)
                return false;

            for (int i = 0; i < processedRequestSlug.Length - 1; i++)
            {
                if (!processedRequestSlug[i].Equals(processedExistingMemberSlug[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        public static string GenerateRefreshToken()
        {
            const int length = 32;
            char[] randomChars = new char[length];
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


            Random random = new Random();

            {
                for (int i = 0; i < length; i++)
                {
                    randomChars[i] = chars[random.Next(chars.Length)];
                }
            }

            long timestamp = DateTime.UtcNow.Ticks;
            string uniquePart = Convert.ToString(timestamp, 8);

            string uniqueRandomString = new string(randomChars) + uniquePart;

            if (uniqueRandomString.Length > length)
            {
                uniqueRandomString = uniqueRandomString.Substring(0, length);
            }

            return uniqueRandomString;
        }

        public static string GenerateJobDetailId(String referenceId) => $"{referenceId}.jobDetail";
        public static string GenerateJobTriggerId(String referenceId) => $"{referenceId}.jobTrigger";
    }
}
