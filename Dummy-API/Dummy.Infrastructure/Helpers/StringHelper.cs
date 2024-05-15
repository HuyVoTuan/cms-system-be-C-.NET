using NanoidDotNet;
using Slugify;
using System.Text.RegularExpressions;

namespace Dummy.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static bool IsValidString(String value)
        {
            // Regular expression to match only letters and whitespace
            Regex regex = new Regex("^[a-zA-Z\\s]*$");
            return regex.IsMatch(value);
        }

        public static String GenerateSlug(String memberName)
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

        public static bool IsSlugContainFullname(String requestSlug, String existingMemberSlug)
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

        public static String GenerateRefreshToken()
        {
            const int length = 32;
            char[] randomChars = new char[length];
            const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


            Random random = new Random();

            {
                for (int i = 0; i < length; i++)
                {
                    randomChars[i] = chars[random.Next(chars.Length)];
                }
            }

            long timestamp = DateTime.UtcNow.Ticks;
            var uniquePart = Convert.ToString(timestamp, 8);

            var uniqueRandomString = new String(randomChars) + uniquePart;

            if (uniqueRandomString.Length > length)
            {
                uniqueRandomString = uniqueRandomString.Substring(0, length);
            }

            return uniqueRandomString;
        }

        public static String GenerateTemporaryPassword()
        {
            const int length = 6;
            char[] randomChars = new char[length];
            const String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


            Random random = new Random();

            {
                for (int i = 0; i < length; i++)
                {
                    randomChars[i] = chars[random.Next(chars.Length)];
                }
            }

            var temporaryPassword = new string(randomChars);
            return temporaryPassword;
        }

        public static String GenerateJobDetailId(String referenceId) => $"{referenceId}.jobDetail";
        public static String GenerateJobTriggerId(String referenceId) => $"{referenceId}.jobTrigger";
    }
}
