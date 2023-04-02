using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Clustering
    {
        public static void PrintClusters(List<List<string>> clusters)
        {
            Console.WriteLine("Clusters:");
            Console.WriteLine("[");
            foreach (var cluster in clusters)
            {
                Console.WriteLine("\t[");
                foreach (var project in cluster)
                {
                    Console.WriteLine($"\t\t{project}");
                }
                Console.WriteLine("\t]");
            }
            Console.WriteLine("]");
        }
        public static List<List<string>> HierarchicalClustering(double[,] similarityMatrix, string[] projectNames, double threshold)
        {
            // Inicializar clusters con cada proyecto como un cluster individual
            List<List<string>> clusters = projectNames.Select((p) => new List<string> { p }).ToList();

            // Mientras haya más de un cluster, seguir agrupando
            while (clusters.Count > 1)
            {
                // Encontrar el par de clusters más similares
                double maxSim = 0;
                int maxI = 0, maxJ = 0;
                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        double sim = AverageLinkage(similarityMatrix, projectNames, clusters[i], clusters[j]);
                        if (sim > maxSim)
                        {
                            maxSim = sim;
                            maxI = i;
                            maxJ = j;
                        }
                    }
                }

                // Si la similitud es menor que el umbral, detener el clustering
                if (maxSim < threshold)
                    break;

                // Unir los clusters más similares
                List<string> newCluster = clusters[maxI].Concat(clusters[maxJ]).ToList();
                clusters.RemoveAt(maxJ);
                clusters.RemoveAt(maxI);
                clusters.Add(newCluster);
            }

            return clusters;
        }

        // Función para calcular la similitud promedio entre dos clusters usando linkage promedio
        private static double AverageLinkage(double[,] similarityMatrix, string[] projectNames, List<string> cluster1, List<string> cluster2)
        {
            double sim = 0;
            foreach (string p1 in cluster1)
            {
                foreach (string p2 in cluster2)
                {
                    int i = IndexOf(projectNames, p1);
                    int j = IndexOf(projectNames, p2);
                    sim += similarityMatrix[i, j];
                }
            }
            sim /= (cluster1.Count * cluster2.Count);
            return sim;
        }

        // Función auxiliar para encontrar el índice de un proyecto en la lista de nombres de proyectos
        private static int IndexOf(string[] projectNames, string projectName)
        {
            for (int i = 0; i < projectNames.Length; i++)
            {
                if (projectNames[i] == projectName)
                    return i;
            }
            return -1;
        }
    }
}
