using DotLiquid;
using System.Reflection;
using System.Xml.Linq;

namespace Dummy.Infrastructure.Helpers
{
    public static class EmailHelper
    {
        private static XDocument ReadConfigFile()
        {
            string xmlSource = "";
            var infrastructureAssembly = Assembly.GetExecutingAssembly();
            var resource = $"{infrastructureAssembly.GetName().Name}.Services.EmailService.Template.EmailTemplate.xml";

            using (Stream resourceStream = infrastructureAssembly.GetManifestResourceStream(resource))
            {
                if (resourceStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(resourceStream))
                    {
                        xmlSource = streamReader.ReadToEnd();
                    }

                    XDocument document = XDocument.Parse(xmlSource);
                    return document;
                }

                return null;
            }
        }
        public static String GetEmailTemplate(string eventType, object properties)
        {
            var document = ReadConfigFile();
            XElement selectedElement = document.Descendants().FirstOrDefault(x => (String)x.Attribute("id") == eventType);
            var emailTemplate = selectedElement.ToString();
            var result = SetEmailProperties(emailTemplate, properties);
            return result;
        }

        public static string GetEmailSubject(string eventType)
        {
            var document = ReadConfigFile();
            XElement emailSubjectElement = document.Descendants().FirstOrDefault(x => (String)x.Attribute("subject") == eventType);
            return emailSubjectElement.Value;
        }

        private static string SetEmailProperties(string emailTemplate, object properties)
        {
            var template = Template.Parse(emailTemplate);
            var hashedJson = Hash.FromAnonymousObject(properties);
            return template.Render(hashedJson);
        }
    }
}
