using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Git
    {
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
                var nombreIntegrante = campos[1].Trim();
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
    }
}
