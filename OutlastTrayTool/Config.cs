using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace OutlastTrayTool
{
    public class Config
    {
        public string configPath;
        public string modFolderPath;
        public IConfigurationRoot configuration;

        public Config()
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;

            configPath = Path.Combine(appFolder, "config.json");
            modFolderPath = Path.Combine(appFolder, "Mods");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig();
            }

            if (!Directory.Exists(modFolderPath))
            {
                Directory.CreateDirectory(modFolderPath);
            }

            configuration = new ConfigurationBuilder()
                .AddJsonFile(configPath, optional: false, reloadOnChange: true)
                .Build();
        }

        private void CreateDefaultConfig()
        {
            string gamePath = @"C:\Program Files (x86)\Steam\steamapps\common\The Outlast Trials";

            string downloadsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Downloads"
            );

            if (!Directory.Exists(gamePath))
            {
                MessageBox.Show("Couldn't find game path. Please manually select your game folder. It will be named The Outlast Trials");

                while (true)
                {
                    using FolderBrowserDialog dialog = new FolderBrowserDialog();

                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    if (Path.GetFileName(dialog.SelectedPath) == "The Outlast Trials")
                    {
                        gamePath = dialog.SelectedPath;
                        break;
                    }

                    MessageBox.Show("Incorrect game folder selected, did you select the folder named The Outlast Trials?");
                }
            }

            var configObj = new
            {
                FOV = 90,
                screenPercentage = 100,
                fog = "Enabled",
                presence = "Disabled",
                startup = "Disabled",
                gamePath = gamePath,
                downloadsPath = downloadsPath,
                modMap = new Dictionary<string, object>()
            };

            string defaultConfig = JsonSerializer.Serialize(configObj, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(configPath, defaultConfig);
        }

        public void ChangeProperty(string propertyName, string value)
        {
            string configJson = File.ReadAllText(configPath);
            dynamic configJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(configJson);

            configJsonObj[propertyName] = value;

            string writtenOutput = Newtonsoft.Json.JsonConvert.SerializeObject(
                configJsonObj,
                Newtonsoft.Json.Formatting.Indented
            );

            File.WriteAllText(configPath, writtenOutput);
        }

        public void ChangeProperty(string propertyName, int value)
        {
            string configJson = File.ReadAllText(configPath);
            dynamic configJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(configJson);

            configJsonObj[propertyName] = value;

            string writtenOutput = Newtonsoft.Json.JsonConvert.SerializeObject(
                configJsonObj,
                Newtonsoft.Json.Formatting.Indented
            );

            File.WriteAllText(configPath, writtenOutput);
        }

        public void ChangeProperty(string propertyName, JObject value)
        {
            string configJson = File.ReadAllText(configPath);
            dynamic configJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(configJson);

            configJsonObj[propertyName] = value;

            string writtenOutput = Newtonsoft.Json.JsonConvert.SerializeObject(
                configJsonObj,
                Newtonsoft.Json.Formatting.Indented
            );

            File.WriteAllText(configPath, writtenOutput);
        }

        public dynamic LoadConfig()
        {
            string configJson = File.ReadAllText(configPath);
            dynamic configJsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(configJson);

            return configJsonObj;
        }
    }
}