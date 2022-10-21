using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class Lib
    {
        public static Dictionary<string, string> ReadAllSettings()
        {
            Dictionary<string, string> settings = new();
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    Console.Error.WriteLine("AppSettings is empty.");
                    return settings;
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        settings.Add(key, appSettings[key]);
                    }
                }
                return settings;
            }
            catch (ConfigurationErrorsException)
            {
                Console.Error.WriteLine("Error reading app settings");
                return settings;
            }
        }

        public static string? ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string? result = appSettings[key] ?? null;
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.Error.WriteLine("Error reading app settings");
                return null;
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.Error.WriteLine("Error writing app settings");
            }
        }
    }
}
