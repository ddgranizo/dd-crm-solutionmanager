﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Utilities
{
   public static class StringFormatter
    {
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