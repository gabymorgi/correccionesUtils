// See https://aka.ms/new-console-template for more information
using checkCopied;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Console.WriteLine("Hello, World!");

string mainFolder = @"D:\Projects\2023-04-finales\react-monorepo\projects";
double thresholdStep = 0.05;

var projectFolders = Directory.GetDirectories(mainFolder);


//MailSender.SendEmail("gabymorgi@gmail.com", "Test", "Hola gabito");
//return 0;

//STEP 1
//make a csv with format: email, name, git
//Git.ClonarRepositorios(mainFolder, $"{mainFolder}/list.csv");

//STEP 2
//delete node_modules and .git folders
// NOT IMPLEMENTED YET

//STEP 3
//Run prettier on all files
//Prettier.FormatSubfolders(projectFolders);

//STEP 4
//var comparisionDic = Comparator.GetComparisionDic(projectFolders);
//Comparator.PrintComparisionDic(comparisionDic, $"{mainFolder}/comparisionDic.csv");
//var comparisionDic = Comparator.ReadComparisionDicFromCsv($"{mainFolder}/comparisionDic.csv");
//var clusters = Clustering.HierarchicalClusteringTree(comparisionDic, thresholdStep);
//Clustering.WriteClusterToJsonFile(clusters, $"{mainFolder}/clusterTree.json");

//DEPRECATED STEP 5 install projects
//Git.InstallProjects(projectFolders);

//STEP 6 monorepo
Git.InitializeYarnAndConfigure(@"D:\Projects\2023-04-finales\react-monorepo");
//Git.renameProjects(projectFolders);

stopwatch.Stop();
Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");

