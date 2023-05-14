using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Prettier
    {
        public static string FindSrcFolder(string path)
        {
            string[] subdirectories = Directory.GetDirectories(path);

            foreach (string subdirectory in subdirectories)
            {
                string subdirectoryName = Path.GetFileName(subdirectory);
                //omit node_modules, public and .git
                if (subdirectoryName == "node_modules" || subdirectoryName == "public" || subdirectoryName == ".git")
                {
                    continue;
                }
                if (subdirectoryName == "src")
                {
                    return path;
                }

                string result = FindSrcFolder(subdirectory);
                if (result != "")
                {
                    return result;
                }
            }

            return "";
        }

        public static void MoveToRoot(string folderPath)
        {
            string srcPath = FindSrcFolder(folderPath);
            Console.WriteLine($"srcPath: {srcPath}");

            // Si la carpeta "src" no existe, busca recursivamente en las subcarpetas
            if (srcPath != "" && srcPath != folderPath)
            {
                // Mueve todos los archivos y carpetas al directorio raíz
                string[] files = Directory.GetFiles(srcPath);
                foreach (string file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var destination = Path.Combine(folderPath, fileName);
                    Console.WriteLine($"Moving {file} to {destination}");
                    if (File.Exists(destination))
                    {
                        File.Delete(destination);
                    }
                    File.Move(file, destination);
                }

                string[] dirs = Directory.GetDirectories(srcPath);
                foreach (string dir in dirs)
                {
                    if (Path.GetFileName(dir) == "node_modules")
                    {
                        continue;
                    }
                    string dirName = Path.GetFileName(dir);
                    var destination = Path.Combine(folderPath, dirName);
                    Console.WriteLine($"Moving {dir} to {destination}");
                    if (Directory.Exists(destination))
                    {
                        Directory.Delete(destination, true); // El segundo argumento 'true' permite eliminar el directorio de destino incluso si no está vacío.
                    }
                    Directory.Move(dir, destination);
                }

                // Elimina la carpeta "src"
                Console.WriteLine($"Deleting {srcPath}");
                Directory.Delete(srcPath);
            }
        }

        static void CreatePrettierIgnore(string folderPath)
        {
            // Ruta del archivo .prettierignore
            string ignoreFilePath = Path.Combine(folderPath, ".prettierignore");
            // Verificar si el archivo .prettierignore existe y, si no, crearlo con los patrones de carpetas a ignorar
            if (!File.Exists(ignoreFilePath))
            {
                string[] ignorePatterns = { "node_modules", "build", "dist", "public" };
                File.WriteAllLines(ignoreFilePath, ignorePatterns);
            }
        }

        static void CreatePrettierConfig(string folderPath)
        {
            // Ruta del archivo .prettierrc.json
            string configFilePath = Path.Combine(folderPath, ".prettierrc.json");
            // Verificar si el archivo .prettierrc.json existe y, si no, crearlo con la configuración de prettier
            if (!File.Exists(configFilePath))
            {
                string config = @"{
                    ""printWidth"": 120,
                    ""tabWidth"": 2,
                    ""semi"": false,
                    ""singleQuote"": true,
                    ""trailingComma"": ""all"",
                    ""quoteProps"": ""as-needed"",
                    ""jsxSingleQuote"": true,
                    ""bracketSameLine"": false
                }";
                File.WriteAllText(configFilePath, config);
            }
        }

        public static void RunPrettier(string folderPath, string npxPath)
        {
            // Argumentos para el comando de prettier
            string prettierArgs = $"npx prettier --write \"{folderPath}/src/**/*.{{js,jsx,ts,tsx}}\" --ignore-path \"{folderPath}\\.prettierignore\" --config \"{folderPath}\\.prettierrc.json\"";

            CreatePrettierIgnore(folderPath);
            CreatePrettierConfig(folderPath);

            // Crear el proceso para ejecutar el comando de prettier
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = npxPath,
                Arguments = prettierArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = new Process { StartInfo = startInfo };

            Console.WriteLine($"Running {startInfo.FileName} {startInfo.Arguments}");
            // Iniciar el proceso y esperar a que termine
            process.Start();
            process.WaitForExit(30000);

            // Leer la salida del comando de prettier
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Imprimir la salida y el error (si hay)
            Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("Error:");
                Console.WriteLine(error);
            }
        }

        public static void FormatSubfolders(Configuration config)
        {
            int totalSubfolders = config.ProjectFolders.Length;
            int doneSubfolders = 0;
            Console.WriteLine(config.ProjectFolders);

            // Iteramos sobre cada subdirectorio
            foreach (string folder in config.ProjectFolders)
            {
                Console.WriteLine($"Procesing {folder}");

                //remove all unnecessary sub folders
                MoveToRoot(folder);

                Console.WriteLine($"Running prettier on {folder}");
                // Ejecutamos runPrettier para el subdirectorio
                RunPrettier(folder, config.NpxPath);

                // Actualizamos el progreso
                doneSubfolders++;
                Console.WriteLine($"Processed {doneSubfolders} / {totalSubfolders} subfolders");
            }
        }
    }
}
