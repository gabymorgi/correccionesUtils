using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Comparator
    {
        [Obsolete("CompareStrings is deprecated, please use CompareStringsByTokens instead.")]
        private static double CompareStrings(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return 0;
            }
            int[,] scores = new int[str1.Length + 1, str2.Length + 1];
            int maxScore = 0;

            for (int i = 1; i <= str1.Length; i++)
            {
                for (int j = 1; j <= str2.Length; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        scores[i, j] = scores[i - 1, j - 1] + 1;

                        if (scores[i, j] > maxScore)
                        {
                            maxScore = scores[i, j];
                        }
                    }
                }
            }

            return (double)maxScore / Math.Max(str1.Length, str2.Length);
        }

        [Obsolete("CompareFiles is deprecated, please use CompareFileWithWindow instead.")]
        private static double CompareFiles(string path1, string path2)
        {
            string[] file1Lines = File.ReadAllLines(path1);
            string[] file2Lines = File.ReadAllLines(path2);
            int totalLines = Math.Min(file1Lines.Length, file2Lines.Length);
            double similaritySum = 0;

            for (int i = 0; i < totalLines; i++)
            {
                double similarity = CompareStringsByTokens(file1Lines[i], file2Lines[i]);
                similaritySum += similarity;
            }

            return similaritySum / totalLines;
        }

        public static List<string> Tokenize(string str)
        {
            List<string> tokens = new List<string>();

            // Separadores adicionales para JSX y JavaScript
            char[] separators = { '<', '>', '(', ')', '{', '}', '=', ' ' };

            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsWhiteSpace(c) || separators.Contains(c))
                {
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }

                    if (separators.Contains(c))
                    {
                        tokens.Add(c.ToString());
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }

            return tokens;
        }

        public static double CompareCharacters(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return 0;
            }

            int length1 = str1.Length;
            int length2 = str2.Length;
            int minLen = Math.Min(length1, length2);
            int maxLen = Math.Max(length1, length2);

            int distance = 0;

            for (int i = 0; i < minLen; i++)
            {
                if (str1[i] != str2[i])
                {
                    distance++;
                }
            }

            distance += maxLen - minLen;

            return 1 - ((double)distance / maxLen);
        }

        public static double CompareStringsByTokens(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            {
                return 0;
            }

            var tokens1 = Tokenize(str1.Trim()).ToList();
            var tokens2 = Tokenize(str2.Trim()).ToList();

            int maxLen = Math.Max(tokens1.Count, tokens2.Count);
            int matches = 0;

            for (int i = 0; i < maxLen; i++)
            {
                if (i < tokens1.Count && i < tokens2.Count)
                {
                    if (tokens1[i] == tokens2[i])
                    {
                        matches++;
                    }
                    else
                    {
                        double similarity = CompareCharacters(tokens1[i], tokens2[i]);
                        if (similarity >= 0.8)
                        {
                            matches++;
                        }
                    }
                }
            }

            return (double)matches / maxLen;
        }

        public static bool IsUninportantLine(string text)
        {
            return !text.TrimStart().StartsWith("//") && !string.IsNullOrEmpty(text);
        }

        public static double CompareFileWithWindow(string file1, string file2)
        {
            int windowSize = 5;


            string[] lines1 = File.ReadAllLines(file1).Where(IsUninportantLine).ToArray();
            string[] lines2 = File.ReadAllLines(file2).Where(IsUninportantLine).ToArray();
            double similarity = 0.0;
            int maxIndex1 = lines1.Length - 1;
            int maxIndex2 = lines2.Length - 1;
            int index1 = 0;
            int index2 = 0;

            while (index1 <= maxIndex1)// && index2 <= maxIndex2)
            {
                double maxSimilarity = 0.0;
                // defino la ventana de búsqueda
                int windowStart = Math.Max(0, index2 - windowSize);
                int windowEnd = Math.Min(maxIndex2, index2 + windowSize);

                for (int i = windowStart; i <= windowEnd; i++)
                {
                    double tempSimilarity = CompareStringsByTokens(lines1[index1], lines2[i]);

                    if (tempSimilarity > maxSimilarity)
                    {
                        maxSimilarity = tempSimilarity;
                    }
                    if (maxSimilarity > 0.9)
                    {
                        index2 = i;
                        break;
                    }
                }

                similarity += maxSimilarity;

                index1++;
                index2++;
            }

            return similarity / Math.Max(lines1.Length, lines2.Length);
        }

        public static List<string> GetProjectFiles(string projectPath)
        {
            List<string> files = new List<string>();
            string[] extensions = new string[] { ".js", ".jsx" };

            foreach (string extension in extensions)
            {
                files.AddRange(Directory.GetFiles($"{projectPath}/src", $"*{extension}", SearchOption.AllDirectories));
            }

            return files;
        }

        public static double CompareProjects(string projectPath1, string projectPath2)
        {
            var maxSimilarityThreshold = 0.85;
            var minSimilarityThreshold = 0.3;

            List<string> files1 = GetProjectFiles(projectPath1);
            List<string> files2 = GetProjectFiles(projectPath2);

            double totalSimilarity = 0.0;
            foreach (string file1 in files1)
            {
                double maxSimilarity = 0.0;
                string fileName = Path.GetFileName(file1);
                string file2 = files2.FirstOrDefault(f => Path.GetFileName(f) == fileName, "");

                if (file2 != "")
                {
                    double similarity = CompareFileWithWindow(file1, file2);
                    maxSimilarity = similarity;
                }
                else
                {
                    foreach (string f2 in files2)
                    {
                        double similarity = CompareFileWithWindow(file1, f2);
                        maxSimilarity = Math.Max(maxSimilarity, similarity);
                        if (maxSimilarity > 0.95)
                        {
                            break;
                        }
                    }
                }

                // clap the similarity
                if (maxSimilarity < minSimilarityThreshold)
                {
                    maxSimilarity = 0;
                }
                if (maxSimilarity > maxSimilarityThreshold)
                {
                    maxSimilarity = 1;
                }

                totalSimilarity += maxSimilarity;
            }

            double averageSimilarity = totalSimilarity / files1.Count;
            return averageSimilarity;
        }

        public static double[,] GetComparisionMatrix(string[] projectFolders)
        {
            int numProjects = projectFolders.Length;
            var similarityMatrix = new double[numProjects, numProjects];

            for (int i = 0; i < numProjects; i++)
            {
                for (int j = i; j < numProjects; j++)
                {
                    if (i == j)
                    {
                        similarityMatrix[i, j] = 1.0;
                    }
                    else
                    {
                        var project1Path = projectFolders[i];
                        var project2Path = projectFolders[j];
                        var similarity = CompareProjects(project1Path, project2Path);

                        similarityMatrix[i, j] = similarity;
                        similarityMatrix[j, i] = similarity;
                    }
                }
                Console.WriteLine($"Proyecto {i + 1} de {numProjects} procesado");
            }
            return similarityMatrix;
        }
    }
}
