using BO;
using DalApi;
using System.Xml.Linq;
namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    static void Main(string[] args)
    {
        Console.Write("Would you like to create Initial data? (Y/N) ");
        string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input");
        if (ans == "Y")
            DalTest.Initialization.Do();

        try
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("0. Exit the main menu\n1. Set project start and end date\n2. Test out Tasks\n3. Test out Engineers\n4. Test out Milestones\n5. Reset Project\n");
                string? input = Console.ReadLine();
                int.TryParse(input, out int inputNumber);
                Console.WriteLine();
                string? userInput;
                switch (inputNumber)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        SetProjectTimeSpan();
                        break;
                    case 2: //Tasks
                        TaskOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        TaskOptionsSwitch(userInput);
                        break;
                    case 3: //Engineers
                        EngineerOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        EngineerOptionsSwitch(userInput);
                        break;
                    case 4: //Milestones
                        MilestoneOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        MilestoneOptionsSwitch(userInput);
                        break;
                    case 5: //Reset
                        s_bl.Config.Reset();
                        s_bl.Engineer.Reset();
                        s_bl.Task.Reset();
                        s_bl.Milestone.Reset();
                        Console.WriteLine("Project Reset...\n");
                        break;
                    default:
                        Console.WriteLine("Incorrect input, try again\n");
                        break;
                }
            }
        }
        catch (BlDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.Message);
        }
        catch (BlAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.Message);
        }
        catch (BlCannotBeDeletedException ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.Message);
        }
        catch (BlInvalidInputException ex)
        {
            //Exception because invalid input was given
            Console.WriteLine(ex.Message);
        }
        catch (BlNullPropertyException ex)
        {
            //Exception because a necessary property was null
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToCreateScheduleException ex)
        {
            //Exception because the schedule could not be created for some reason
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToPerformActionInProductionException ex)
        {
            //Exception because this action cannot be performed once the system is in production mode
            Console.WriteLine(ex.Message);
        }
        catch (BlCannotChangeDateException ex)
        {
            //Exception because the start and end dates cannot be changed once they're set
            Console.WriteLine(ex.Message);
        }
    }

    static void EngineerOptionsPrint()
    {
        Console.WriteLine("" +
                            "a) Go back\n" +
                            "b) Add an engineer\n" +
                            "c) Display an engineer using an engineer's id\n" +
                            "d) Display the engineer list\n" +
                            "e) Update an engineer\n" +
                            "f) Delete an engineer from the engineer list\n" +
                            "g) Reset engineers\n");
    }

    static void TaskOptionsPrint()
    {
        Console.WriteLine("" +
                            "a) Go back\n" +
                            "b) Add a task to the task list\n" +
                            "c) Display a task using a task's id\n" +
                            "d) Display the task list\n" +
                            "e) Update a task\n" +
                            "f) Update a task's start date\n" +
                            "g) Assign an engineer to a Task\n" +
                            "h) Delete a task from the task list\n" +
                            "i) Reset tasks\n");
    }

    static void MilestoneOptionsPrint()
    {
        Console.WriteLine("" +
                            "a) Go back\n" +
                            "b) Create (automatically) milestones and project schedule\n" +
                            "c) Get Milestone\n" +
                            "d) Update Milestone\n" +
                            "e) Reset Milestones\n");
    }

    static void SetProjectTimeSpan()
    {
        try
        {
            Console.WriteLine("Enter desired project start date");
            DateTime startDate;
            bool startDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime startDateValue);
            startDate = startDateConverted ? startDateValue : DateTime.Now;
            if (startDateConverted)
                s_bl.Config.SetProjectStartDate(startDate);
            Console.WriteLine("Enter desired project end date");
            DateTime endDate;
            bool endDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime endDateValue);
            endDate = endDateConverted ? endDateValue : DateTime.Now;
            if (endDateConverted)
                s_bl.Config.SetProjectEndDate(endDate);
        }
        catch (BlCannotChangeDateException ex)
        {
            //Exception because start and end date cannot be changed once set
            Console.WriteLine(ex.Message +"\n");
        }
        
    }

    /// <summary>
    /// Switch to perform requested action with Engineers
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Engineers</param>
    static void EngineerOptionsSwitch(string? userInput)
    {
        int id;
        BO.Engineer newEngineer;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Engineer
                    Console.WriteLine("Enter the id, name, email, experience level, cost, and assigned task ID (on separate lines):\n");
                    newEngineer = ParseEngineer();
                    s_bl.Engineer.AddEngineer(newEngineer);
                    break;
                case "c": //Display Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    BO.Engineer? engineerToPrint = s_bl.Engineer.GetEngineer(id);
                    Console.WriteLine();
                    Console.WriteLine(engineerToPrint + "\n");
                    break;
                case "d": //Display Engineer List
                    IEnumerable<BO.Engineer?> engineerList = s_bl.Engineer.GetListOfEngineers();
                    foreach (BO.Engineer? engineer in engineerList)
                    {
                        Console.WriteLine(engineer);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Engineer
                    Console.WriteLine("Enter the updated information of the engineer, including - id, name, email, experience level, cost, and assigned task ID (on separate lines):\n");
                    newEngineer = ParseEngineer();
                    s_bl.Engineer.UpdateEngineer(newEngineer);
                    break;
                case "f": //Delete Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    s_bl.Engineer.DeleteEngineer(id);
                    break;
                case "g": //Reset Engineers
                    s_bl.Engineer.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (BlDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.Message);
        }
        catch (BlAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.Message);
        }
        catch (BlCannotBeDeletedException ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.Message);
        }
        catch (BlInvalidInputException ex)
        {
            //Exception because invalid input was given
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Switch to perform requested action with Tasks
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Tasks</param>
    static void TaskOptionsSwitch(string? userInput)
    {
        int id;
        BO.Task newTask;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Task
                    if (!s_bl.Config.inProduction())
                    {
                        Console.WriteLine("Enter the id, name, description, date created, projected start date, required effort time, deadline, deliverables, notes, and complexity (on separate lines):\n");
                        newTask = ParseTask(adding: true); //Need to also ask for dependencies
                        s_bl.Task.AddTask(newTask);
                        break;
                    }
                    Console.WriteLine("In production so can't create a new task\n");
                    break;
                case "c": //Display Task
                    Console.WriteLine("Enter the id of the task you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    BO.Task? taskToPrint = s_bl.Task.GetTask(id);
                    Console.WriteLine(taskToPrint + "\n");
                    break;
                case "d": //Display Task List
                    IEnumerable<BO.Task?> taskList = s_bl.Task.GetListOfTasks();
                    foreach (BO.Task? task in taskList)
                    {
                        Console.WriteLine(task);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Task
                    if (s_bl.Config.inProduction())
                    {
                        Console.WriteLine("Enter the updated information of the task, including - id, name, description, actual start date, actual end date, deliverables, notes, assigned engineer ID (on separate lines):\n");
                    }
                    else 
                    {
                        Console.WriteLine("Enter the updated information of the task, including - id, name, description, projected start date, required effort time, deadline, deliverables, notes, complexity (on separate lines):\n");
                    }
                    newTask = ParseTask();
                    s_bl.Task.UpdateTask(newTask);
                    break;
                case "f": //Update Task Start Date
                    Console.WriteLine("Enter the id of the task you would like to update:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine("Enter the new start date of the Task:\n");
                    DateTime.TryParse(Console.ReadLine(), out DateTime startDateValue);
                    s_bl.Task.UpdateTaskStartDate(id, startDateValue);
                    break;
                case "g": //Assign engineer to Task
                    if (!s_bl.Config.inProduction())
                        throw new BO.BlUnableToPerformActionInPlanningException("Cannot assign an engineer to a task in planning mode");
                    Console.WriteLine("Enter the id of the task you would like to assign an engineer to:");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine("Enter the id of the engineer you would like to assign to the task:");
                    int.TryParse(Console.ReadLine(), out int engineerId);
                    BO.Task oldTask = s_bl.Task.GetTask(id);
                    BO.Engineer engineer = s_bl.Engineer.GetEngineer(engineerId);
                    BO.Task taskWithEngineer = new(id, oldTask.Name, oldTask.Description, Status.Unscheduled, oldTask.Dependencies, oldTask.Milestone, oldTask.CreatedAtDate, oldTask.ProjectedStartDate, oldTask.ActualStartDate, oldTask.ProjectedEndDate, oldTask.Deadline, oldTask.ActualEndDate, oldTask.RequiredEffortTime, oldTask.Deliverables, oldTask.Notes, new EngineerInTask(engineer.ID, engineer.Name), oldTask.Complexity);
                    s_bl.Task.UpdateTask(taskWithEngineer);
                    break;
                case "h": //Delete Task
                    Console.WriteLine("Enter the id of the task you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    s_bl.Task.DeleteTask(id);
                    break;
                case "i": //Reset Tasks
                    s_bl.Task.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (BlDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.Message);
        }
        catch (BlAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.Message);
        }
        catch (BlCannotBeDeletedException ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.Message);
        }
        catch (BlInvalidInputException ex)
        {
            //Exception because invalid input was given
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToPerformActionInProductionException ex)
        {
            //Exception because this action cannot be performed once the system is in production mode
            Console.WriteLine(ex.Message);
        }
        catch (BlNullPropertyException ex)
        {
            //Exception because a necessary property was null
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToPerformActionInPlanningException ex)
        {
            //Exception because this action cannot be performed once the system is in planning mode
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Switch to perform requested action with Milestones
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Milestones</param>
    static void MilestoneOptionsSwitch(string? userInput)
    {
        int id;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Create (automatically) milestones and project schedule
                    s_bl.Milestone.CreateProjectSchedule();
                    break;
                case "c": //Display/Get Milestone
                    Console.WriteLine("Enter the id of the milestone you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    Milestone? milestoneToPrint = s_bl.Milestone.GetMilestone(id);
                    Console.WriteLine();
                    Console.WriteLine(milestoneToPrint + "\n");
                    break;
                case "d": //Update Milestone
                    Console.WriteLine("Enter the updated information of the milestone, including - id, name, description, and notes (on separate lines):\n");
                    int.TryParse(Console.ReadLine(), out id);
                    string? name = Console.ReadLine();
                    string? description = Console.ReadLine();
                    string? notes = Console.ReadLine();
                    s_bl.Milestone.UpdateMilestone(id, name, description, notes);
                    break;
                case "e": //Reset Milestones
                    s_bl.Milestone.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (BlDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.Message);
        }
        catch (BlCannotBeDeletedException ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.Message);
        }
        catch (BlInvalidInputException ex)
        {
            //Exception because invalid input was given
            Console.WriteLine(ex.Message);
        }
        catch (BlNullPropertyException ex)
        {
            //Exception because a necessary property was null
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToCreateScheduleException ex)
        {
            //Exception because the schedule could not be created for some reason
            Console.WriteLine(ex.Message);
        }
        catch (BlUnableToPerformActionInProductionException ex)
        {
            //Exception because this action cannot be performed once the system is in production mode
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Receives input from user for all fields of Engineer
    /// </summary>
    /// <returns>Engineer object based on user input</returns>
    private static BO.Engineer ParseEngineer()
    {
        int.TryParse(Console.ReadLine(), out int id);
        String? name = Console.ReadLine();
        String? email = Console.ReadLine();
        Enum.TryParse(Console.ReadLine(), out EngineerExperience level);
        double cost;
        bool costConverted = double.TryParse(Console.ReadLine(), out double costValue);
        cost = costConverted ? costValue : 0;
        //Get Assigned Task from ID
        TaskInEngineer? taskInEngineer = null;
        bool hasTask = int.TryParse(Console.ReadLine(), out int taskID);
        if (hasTask)
        {
            BO.Task assignedTask = s_bl.Task.GetTask(taskID);
            taskInEngineer = new(assignedTask.ID, assignedTask.Name);
        }
        BO.Engineer newEngineer = new(id, name, email, level, cost, taskInEngineer);
        Console.WriteLine();

        return newEngineer;
    }

    /// <summary>
    /// Receives input from user for all fields of Task
    /// </summary>
    /// <returns>Task object based on user input</returns>
    private static BO.Task ParseTask(bool adding = false)
    {
        //Enter the updated information of the task, including:
        //In Production - id, name, description, actual start date, actual end date, deliverables, notes, assigned engineer ID
        //In Planning - id, name, description, projected start date, required effort time, deadline, deliverables, notes, complexity [and dependencies]
        //When Adding - id, name, description, date created, projected start date, required effort time, deadline, deliverables, notes, and complexity [and dependencies]
        bool inProduction = s_bl.Config.inProduction();
        bool inPlanning = !s_bl.Config.inProduction();

        int.TryParse(Console.ReadLine(), out int id);
        string? name = Console.ReadLine();
        string? description = Console.ReadLine();
        
        DateTime? dateCreated = null;
        if (adding)
        {
            bool dateCreatedConverted = DateTime.TryParse(Console.ReadLine(), out DateTime dateCreatedValue);
            dateCreated = dateCreatedConverted ? dateCreatedValue : null;
        }

        DateTime? projectedStartDate = null;
        if (inPlanning || adding)
        {
            bool projectedDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime projectedDateValue);
            projectedStartDate = projectedDateConverted ? projectedDateValue : null;
        }
        
        DateTime? actualStartDate = null;
        if (inProduction)
        {
            bool startDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime startDateValue);
            actualStartDate = startDateConverted ? startDateValue : null;
        }
        
        TimeSpan? duration = null;
        if (inPlanning || adding)
        {
            bool durationConverted = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan durationValue);
            duration = durationConverted ? durationValue : null;
        }

        DateTime? deadline = null;
        if (inPlanning || adding)
        {
            bool deadlineConverted = DateTime.TryParse(Console.ReadLine(), out DateTime deadlineValue);
            deadline = deadlineConverted ? deadlineValue : null;
        }
        
        DateTime? actualEndDate = null;
        if (inProduction)
        {
            bool endDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime endDateValue);
            actualEndDate = endDateConverted ? endDateValue : null;
        }
        
        string? deliverables = Console.ReadLine();
        string? notes = Console.ReadLine();
        
        //Get Assigned Engineer from ID
        EngineerInTask? assignedEngineer = null;
        if (inProduction)
        {
            bool engineerConverted = int.TryParse(Console.ReadLine(), out int assignedId);
            if (engineerConverted)
            {
                BO.Engineer engineer = s_bl.Engineer.GetEngineer(assignedId);
                assignedEngineer = new(engineer.ID, engineer.Name);
            }
        }

        EngineerExperience complexity = EngineerExperience.Beginner;
        if (inPlanning || adding)
        {
            Enum.TryParse(Console.ReadLine(), out complexity);

        }

        //Ask for dependencies
        List<TaskInList> dependencies = new List<TaskInList>();
        if (inPlanning || adding)
        {
            Console.WriteLine("Enter IDs of tasks this task depends on (comma-separated):");
            string? input = Console.ReadLine();
            if (input != "")
            {
                string[] dependencyIds = input.Split(',');

                foreach (string dependencyId in dependencyIds)
                {
                    if (int.TryParse(dependencyId.Trim(), out int depId))
                    {
                        BO.Task dependencyTask = s_bl.Task.GetTask(depId);
                        if (dependencyTask != null)
                        {
                            TaskInList taskInList = new TaskInList(
                                dependencyTask.ID,
                                dependencyTask.Name,
                                dependencyTask.Description,
                                dependencyTask.Status
                            );
                            dependencies.Add(taskInList);
                        }
                        else
                        {
                            Console.WriteLine($"Task with ID {depId} not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Invalid ID: {dependencyId}");
                    }
                }
            }
        }
        
        Console.WriteLine();

        BO.Task newTask = new(id, name, description, Status.Unscheduled, dependencies, null, dateCreated, projectedStartDate, actualStartDate, projectedStartDate+duration, deadline, actualEndDate, duration, deliverables, notes, assignedEngineer, complexity);
        return newTask;
    }


}
