using DotLiquid;
using System.Reflection;
using System.Xml.Linq;

namespace Dummy.Infrastructure.Helpers
{
    public static class EmailHelper
    {
        public static String GetEmailTemplate(string eventType, object properties)
        {
            var document = ReadConfigFile();
            XElement selectedElement = document.Descendants().Where(x => x.Attribute("id").ToString() == eventType).FirstOrDefault();
            var emailTemplate = selectedElement.ToString();
            var result = SetEmailProperties(emailTemplate, properties);
            return result;
        }
        private static XDocument ReadConfigFile()
        {
            string xmlSoruce = "";
            var infrastructureAssembly = Assembly.GetExecutingAssembly();
            var resource = $"{infrastructureAssembly.GetName().Name}.Services.EmailService.Template.EmailTemplate.xml";

            using (Stream resourceStream = infrastructureAssembly.GetManifestResourceStream(resource))
            {
                if (resourceStream != null)
                {
                    using (StreamReader streamReader = new StreamReader(resourceStream))
                    {
                        xmlSoruce = streamReader.ReadToEnd();
                    }

                    XDocument document = XDocument.Parse(xmlSoruce);
                    return document;
                }

                return null;
            }
        }

        public static string GetEmailSubject(string eventType)
        {
            var document = ReadConfigFile();
            XElement emailSubjectElement = document.Descendants().Where(x => x.Attribute("subject").ToString() == eventType).FirstOrDefault();
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
