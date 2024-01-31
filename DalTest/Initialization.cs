namespace DalTest;
using DO;
using DalApi;
using System;
using Dal;

/// <summary>
/// Class to initialize data layer with "random" data
/// </summary>
public static class Initialization
{
    private static IDal? s_dal;

    private static readonly Random s_rand = new();

    public static void Do() 
    {
        //s_dal = dal ?? throw new NullReferenceException("Dal cannot be null!");
        s_dal = DalApi.Factory.Get;
        createConfig();
        createEngineers();
        createTasks();
        createDependencies();
    }

    /// <summary>
    /// Set random start and end date for the project
    /// </summary>
    private static void createConfig()
    {
        DateTime startDate = DateTime.Now.AddDays(s_rand.Next(1,28));
        DateTime endDate = DateTime.Now.AddMonths(s_rand.Next(6,12));
        s_dal!.Config.SetStartDate(startDate);
        s_dal!.Config.SetEndDate(endDate);
    }

    /// <summary>
    /// Create 5 engineers "randomly"
    /// </summary>
    private static void createEngineers()
    {
        String[] engineerNames = {
            "Moishe Goldstein", "Shloimy Rosenberg", "Yitzy Schwartz", "Shmuly Cohen", "Dovid Greenbaum"
        };

        string[] engineerEmails = {
            "moishe.goldstein@example.com", "shloimy.rosenberg@example.com", "yitzy.schwartz@example.com",
            "shmuly.cohen@example.com", "dovid.greenbaum@example.com"
        };


        for (int i=0; i < engineerNames.Length; i++)
        {
            //Pick unused Engineer ID
            int id = s_rand.Next(200000000, 400000000); 
            while (true)
            {
                try
                {
                    s_dal!.Engineer.Read(id); //Try to read Engineer with _id. Will throw error if it doesn't exist
                    id = s_rand.Next(200000000, 400000000);
                }
                catch (Exception ex)
                {
                    //Unused ID Found. Continue...
                    break;
                }
            }

            //randomly choose experience level
            var values = Enum.GetValues(typeof(EngineerExperience));
            int randomIndex = s_rand.Next(values.Length);
            EngineerExperience engineerExperience = (EngineerExperience)values.GetValue(randomIndex);
            
            Engineer newEng = new(id, engineerNames[i], engineerEmails[i], s_rand.Next(80000, 200000), engineerExperience);
            s_dal!.Engineer.Create(newEng);
        }

    }

    /// <summary>
    /// Create 20 Tasks "randomly" between the start and end date of the project
    /// </summary>
    private static void createTasks()
    {
        for (int i = 0; i<20; i++)
        {
            //randomly choose difficulty level
            var values = Enum.GetValues(typeof(EngineerExperience));
            int randomIndex = s_rand.Next(values.Length);
            EngineerExperience _difficultyLevel = (EngineerExperience)values.GetValue(randomIndex);
            
            //Making each task a random length
            TimeSpan toSubtract = TimeSpan.FromDays(s_rand.Next(1,28));
            DateTime dateCreated = DateTime.Now - toSubtract;
            DateTime projectedStartDate = DateTime.Now.AddMonths(s_rand.Next(1, 5)).AddDays(s_rand.Next(0,28));
            DateTime deadline = projectedStartDate.AddMonths(s_rand.Next(1, 5)).AddDays(s_rand.Next(0, 28));
            TimeSpan duration = deadline.Subtract(projectedStartDate);

            Task newTask = new(0, false, _difficultyLevel, null, null, null, null, null, dateCreated, projectedStartDate, null, duration, deadline, null);
            s_dal!.Task.Create(newTask);
        }
    }

    /// <summary>
    /// Create 40 "random" dependencies including some with multiple dependencies, 
    /// avoiding duplicates and circular dependencies
    /// </summary>
    private static void createDependencies()
    {
        Task?[] tasks = s_dal!.Task.ReadAll().ToArray();

        //Create cases of multiple dependencies, and where two different tasks have the same dependencies
        int dependentTask1 = tasks[0]!.id;
        int dependentTask2 = tasks[1]!.id;
        int dependsOnTask1 = tasks[2]!.id;
        int dependsOnTask2 = tasks[3]!.id;
        Dependency dependency1 = new(0, dependentTask1, dependsOnTask1);
        Dependency dependency2 = new(0, dependentTask1, dependsOnTask2);
        Dependency dependency3 = new(0, dependentTask2, dependsOnTask1);
        Dependency dependency4 = new(0, dependentTask2, dependsOnTask2);
        s_dal!.Dependency.Create(dependency1);
        s_dal!.Dependency.Create(dependency2);
        s_dal!.Dependency.Create(dependency3);
        s_dal!.Dependency.Create(dependency4);

        //Generate random dependencies
        for (int i = 0; i <= 36; i++) 
        { 
           
            //Choose random tasks to be dependent on each other
            int dependentTask = tasks[s_rand.Next(0, tasks.Length)]!.id;
            int dependsOnTask = tasks[s_rand.Next(0, tasks.Length)]!.id;

            //Make sure the dependency we are creating does not create a circular dependency
            while (createsCircularDependency(dependentTask, dependsOnTask))
            {
                dependsOnTask = tasks[s_rand.Next(0, tasks.Length)]!.id;
            }

            Dependency newDependency = new(0, dependentTask, dependsOnTask);

            //Make sure a dependency doesn't exist in s_dalDependency with the same dependentTask and dependsOnTask
            Dependency[] dependencies = s_dal!.Dependency.ReadAll().ToArray();
            bool existsAlready = false;
            foreach(Dependency dependency in dependencies)
            {
                if (dependency.dependentTask == dependentTask && dependency.dependsOnTask == dependsOnTask) 
                {
                    existsAlready = true;
                    break;
                }
            }
            if (!existsAlready)
            {
                s_dal!.Dependency.Create(newDependency);
            }
        }
    }

    /// <summary>
    /// Figure out if the dependentTask and DependsOnTask will create a circular dependency 
    /// </summary>
    /// <param name="dependentTask"></param>
    /// <param name="dependsOnTask"></param>
    /// <returns>If the dependency is circular</returns>
    private static bool createsCircularDependency(int dependentTask, int dependsOnTask)
    {
        // Check if dependsOnTask is directly or indirectly dependent on dependentTask
        return isIndirectlyDependent(dependentTask, dependsOnTask, new List<int>());
    }

    /// <summary>
    /// Recursively check if there is a direct or indirect circular dependency
    /// </summary>
    /// <param name="targetTask"></param>
    /// <param name="currentTask"></param>
    /// <param name="visitedTasks"></param>
    /// <returns>True if circular dependency found</returns>
    private static bool isIndirectlyDependent(int targetTask, int currentTask, List<int> visitedTasks)
    {
        if (visitedTasks.Contains(currentTask))
        {
            // We've encountered a task we've already visited in this recursion branch, indicating a circular dependency
            return true;
        }

        visitedTasks.Add(currentTask);

        Dependency[] dependencies = s_dal!.Dependency.ReadAll().Where(d => d.dependentTask == currentTask).ToArray();

        foreach (Dependency dependency in dependencies)
        {
            if (dependency.dependsOnTask == targetTask || isIndirectlyDependent(targetTask, dependency.dependsOnTask, visitedTasks))
            {
                return true;
            }
        }

        visitedTasks.Remove(currentTask); // Backtrack when moving to the next task
        return false;
    }
}
