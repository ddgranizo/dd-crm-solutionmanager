using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Utilities
{
   public static class StringFormatter
    {


        public static string GetPathWithLastSlash(string path)
        {
            if (path.Last() != '\\' && path.Last() != '/')
            {
                path = string.Format("{0}\\", path);
            }

            return path;
        }

        public static string GetSolutionFileName(string uniqueName, string version, bool managed)
        {
            string managedStr = managed ? "_managed" : string.Empty;
            string fileName = $"{uniqueName}_{string.Join("_", version.Split('.'))}{managedStr}.zip";
            return fileName;
        }

        public static string FormatString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

                if (unicodeCategory == UnicodeCategory.DecimalDigitNumber
                    || unicodeCategory == UnicodeCategory.LowercaseLetter
                    || unicodeCategory == UnicodeCategory.UppercaseLetter
                    || c == '_')
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
