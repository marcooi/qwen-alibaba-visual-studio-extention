using System;
using System.IO;
using Microsoft.VisualStudio.Shell;

namespace QwenAlibabaCodingPlan
{
    public class QwenSettings
    {
        private static string SettingsFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "QwenAlibabaCodingPlan",
            "settings.json");

        public string ApiKey { get; set; }
        public string ApiUrl { get; set; } = "https://coding-intl.dashscope.aliyuncs.com/apps/anthropic";
        public string Model { get; set; } = "qwen3.5-plus";

        public static QwenSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<QwenSettings>(json) ?? new QwenSettings();
                }
            }
            catch
            {
            }
            return new QwenSettings();
        }

        public void Save()
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
            }
        }
    }
}