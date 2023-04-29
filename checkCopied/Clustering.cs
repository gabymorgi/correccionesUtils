﻿using Newtonsoft.Json;

namespace checkCopied
{
    public class Cluster
    {
        public double Threshold { get; set; }
        public List<string> Members { get; set; }
        public List<Cluster> Clusters { get; set; }

        public Cluster(double threshold, List<string> members)
        {
            Threshold = threshold;
            Members = members;
            Clusters = new List<Cluster>();
        }
    }

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
        public static List<List<string>> HierarchicalClustering(Dictionary<string, Dictionary<string, double>> similarityDic, double threshold)
        {
            // Se asume que similarityDic representa una matriz de similarity, pero con índices nombrados
            // Por lo tanto, las claves del diccionario externo serán las mismas que las internas

            // Obtener un arreglo con las claves de los diccionarios
            var keys = new List<string>(similarityDic.Keys);
            // Inicializar clusters con cada proyecto como un cluster individual
            List<List<string>> clusters = keys.Select(key => new List<string> { key }).ToList();

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
                        double sim = AverageLinkage(similarityDic, clusters[i], clusters[j]);
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
        private static double AverageLinkage(Dictionary<string, Dictionary<string, double>> similarityMatrix, List<string> cluster1, List<string> cluster2)
        {
            double sim = 0;
            foreach (string p1 in cluster1)
            {
                foreach (string p2 in cluster2)
                {
                    sim += similarityMatrix[p1][p2];
                }
            }
            sim /= (cluster1.Count * cluster2.Count);
            return sim;
        }

        public static Cluster HierarchicalClusteringTree(Dictionary<string, Dictionary<string, double>> similarityDic, double thresholdStep = 0.1)
        {
            List<string> allMembers = new List<string>(similarityDic.Keys);
            Cluster rootNode = new Cluster(0, allMembers);
            BuildClusterTree(similarityDic, rootNode, thresholdStep);
            return rootNode;
        }

        private static void BuildClusterTree(Dictionary<string, Dictionary<string, double>> similarityDic, Cluster parentNode, double thresholdStep)
        {
            double currentThreshold = parentNode.Threshold + thresholdStep;

            List<List<string>> clusters = HierarchicalClustering(similarityDic, currentThreshold);

            // Si todos los elementos están en un único grupo, no creamos subgrupos
            if (clusters.Count == 1)
            {
                return;
            }

            // Si hay más de un grupo, los agregamos como subgrupos al nodo actual
            parentNode.Clusters = clusters.Select(cluster => new Cluster(currentThreshold, cluster)).ToList();

            // Llamamos a la función de forma recursiva en cada subgrupo
            foreach (Cluster childNode in parentNode.Clusters)
            {
                Dictionary<string, Dictionary<string, double>> subSimilarityDic = CreateSubSimilarityDic(similarityDic, childNode.Members);
                BuildClusterTree(subSimilarityDic, childNode, thresholdStep);
            }
        }

        private static Dictionary<string, Dictionary<string, double>> CreateSubSimilarityDic(Dictionary<string, Dictionary<string, double>> similarityDic, List<string> members)
        {
            return members.ToDictionary(
                member => member,
                member => members.ToDictionary(
                    otherMember => otherMember,
                    otherMember => similarityDic[member][otherMember]
                )
            );
        }

        public static void WriteClusterToJsonFile(Cluster cluster, string fileName)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(cluster, settings);
            File.WriteAllText(fileName, json);
        }
    }
}
