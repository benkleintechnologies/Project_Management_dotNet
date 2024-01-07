namespace DalTest;
using DO;
using DalApi;
using System.Xml.Linq;
using System;

public static class Initialization
{
    private static IEngineer? s_dalEngineer; //stage 1
    private static IDependency? s_dalDependency; //stage 1
    private static ITask? s_dalTask; //stage 1

    private static readonly Random s_rand = new();

    public static void Do(IEngineer? dalEngineer, IDependency? dalDependecy, ITask? dalTask)
    {
        s_dalEngineer = dalEngineer ?? throw new NullReferenceException("DalEngineer cannot be null!");
        createEngineers();
        s_dalTask = dalTask ?? throw new NullReferenceException("DalTask cannot be null!");
        createTasks();
        s_dalDependency = dalDependecy ?? throw new NullReferenceException("DalDependency cannot be null!");
        createDependencies();
    }

    private static void createEngineers()
    {
        String[] engineerNames = {
            "Moishe Goldstein", "Shloimy Rosenberg", "Yitzy Schwartz", "Shmuly Cohen", "Dovid Greenbaum",
            "Shloime Rosenblatt", "Leiby Levine", "Yudi Abramowitz", "Mordy Eisenberg", "Yakov Fried",
            "Avrumy Stern", "Ahrele Goldman", "Tzvi Berger", "Yossi Adler", "Meir Silber", "Simchy Perlman",
            "Aryeh Feldman", "Zevy Klein", "Moshe Blum", "Yisroel Hochberg"
        };

        foreach (var engineerName in engineerNames)
        {
            int id; 
            do
                id = s_rand.Next(200000000, 400000000); 
            while (s_dalEngineer!.Read(id) != null);

            //randomaly choose experience level
            var values = Enum.GetValues(typeof(EngineerExperience));
            int randomIndex = s_rand.Next(values.Length);
            EngineerExperience engineerExperience = (EngineerExperience)values.GetValue(randomIndex);
            
            Engineer newEng = new(id, engineerName, null, s_rand.Next(80000, 200000), engineerExperience);
            s_dalEngineer!.Create(newEng);
        }

    }

    private static void createTasks()
    {
        String[] taskNames = {
            "Optimize Database Queries",
            "Implement Security Patch",
            "Design User Interface for Feature X",
            "Debug Performance Bottlenecks",
            "Integrate API for External Service",
            "Conduct Code Review for Module Y",
            "Create Automated Testing Suite",
            "Resolve Bug Reports from QA",
            "Implement Feature Z from Product Roadmap",
            "Optimize Memory Usage in Application",
            "Refactor Legacy Codebase",
            "Research and Evaluate New Technologies",
            "Build Scalable Infrastructure for Project",
            "Enhance Logging and Monitoring",
            "Collaborate on System Architecture",
            "Create Documentation for Codebase",
            "Implement Continuous Integration/Continuous Deployment",
            "Resolve Customer Support Tickets",
            "Optimize Frontend Performance",
            "Implement Machine Learning Algorithm",
            "Design and Implement RESTful API",
            "Perform Security Code Review",
            "Upgrade Frameworks and Libraries",
            "Build Proof of Concept for New Feature"
        };

        DateTime currentDate = DateTime.Now;

        foreach (String taskName in taskNames)
        {
            //randomaly choose difficulty level
            var values = Enum.GetValues(typeof(EngineerExperience));
            int randomIndex = s_rand.Next(values.Length);
            EngineerExperience difficultyLevel = (EngineerExperience)values.GetValue(randomIndex);

            //Choose random engineer with required experience
            List<Engineer> engineerList = new List<Engineer>();
            foreach (Engineer engineer in s_dalEngineer!.ReadAll().ToArray())
            {
                if (engineer.level >= difficultyLevel)
                {
                    engineerList.Add(engineer);
                };
            }
            int randomEngineerIndex = s_rand.Next(engineerList.Count);
            Engineer randomEngineer = engineerList[randomEngineerIndex];
            
            //Making each task a random length, starting when the previous one is supposed to finish
            DateTime dateCreated = DateTime.Now;
            DateTime projectedStartDate = currentDate;
            currentDate = currentDate.AddMonths(s_rand.Next(1, 5)).AddDays(s_rand.Next(0,28));
            DateTime deadline = currentDate;
            TimeSpan duration = deadline.Subtract(projectedStartDate);

            Task newTask = new(0, randomEngineer.id, s_rand.Next(2)==1, difficultyLevel, taskName, null, null, null, dateCreated, projectedStartDate, null, duration, deadline, null);
            s_dalTask!.Create(newTask);
        }
    }

    private static void createDependencies()
    {
        Task[] tasks = s_dalTask!.ReadAll().ToArray();
        int dependentTask = -1;
        int dependsOnTask = -1;
        for(int i = 1; i <= tasks.Length; i++) 
        { 
            if (i % 3 == 0)
            {
                //Make every third task dependent on the task 2 before in the list
                dependentTask = tasks[i].id;
                dependsOnTask = tasks[i-2].id;
                DateTime shippingDate = DateTime.Now.AddDays(s_rand.Next(1, 5));
                Dependency newDependency = new(0, dependentTask, dependsOnTask, null, null, DateTime.Now, shippingDate, null);
                
                //Make sure a dependency doesn't exist in s_dalDependency with the same dependentTask and dependsOnTask
                Dependency[] dependencies = s_dalDependency!.ReadAll().ToArray();
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
                    s_dalDependency!.Create(newDependency);
                }
            }
        }
        
    }
}
