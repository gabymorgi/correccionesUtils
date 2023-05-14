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

        public static List<(string, string)> getRepoList()
        {
            
            Error:
            Console.WriteLine("Ingrese la ruta al csv con la lista de alumnos:");
            UserInterface.WriteLine("Debe tener un header con las columnas 'nombre' y 'url'", ConsoleColor.Yellow);

            string? rutaCSV = Console.ReadLine();

            if (string.IsNullOrEmpty(rutaCSV))
            {
                UserInterface.WriteLine("La ruta no puede estar vacia", ConsoleColor.Red);
                goto Error;
            }

            if (!File.Exists(rutaCSV))
            {
                UserInterface.WriteLine("El archivo no existe", ConsoleColor.Red);
                goto Error;
            }

            var lineas = File.ReadAllLines(rutaCSV);
            int indexName = -1;
            int indexURL = -1;
            if (lineas.Length > 0)
            {
                var headerFields = lineas[0].Split(',');

                for (int i = 0; i < headerFields.Length; i++)
                {
                    //Console.WriteLine(headerFields[i]);
                    if (headerFields[i].ToLower() == "nombre") indexName = i;
                    if (headerFields[i].ToLower() == "url") indexURL = i;
                }
            }
            if (lineas.Length == 0 || indexName < 0 || indexURL < 0)
            {
                UserInterface.WriteLine("Header values not found", ConsoleColor.Red);
                goto Error;
            }

            var repoList = new List<(string, string)>();

            for (int i = 1; i < lineas.Length; i++)
            {
                var fields = lineas[i].Split(",");
                repoList.Add((SanitizeFileName(fields[indexName].Trim()), fields[indexURL].Trim()));
            }

            return repoList;
        }

        public static void ClonarRepositorios(string rutaDestino)
        {
            // Lee el archivo CSV
            var repoList = getRepoList();
            var errors = new List<(string, string)>();

            Directory.CreateDirectory(rutaDestino);
            int successRepos = 0;
            int errorRepos = 0;
            int totalRepos = repoList.Count;

            foreach (var repo in repoList)
            {
                var nombreIntegrante = repo.Item1;
                var urlRepositorio = repo.Item2;

                try
                {
                    // Crea la carpeta en el destino
                    var rutaCarpeta = Path.Combine(rutaDestino, nombreIntegrante);
                    Directory.CreateDirectory(rutaCarpeta);

                    // Clona el repositorio en la carpeta
                    var proceso = new Process();
                    proceso.StartInfo.FileName = "git";
                    proceso.StartInfo.Arguments = $"clone {urlRepositorio} {rutaCarpeta}";
                    proceso.StartInfo.UseShellExecute = false;
                    proceso.StartInfo.RedirectStandardError = true;
                    proceso.Start();
                    string errorOutput = proceso.StandardError.ReadToEnd();
                    proceso.WaitForExit();

                    if (proceso.ExitCode == 0)
                    {
                        successRepos++;
                        UserInterface.WriteLine($"Clonado {nombreIntegrante}: ({successRepos + errorRepos}/{totalRepos})", ConsoleColor.Green);
                    } else
                    {
                        errorRepos++;
                        errors.Add((nombreIntegrante, errorOutput));
                        UserInterface.WriteLine($"Error {nombreIntegrante}: ({successRepos + errorRepos}/{totalRepos})", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    errorRepos++;
                    errors.Add((nombreIntegrante, ex.Message));
                    UserInterface.WriteLine($"Error {nombreIntegrante}: ({successRepos + errorRepos}/{totalRepos})", ConsoleColor.Red);
                }
                
            }
            if (errors.Count > 0)
            {
                UserInterface.WriteLine("No se pudieron clonar los siguientes repos, revisalos manualmente", ConsoleColor.Red);
                foreach (var error in errors)
                {
                    UserInterface.WriteLine($"{error.Item1}: {error.Item2}", ConsoleColor.Red);
                }
            }
        }

        // deprecated
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

        public static void RenameProjects(string[] subdirectories)
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

        public static void InitializeYarnAndConfigure(string folderPath, string yarnPath)
        {
            // Ejecuta `yarn init`
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = yarnPath,
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

        public static void YarnStart(string folderPath, string yarnPath)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = yarnPath,
                    Arguments = "install",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = folderPath
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
