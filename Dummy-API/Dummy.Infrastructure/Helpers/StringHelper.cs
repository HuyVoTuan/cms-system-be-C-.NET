namespace Dummy.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string GenerateSlug(string memberName)
        {
            var slug = memberName.Replace(" ", "-");
            return slug;
        }
    }
}
