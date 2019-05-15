using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Executer.Utilities
{
    public static class SecretManager
    {
        public const string SecretFileName = "secrets.json";
        public static string GetSecret(string secretName, string defaultValue)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName =
                assembly.GetManifestResourceNames()
                .FirstOrDefault(k => k.EndsWith(SecretFileName));
            if (resourceName == null)
            {
                return defaultValue;
            }
            string result = string.Empty;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            if (deserialized.ContainsKey(secretName))
            {
                return deserialized[secretName];
            }
            return defaultValue;
        }
    }
}
