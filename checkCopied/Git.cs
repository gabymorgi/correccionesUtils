using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Git
    {
        public static string SanitizeFileName(string input)
        {
            // Reemplazar caracteres acentuados y eñes por versiones sin acento y sin eñe.
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalized)
            {
                UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            string sanitized = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

            // Reemplazar caracteres problemáticos por guiones bajos.
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            return sanitized;
        }

        /**
         * se espera que el CSV tenga el formato
         * email, nombre, url
         */
        public static void ClonarRepositorios(string rutaDestino, string rutaCsv)
        {
            // Lee el archivo CSV
            var lineas = File.ReadAllLines(rutaCsv);

            foreach (var linea in lineas)
            {
                // Parsea la línea del CSV para obtener el nombre del integrante y la URL del repositorio
                var campos = linea.Split(',');
                var nombreIntegrante = SanitizeFileName(campos[1].Trim());
                var urlRepositorio = campos[2].Trim();

                try
                {
                    // Crea la carpeta en el destino
                    var rutaCarpeta = Path.Combine(rutaDestino, nombreIntegrante);
                    Directory.CreateDirectory(rutaCarpeta);

                    // Clona el repositorio en la carpeta
                    var proceso = new Process();
                    proceso.StartInfo.FileName = "git";
                    proceso.StartInfo.Arguments = $"clone {urlRepositorio} {rutaCarpeta}";
                    proceso.Start();
                    proceso.WaitForExit();

                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error en {nombreIntegrante}: {ex.Message}");
                }
            }
        }

        public static void InstallProjects(string[] subdirectories)
        {
            string npmPath = " C:\\Users\\gabym\\AppData\\Roaming\\npm\\npm.cmd";
            for(int i = 0; i < subdirectories.Length; i++)
            {
                Console.WriteLine($"Installing {subdirectories[i]} ({i + 1}/{subdirectories.Length})");
                try
                {
                    var proceso = new Process();
                    // Ejecuta "npm install" en la carpeta
                    proceso.StartInfo.FileName = npmPath;
                    proceso.StartInfo.Arguments = "install";
                    proceso.StartInfo.WorkingDirectory = subdirectories[i];
                    proceso.Start();
                    proceso.WaitForExit();
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static string GetValidPackageName(string folderName)
        {
            string lowerCaseName = folderName.ToLower();
            string validCharactersPattern = @"[^a-z0-9._-]+";
            string validPackageName = Regex.Replace(lowerCaseName, validCharactersPattern, "-");
            return validPackageName;
        }

        public static void renameProjects(string[] subdirectories)
        {
            foreach (var directory in subdirectories)
            {
                string packageJsonPath = Path.Combine(directory, "package.json");

                if (File.Exists(packageJsonPath))
                {
                    string packageJsonContent = File.ReadAllText(packageJsonPath);
                    JObject packageJson = JObject.Parse(packageJsonContent);

                    string folderName = GetValidPackageName(new DirectoryInfo(directory).Name);
                    packageJson["name"] = folderName;

                    File.WriteAllText(packageJsonPath, packageJson.ToString());
                    Console.WriteLine($"Updated package.json for project {folderName}");
                }
                else
                {
                    Console.WriteLine($"package.json not found in {directory}");
                }
            }
        }

        public static void InitializeYarnAndConfigure(string folderPath)
        {
            // Ejecuta `yarn init`
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    //C:\\Users\\gabym\\AppData\\Roaming\\npm\\node_modules\\yarn\\bin
                    //C:/Users/gabym/AppData/Local/Yarn/bin/yarn.cmd
                    //"C:\Users\gabym\AppData\Roaming\npm\node_modules\yarn\bin\yarn.cmd"
                    FileName = @"C:\Users\gabym\AppData\Roaming\npm\node_modules\yarn\bin\yarn.cmd",
                    Arguments = "init -y",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = folderPath
                }
            };
            process.Start();
            process.WaitForExit();

            // Edita el package.json con la configuración requerida
            string packageJsonPath = Path.Combine(folderPath, "package.json");
            string packageJsonContent = File.ReadAllText(packageJsonPath);
            JObject packageJson = JObject.Parse(packageJsonContent);

            packageJson["name"] = "react-monorepo";
            packageJson["private"] = true;
            packageJson["workspaces"] = JArray.Parse("[\"projects/*\"]");

            File.WriteAllText(packageJsonPath, packageJson.ToString());
        }
    }
}
