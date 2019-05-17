using SolutionManagerUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionManagerUI.Utilities
{
    public static class SettingsManager
    {

        public static T GetSetting<T>(List<Setting> settings, string key, T defaultValue)
        {
            var setting = settings.FirstOrDefault(k => k.Key == key);
            if (setting != null)
            {
                var value = setting.Value;
                if (value != null)
                {
                    try
                    {
                        return ChangeType<T>(value);
                    }
                    catch (Exception)
                    {
                        //avoid throwing
                    }
                }
            }
            return defaultValue;
        }

        private static T ChangeType<T>(object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }
    }
}
