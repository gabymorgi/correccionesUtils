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
        public void ClonarRepositorios(string rutaDestino, string rutaCsv)
        {
            // Lee el archivo CSV
            var lineas = File.ReadAllLines(rutaCsv);

            foreach (var linea in lineas)
            {
                // Parsea la línea del CSV para obtener el nombre del integrante y la URL del repositorio
                var campos = linea.Split(',');
                var nombreIntegrante = campos[0].Trim();
                var urlRepositorio = campos[1].Trim();

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

        public void InstallProjects(string rootPath)
        {
            string[] subdirectories = Directory.GetDirectories(rootPath);
            foreach (string subdirectory in subdirectories)
            {
                try
                {
                    var proceso = new Process();
                    // Ejecuta "npm install" en la carpeta
                    proceso.StartInfo.FileName = "npm";
                    proceso.StartInfo.Arguments = "install";
                    proceso.StartInfo.WorkingDirectory = subdirectory;
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
