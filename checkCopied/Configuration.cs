using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class ConfigurationObject
    {
        public string? MainFolder { get; set; }
        public double ThresholdStep { get; set; }
        public string? NpxPath { get; set; }
        public string? YarnPath { get; set; }
        
    }

    internal class Configuration
    {
        private const string CONFIG_FILE_PATH = "config.json";
        private string mainFolder = string.Empty;
        public string MainFolder
        {
            get
            {
                return mainFolder;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    ProjectsMainFolder = Path.Combine(value, "projects");
                    if (!Directory.Exists(ProjectsMainFolder))
                    {
                        Directory.CreateDirectory(ProjectsMainFolder);
                    }
                    ProjectFolders = Directory.GetDirectories(ProjectsMainFolder);
                }
                mainFolder = value;
            }
        }
        public string ProjectsMainFolder { get; set; } = string.Empty;
        public string[] ProjectFolders { get; set; } = Array.Empty<string>();
        public double ThresholdStep { get; set; }
        public string NpxPath { get; set; }
        public string YarnPath { get; set; }

        public Configuration()
        {
            MainFolder = "";
            ThresholdStep = 0.05;
            NpxPath = "";
            YarnPath = "";
            LoadFromFile(CONFIG_FILE_PATH);
        }

        public void SaveToFile()
        {
            string json = JsonConvert.SerializeObject(new ConfigurationObject() {
                MainFolder = MainFolder,
                NpxPath = NpxPath,
                ThresholdStep = ThresholdStep,
                YarnPath = YarnPath
            }, Formatting.Indented);
            File.WriteAllText(CONFIG_FILE_PATH, json);
        }

        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                CreateConfiguration();
                return;
            }

            string json = File.ReadAllText(filePath);
            var deserialized = JsonConvert.DeserializeObject<ConfigurationObject>(json);
            if (deserialized == null)
            {
                CreateConfiguration();
                return;
            }
            MainFolder = deserialized.MainFolder ?? string.Empty;
            ThresholdStep = deserialized.ThresholdStep;
            NpxPath = deserialized.NpxPath ?? string.Empty;
            YarnPath = deserialized.YarnPath ?? string.Empty;
            
        }

        public void CreateConfiguration()
        {
            ConfigureNpxPath();
            ConfigureYarnPath();
            ConfigureThresholdStep();
            ConfigureMainPath();
            SaveToFile();
        }

        public void ConfigureNpxPath()
        {
            Console.WriteLine("Ingrese la ruta a npm:");
            UserInterface.WriteLine(@"Ejemplo: C:/Users/gabym/AppData/Roaming/npm/npx.cmd", ConsoleColor.Cyan);
            if (NpxPath != "")
            {
                UserInterface.WriteLine($"(actual value) {NpxPath}", ConsoleColor.Green);
            }

            string newPath;
            do
            {
                newPath = Console.ReadLine() ?? NpxPath;

            } while(!IsValidNpxPath(newPath));

            NpxPath = newPath;
        }

        public static bool IsValidNpxPath(string npxPath)
        {
            if (!File.Exists(npxPath))
            {
                Console.WriteLine("La ruta ingresada no es válida. Intente de nuevo.");
                return false;
            }

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = npxPath;
                process.StartInfo.Arguments = "npx -v";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Versión de npx: {output}");
                }
                else
                {
                    Console.WriteLine("No se pudo obtener la versión de npx. Verifique la ruta a npx e intente de nuevo.");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error al ejecutar npx. Verifique la ruta a npx e intente de nuevo.");
                return false;
            }
            return true;
        }

        public void ConfigureYarnPath()
        {
            Console.WriteLine("Ingrese la ruta a yarn:");
            UserInterface.WriteLine(@"Ejemplo: C:/Users/gabym/AppData/Roaming/npm/node_modules/yarn/bin/yarn.cmd", ConsoleColor.Cyan);
            if (YarnPath != "")
            {
                UserInterface.WriteLine($"(actual value) {YarnPath}", ConsoleColor.Green);
            }

            string newPath;
            do
            {
                newPath = Console.ReadLine() ?? YarnPath;

            } while (!IsValidYarnPath(newPath));

            YarnPath = newPath;
        }

        public static bool IsValidYarnPath(string YarnPath)
        {
            if (!File.Exists(YarnPath))
            {
                Console.WriteLine("La ruta ingresada no es válida. Intente de nuevo.");
                return false;
            }

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = YarnPath;
                process.StartInfo.Arguments = "-v";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Versión de yarn: {output}");
                }
                else
                {
                    Console.WriteLine("No se pudo obtener la versión de yarn. Verifique la ruta a yarn e intente de nuevo.");
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error al ejecutar yarn. Verifique la ruta a yarn e intente de nuevo.");
                return false;
            }
            return true;
        }

        public void ConfigureThresholdStep()
        {
            double thresholdStep;
            string newThreshold;
            do
            {
                Console.WriteLine("Ingrese el threshold step (0 - 1)");
                UserInterface.WriteLine(@"Ejemplo: 0.05", ConsoleColor.Cyan);

                UserInterface.WriteLine($"(actual value) {ThresholdStep}", ConsoleColor.Green);
                newThreshold = Console.ReadLine() ?? ThresholdStep.ToString();
                if (newThreshold == "") newThreshold = ThresholdStep.ToString();
            } while (!double.TryParse(newThreshold, out thresholdStep) || thresholdStep < 0 || thresholdStep > 1);

            ThresholdStep = thresholdStep;
        }

        public void ConfigureMainPath()
        {
            Error:
            Console.WriteLine("Ingrese la ruta principal:");
            UserInterface.WriteLine(@"D:\Projects\2023-04-finales", ConsoleColor.Cyan);
            if (MainFolder != "")
            {
                UserInterface.WriteLine($"(actual value) {MainFolder}", ConsoleColor.Green);
            }

            string newFolder = Console.ReadLine() ?? MainFolder;

            if (!Directory.Exists(newFolder))
            {
                try
                {
                    Directory.CreateDirectory(newFolder);
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    goto Error;
                }
            }

            MainFolder = newFolder;
        }
    }
}
