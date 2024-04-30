using Newtonsoft.Json;

namespace Dummy.Infrastructure.Helpers
{
    public static class LoadJsonFileHelper
    {
        public static Dictionary<string, string> LoadJsonFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
    }
}
