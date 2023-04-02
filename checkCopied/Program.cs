// See https://aka.ms/new-console-template for more information
using checkCopied;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;



Console.WriteLine("Hello, World!");

string mainFolder = @"D:\Projects\2022-12-entregas";
double threshold = 0.70;

//var file1 = @"D:\Projects\2022-12-entregas\AlejandroLopez_MauricioAndresAbrilVilladiego\src\App.jsx";
//var file2 = @"D:\Projects\2022-12-entregas\AndresFelipeVelezPassos\src\App.jsx";
//var asdf = Comparator.CompareFileWithWindow(file1, file2);
//Console.WriteLine(asdf);

MailSender.SendEmail("gabymorgi@gmail.com", "Test", "Hola gabito");
return 0;


Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

var projectFolders = Directory.GetDirectories(mainFolder);

Prettier.FormatSubfolders(projectFolders);
var comparisionMatrix = Comparator.GetComparisionMatrix(projectFolders);
var clusters = Clustering.HierarchicalClustering(comparisionMatrix, projectFolders, threshold);
Clustering.PrintClusters(clusters);

stopwatch.Stop();
Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");

