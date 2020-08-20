using GCodeRazor.Examples;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCodeRazor
{
    public static class Serializer
    {
        public static Settings GetSettings()
        {
            string settingsFilePath = GetSettingsFilePath();
            if (!File.Exists(settingsFilePath))
            {
                return CreatNewSettings();
            }

            try
            {
                StreamReader reader = new StreamReader(settingsFilePath);
                string settingsJson = reader.ReadToEnd();
                reader.Close();
                return JsonConvert.DeserializeObject<Settings>(settingsJson);
            }
            catch
            {
                return CreatNewSettings();
            }
        }

        private static Settings CreatNewSettings()
        {
            Settings settings = new Settings();
            settings.SetDefaults();
            string settingsJson = JsonConvert.SerializeObject(settings, Formatting.Indented);
            StreamWriter writer = new StreamWriter(GetSettingsFilePath(), false);
            writer.Write(settingsJson);
            writer.Close();
            return settings;
        }

        private static string GetSettingsFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");
        }

        private static string GetTempFolder()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return folderPath;
        }

        public static string SaveTemporaryFile(string content)
        {
            string temporaryFileName = Guid.NewGuid() + ".nc";
            string filePath = Path.Combine(GetTempFolder(), temporaryFileName);
            StreamWriter writer = new StreamWriter(filePath, false);
            writer.Write(content);
            writer.Close();
            return filePath;
        }

        public static IExampleControlModel[] GetExamples()
        {
            string examplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "examples.json");
            if (!File.Exists(examplePath))
            {
                IExampleControlModel[] examples = JsonConvert.DeserializeObject<IExampleControlModel[]>(GetDefaultExamplesJson());
                string json = JsonConvert.SerializeObject(examples, Formatting.Indented);
                StreamWriter writer = new StreamWriter(examplePath, false);
                int length = json.Length;
                writer.Write(json);
                writer.Close();
                return examples;
            }

            try
            {
                StreamReader reader = new StreamReader(examplePath);
                string jsonContent = reader.ReadToEnd();
                reader.Close();

                IExampleControlModel[] examplesArr = JsonConvert.DeserializeObject<IExampleControlModel[]>(jsonContent);
                return examplesArr;
            } catch
            {
            }

            return JsonConvert.DeserializeObject<IExampleControlModel[]>(GetDefaultExamplesJson());
        }

        public static string GetDefaultExamplesJson()
        {
            var resourceName = "GCodeRazor.Resources.examples.json";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
