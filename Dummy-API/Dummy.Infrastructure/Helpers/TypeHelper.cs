using System.Reflection;

namespace Dummy.Infrastructure.Helpers
{
    public static class TypeHelper
    {
        public static List<String> GetConstants(Type type)
        {
            var constants = new List<String>();

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(String))
                {
                    constants.Add((String)fieldInfo.GetValue(null));
                }
            }
            return constants;
        }
    }
}
