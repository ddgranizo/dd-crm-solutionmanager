using SolutionManagerUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SolutionManagerUI.Utilities
{
    public static class AppDataManager
    {

        private const string ApplicationFolderSettings = "DDSolutionManagerUI";

        private const string Settings = "Settings.xaml";
        private const string SettingsBackup = "SettingsBackup.xaml";

        private const string ConnectionsFileSetting = "Connections.xaml";
        private const string ConnectionsFileSettingBackup = "ConnectionsBackup.xaml";


        public static void CreateAppDataPathIfNotExists()
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }

            if (!File.Exists(GetSettingsPath()))
            {
                SaveSettings(DefaultSettings.GetDefaultSettingSet());
            }

            if (!File.Exists(GetConnectionsFilePath()))
            {
                SaveConnections(DefaultSettings.GetDefaultConnectionSet());
            }
        }

        public static void SaveSettings(List<Setting> settings)
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }
            if (File.Exists(GetSettingsBackupPath()))
            {
                File.Delete(GetSettingsBackupPath());
            }
            if (File.Exists(GetSettingsPath()))
            {
                File.Move(GetSettingsPath(), GetSettingsBackupPath());
            }
            try
            {
                using (var writer = new System.IO.StreamWriter(GetSettingsPath()))
                {
                    var serializer = new XmlSerializer(typeof(List<Setting>));
                    serializer.Serialize(writer, settings);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                File.Move(GetSettingsPath(), GetSettingsBackupPath());
                throw;
            }
            if (File.Exists(GetSettingsBackupPath()))
            {
                File.Delete(GetSettingsBackupPath());
            }
        }


        public static void SaveConnections(List<CrmConnection> connections)
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }
            if (File.Exists(GetConnectionsFilePath()))
            {
                File.Move(GetConnectionsFilePath(), GetConnectionsBackupFilePath());
            }
            try
            {
                using (var writer = new System.IO.StreamWriter(GetConnectionsFilePath()))
                {
                    var serializer = new XmlSerializer(typeof(List<CrmConnection>));
                    serializer.Serialize(writer, connections);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                File.Move(GetConnectionsBackupFilePath(), GetConnectionsFilePath());
                throw;
            }
            if (File.Exists(GetConnectionsBackupFilePath()))
            {
                File.Delete(GetConnectionsBackupFilePath());
            }
        }


        public static List<CrmConnection> LoadConnections()
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }
            if (File.Exists(GetConnectionsFilePath()))
            {
                var content = File.ReadAllText(GetConnectionsFilePath());
                if (!string.IsNullOrEmpty(content))
                {
                    List<CrmConnection> orgs = null;
                    using (var stream = System.IO.File.OpenRead(GetConnectionsFilePath()))
                    {
                        var serializer = new XmlSerializer(typeof(List<CrmConnection>));
                        orgs = serializer.Deserialize(stream) as List<CrmConnection>;
                    }
                    return orgs;
                }
            }
            return new List<CrmConnection>();
        }


        public static List<Setting> LoadSettings()
        {
            if (!Directory.Exists(GetFolderPath()))
            {
                Directory.CreateDirectory(GetFolderPath());
            }

            if (File.Exists(GetSettingsPath()))
            {
                var content = File.ReadAllText(GetSettingsPath());
                if (!string.IsNullOrEmpty(content))
                {
                    List<Setting> config = null;
                    using (var stream = System.IO.File.OpenRead(GetSettingsPath()))
                    {
                        var serializer = new XmlSerializer(typeof(List<Setting>));
                        config = serializer.Deserialize(stream) as List<Setting>;
                    }
                    return config;
                }
            }
            return new List<Setting>();

        }
        

        

        private static string GetConnectionsBackupFilePath()
        {
            string basePath = GetFolderPath();
            return string.Format("{0}\\{1}", basePath, ConnectionsFileSettingBackup);
        }

        private static string GetSettingsPath()
        {
            string basePath = GetFolderPath();
            return string.Format("{0}\\{1}", basePath, Settings);
        }

        private static string GetSettingsBackupPath()
        {
            string basePath = GetFolderPath();
            return string.Format("{0}\\{1}", basePath, SettingsBackup);
        }

        private static string GetConnectionsFilePath()
        {
            string basePath = GetFolderPath();
            return string.Format("{0}\\{1}", basePath, ConnectionsFileSetting);
        }

        private static string GetFolderPath()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return string.Format("{0}\\{1}", basePath, ApplicationFolderSettings);
        }
    }
}
