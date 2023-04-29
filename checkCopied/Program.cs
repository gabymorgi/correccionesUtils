// See https://aka.ms/new-console-template for more information
using checkCopied;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();

Console.WriteLine("Hello, World!");

string mainFolder = @"D:\Projects\2023-04-finales";
double threshold = 0.70;

var projectFolders = Directory.GetDirectories(mainFolder);


//MailSender.SendEmail("gabymorgi@gmail.com", "Test", "Hola gabito");
//return 0;

//STEP 1
//make a csv with format: email, name, git
//get rid of ñ and accents
//Git.ClonarRepositorios(mainFolder, $"{mainFolder}/list.csv");

//STEP 2
//delete node_modules, public and .git folders
// NOT IMPLEMENTED YET

//STEP 3
//Run prettier on all files
//Prettier.FormatSubfolders(projectFolders); NOT IMPLEMENTED YET

//STEP 4
//var comparisionDic = Comparator.GetComparisionDic(projectFolders);
//Comparator.PrintComparisionDic(comparisionDic, $"{mainFolder}/comparisionDic.csv");
//var clusters = Clustering.HierarchicalClustering(comparisionDic, projectFolders, threshold);
//Clustering.PrintClusters(clusters);

//STEP 5 install projects
Git.InstallProjects(projectFolders);

stopwatch.Stop();
Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");

