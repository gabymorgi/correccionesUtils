using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace checkCopied
{
    internal class Subtitles
    {
        public static void main()
        {
            string inputFilePath = @"D:\Descargas\JDL\Makkhi (Eega) New Released Hindi Dubbed Full Movie _ Sudeep, Nani, Samantha _ Latest Hindi Movies\Makkhi.srt";
            string outputFilePath = @"D:\Descargas\JDL\Makkhi (Eega) New Released Hindi Dubbed Full Movie _ Sudeep, Nani, Samantha _ Latest Hindi Movies\Makkhi-adj.srt";
            int offsetInMiliseconds = -50000;

            AdjustSubtitles(inputFilePath, outputFilePath, offsetInMiliseconds);

        }

        static void AdjustSubtitles(string inputFilePath, string outputFilePath, int offsetInMiliseconds)
        {
            var regex = new Regex(@"(\d{2}:\d{2}:\d{2},\d{3})");

            using (var reader = new StreamReader(inputFilePath))
            using (var writer = new StreamWriter(outputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (regex.IsMatch(line))
                    {
                        line = regex.Replace(line, match => AdjustTimestamp(match.Value, offsetInMiliseconds));
                    }
                    writer.WriteLine(line);
                }
            }
        }

        static string AdjustTimestamp(string timestamp, int offsetInMiliseconds)
        {
            TimeSpan time = TimeSpan.ParseExact(timestamp, @"hh\:mm\:ss\,fff", null);
            time = time.Add(TimeSpan.FromMilliseconds(offsetInMiliseconds));
            return time.ToString(@"hh\:mm\:ss\,fff");
        }

        
    }
}
