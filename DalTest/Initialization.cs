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
    private static IDal? s_Dal; // stage 2

    private static readonly Random s_rand = new();

    public static void Do(IDal dal) // stage 2
    {
        s_Dal = dal ?? throw new NullReferenceException("Dal cannot be null!"); // stage 2
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
        DateTime _startDate = DateTime.Now.AddDays(s_rand.Next(1,28));
        DateTime _endDate = DateTime.Now.AddMonths(s_rand.Next(6,12));
        s_Dal!.Config.setStartDate(_startDate);
        s_Dal!.Config.setEndDate(_endDate);
    }

    /// <summary>
    /// Create 5 engineers "randomally"
    /// </summary>
    private static void createEngineers()
    {
        String[] _engineerNames = {
            "Moishe Goldstein", "Shloimy Rosenberg", "Yitzy Schwartz", "Shmuly Cohen", "Dovid Greenbaum"
        };

        string[] _engineerEmails = {
            "moishe.goldstein@example.com", "shloimy.rosenberg@example.com", "yitzy.schwartz@example.com",
            "shmuly.cohen@example.com", "dovid.greenbaum@example.com"
        };


        for (int i=0; i < _engineerNames.Length; i++)
        {
            int _id; 
            do
                _id = s_rand.Next(200000000, 400000000); 
            while (s_Dal!.Engineer.Read(_id) != null);

            //randomaly choose experience level
            var _values = Enum.GetValues(typeof(EngineerExperience));
            int _randomIndex = s_rand.Next(_values.Length);
            EngineerExperience _engineerExperience = (EngineerExperience)_values.GetValue(_randomIndex);
            
            Engineer _newEng = new(_id, _engineerNames[i], _engineerEmails[i], s_rand.Next(80000, 200000), _engineerExperience);
            s_Dal!.Engineer.Create(_newEng);
        }

    }

    /// <summary>
    /// Create 20 Tasks "radomally" between the start and end date of the project
    /// </summary>
    private static void createTasks()
    {
        for (int i = 0; i<20; i++)
        {
            //randomaly choose difficulty level
            var _values = Enum.GetValues(typeof(EngineerExperience));
            int _randomIndex = s_rand.Next(_values.Length);
            EngineerExperience _difficultyLevel = (EngineerExperience)_values.GetValue(_randomIndex);
            
            //Making each task a random length
            TimeSpan _toSubtract = TimeSpan.FromDays(s_rand.Next(1,28));
            DateTime _dateCreated = DateTime.Now - _toSubtract;
            DateTime _projectedStartDate = DateTime.Now.AddMonths(s_rand.Next(1, 5)).AddDays(s_rand.Next(0,28));
            DateTime _deadline = _projectedStartDate.AddMonths(s_rand.Next(1, 5)).AddDays(s_rand.Next(0, 28));
            TimeSpan _duration = _deadline.Subtract(_projectedStartDate);

            Task _newTask = new(0, false, _difficultyLevel, null, null, null, null, null, _dateCreated, _projectedStartDate, null, _duration, _deadline, null);
            s_Dal!.Task.Create(_newTask);
        }
    }

    /// <summary>
    /// Create 40 "radom" dependencies including some with multiple dependencies, 
    /// avoiding duplicates and circular dependencies
    /// </summary>
    private static void createDependencies()
    {
        Task[] _tasks = s_Dal!.Task.ReadAll().ToArray();

        //Create cases of multiple dependencies, and where two different tasks have the same dependencies
        int _dependentTask1 = _tasks[0].id;
        int _dependentTask2 = _tasks[1].id;
        int _dependsOnTask1 = _tasks[2].id;
        int _dependsOnTask2 = _tasks[3].id;
        Dependency _dependency1 = new(0, _dependentTask1, _dependsOnTask1, null, null, null, null, null);
        Dependency _dependency2 = new(0, _dependentTask1, _dependsOnTask2, null, null, null, null, null);
        Dependency _dependency3 = new(0, _dependentTask2, _dependsOnTask1, null, null, null, null, null);
        Dependency _dependency4 = new(0, _dependentTask2, _dependsOnTask2, null, null, null, null, null);
        s_Dal!.Dependency.Create(_dependency1);
        s_Dal!.Dependency.Create(_dependency2);
        s_Dal!.Dependency.Create(_dependency3);
        s_Dal!.Dependency.Create(_dependency4);

        //Generate random dependencies
        for (int i = 0; i <= 36; i++) 
        { 
           
            //Choose random tasks to be dependent on each other
            int _dependentTask = _tasks[s_rand.Next(0, _tasks.Length)].id;
            int _dependsOnTask = _tasks[s_rand.Next(0, _tasks.Length)].id;

            //Make sure the dependency we are creating does not create a circular dependency
            while (createsCircularDependency(_dependentTask, _dependsOnTask))
            {
                _dependsOnTask = _tasks[s_rand.Next(0, _tasks.Length)].id;
            }

            Dependency _newDependency = new(0, _dependentTask, _dependsOnTask, null, null, null, null, null);

            //Make sure a dependency doesn't exist in s_dalDependency with the same dependentTask and dependsOnTask
            Dependency[] _dependencies = s_Dal!.Dependency.ReadAll().ToArray();
            bool _existsAlready = false;
            foreach(Dependency _dependency in _dependencies)
            {
                if (_dependency.dependentTask == _dependentTask && _dependency.dependsOnTask == _dependsOnTask) 
                {
                    _existsAlready = true;
                    break;
                }
            }
            if (!_existsAlready)
            {
                s_Dal!.Dependency.Create(_newDependency);
            }
        }
    }

    /// <summary>
    /// Figure out if the dependentTask and DependsOnTask will create a ciircular dependency 
    /// </summary>
    /// <param name="_dependentTask"></param>
    /// <param name="_dependsOnTask"></param>
    /// <returns>If the dependency is circular</returns>
    private static bool createsCircularDependency(int _dependentTask, int _dependsOnTask)
    {
        // Check if dependsOnTask is directly or indirectly dependent on dependentTask
        return isIndirectlyDependent(_dependentTask, _dependsOnTask, new List<int>());
    }

    /// <summary>
    /// Recursively check if there is a direct or indirect circular dependency
    /// </summary>
    /// <param name="_targetTask"></param>
    /// <param name="_currentTask"></param>
    /// <param name="_visitedTasks"></param>
    /// <returns>True if circular dependency found</returns>
    private static bool isIndirectlyDependent(int _targetTask, int _currentTask, List<int> _visitedTasks)
    {
        if (_visitedTasks.Contains(_currentTask))
        {
            // We've encountered a task we've already visited in this recursion branch, indicating a circular dependency
            return true;
        }

        _visitedTasks.Add(_currentTask);

        Dependency[] _dependencies = s_Dal!.Dependency.ReadAll().Where(d => d.dependentTask == _currentTask).ToArray();

        foreach (var _dependency in _dependencies)
        {
            if (_dependency.dependsOnTask == _targetTask || isIndirectlyDependent(_targetTask, _dependency.dependsOnTask, _visitedTasks))
            {
                return true;
            }
        }

        _visitedTasks.Remove(_currentTask); // Backtrack when moving to the next task
        return false;
    }
}
