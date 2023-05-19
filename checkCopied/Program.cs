// See https://aka.ms/new-console-template for more information
using checkCopied;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

Console.WriteLine("Welcome!");

var config = new Configuration();

var MenuOptions = new UserInterface.MenuOption[]
{
    new UserInterface.MenuOption{
        Value = "0",
        Name = "Change Main Folder",
        Action = () => config.ConfigureMainPath(),
    },
    new UserInterface.MenuOption{
        Value = "1",
        Name = "Configure",
        Action = () => config.CreateConfiguration(),
    },
    new UserInterface.MenuOption{
        Value = "2",
        Name = "Clonar repos",
        Action = () => {
            Git.ClonarRepositorios(config.ProjectsMainFolder);
            config.MainFolder = config.MainFolder; //force recalculate project repos
        },
    },
    new UserInterface.MenuOption{
        Value = "3",
        Name = "Run Prettier",
        Action = () => Prettier.FormatSubfolders(config),
    },
    new UserInterface.MenuOption{
        Value = "4",
        Name = "Run Comparator",
        Action = () => {
            try
            {
                var comparisionDic = Comparator.GetComparisionDic(config.ProjectFolders);
                Comparator.PrintComparisionDic(comparisionDic, $"{config.MainFolder}/comparisionDic.csv");
                Console.WriteLine($"Success: You can run clustering or find the comparision matrix at {config.MainFolder}/comparisionDic.csv");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        },
    },
    new UserInterface.MenuOption
    {
        Value = "5",
        Name = "Run Clustering",
        Action = () =>
        {
            try
            {
                var comparisionDic = Comparator.ReadComparisionDicFromCsv($"{config.MainFolder}/comparisionDic.csv");
                var clusters = Clustering.HierarchicalClusteringTree(comparisionDic, config.ThresholdStep);
                Clustering.WriteClusterToJsonFile(clusters, $"{config.MainFolder}/clusterTree.json");
                Console.WriteLine($"Success: You can find the cluster JSON at {config.MainFolder}/clusterTree.json");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        },
    },
    new UserInterface.MenuOption{
        Value = "6",
        Name = "Create monorepo",
        Action = () => {
            // yarn init -y
            // change package.json on every project so it has a unique name
            // yarn install
            Git.InitializeYarnAndConfigure(config.MainFolder, config.YarnPath);
            Git.RenameProjects(config.ProjectFolders);
            Git.YarnStart(config.MainFolder, config.YarnPath);
        },
    },
    new UserInterface.MenuOption
    {
        Value = "7",
        Name = "Run projects",
        Action = () =>
        {
            Console.WriteLine("Run de following command on your console to run each project");
            UserInterface.WriteLine("yarn workspace <project-name> run <command>", ConsoleColor.Green);
            if (File.Exists($"{config.MainFolder}/clusterTree.json")) {
                Console.WriteLine("Here you have the list ordered by the clusters");
                var clusters = Clustering.LoadClusterFromJsonFile($"{config.MainFolder}/clusterTree.json");
                var clusterList = Clustering.ClusterToList(clusters);
                foreach (var cluster in clusterList)
                {
                    Console.WriteLine($"yarn workspace {Git.GetValidPackageName(cluster)} run dev");
                }
            }
        }
    },
    new UserInterface.MenuOption
    {
        Value = "q",
        Name = "Quit",
        Action = () => Environment.Exit(0),
    },
};

while (true)
{
    UserInterface.PrintMenu(MenuOptions);
}


//MailSender.SendEmail("gabymorgi@gmail.com", "Test", "Hola gabito");
//return 0;

//DEPRECATED STEP 5 install projects
//Git.InstallProjects(projectFolders);
