namespace DalTest;

using Dal;
using DalApi;
using DO;
using Task = DO.Task;

/// <summary>
/// Main Program of DalTest which allows the Data Layer to be tested by CLI
/// </summary>
internal class Program
{
    //static readonly IDal s_dal = new DalList(); // stage 2
    //static readonly IDal s_dal = new DalXml(); //Stage 3
    static readonly IDal s_dal = Factory.Get; // stage 4

    /// <summary>
    /// Runs loop of main menu
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        try
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("0. Exit the main menu\n1. Initialize data with random values\n2. Test out Engineer\n3. Test out Task\n4. Test out Dependency\n5. Reset Project\n");
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
                        Console.Write("Would you like to create initial data? (Y/N)\n"); //stage 3
                        string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input"); //stage 3
                        if (ans == "Y") //stage 3
                        { 
                            s_dal.Engineer.Reset();
                            s_dal.Dependency.Reset();
                            s_dal.Task.Reset();
                            s_dal.Config.Reset();
                            //Initialization.Do(s_dal); // stage 2
                            Initialization.Do(); // stage 4
                        }
                        break;
                    case 2:
                        EngineerOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        EngineerOptionsSwitch(userInput);
                        break;
                    case 3:
                        TaskOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        TaskOptionsSwitch(userInput);
                        break;
                    case 4:
                        DependencyOptionsPrint();
                        userInput = Console.ReadLine();
                        Console.WriteLine();
                        DependencyOptionsSwitch(userInput);
                        break;
                    case 5:
                        s_dal.Config.Reset();
                        s_dal.Engineer.Reset();
                        s_dal.Dependency.Reset();
                        s_dal.Task.Reset();
                        Console.WriteLine("Project Reset...\n");
                        break;
                    default: Console.WriteLine("Incorrect input, try again\n"); 
                        break;
                }
            }
        }
        catch (DalDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalDeletionImpossible ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.ToString());
        }
    }

    static void EngineerOptionsPrint()
    {
        Console.WriteLine("" +
                            "a) Go back\n" +
                            "b) Add an engineer to the engineer list\n" +
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
                            "f) Delete a task from the task list\n" +
                            "g) Reset tasks\n");
    }

    static void DependencyOptionsPrint()
    {
        Console.WriteLine("" +
                            "a) Go back\n" +
                            "b) Add a dependency to the dependency list\n" +
                            "c) Display a dependency using a dependency's id\n" +
                            "d) Display the dependency list\n" +
                            "e) Update a dependency\n" +
                            "f) Delete a dependecy from the task list\n" +
                            "g) Reset dependencies\n");
    }

    /// <summary>
    /// Switch to perform requested action with Engineers
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Engineers</param>
    static void EngineerOptionsSwitch(string? userInput)
    {
        int id;
        Engineer newEngineer;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Engineer
                    Console.WriteLine("Enter the id, name, email, cost, and experience level (on seperate lines):\n");
                    newEngineer = ParseEngineer();
                    s_dal.Engineer.Create(newEngineer);
                    break;
                case "c": //Display Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    Engineer? engineerToPrint = s_dal.Engineer.Read(id);
                    Console.WriteLine();
                    Console.WriteLine(engineerToPrint + "\n");
                    break;
                case "d": //Display Engineer List
                    IEnumerable<Engineer?> engineerList = s_dal.Engineer.ReadAll();
                    foreach (Engineer? engineer in engineerList)
                    {
                        Console.WriteLine(engineer);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Engineer
                    Console.WriteLine("Enter the updated information of the engineer, including - id, name, email, cost, and experience level (on seperate lines):\n");
                    newEngineer = ParseEngineer();
                    s_dal.Engineer.Update(newEngineer);
                    break;
                case "f": //Delete Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    s_dal.Engineer.Delete(id);
                    break;
                case "g": //Reset Engineers
                    s_dal.Engineer.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (DalDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalDeletionImpossible ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Switch to perform requested action with Tasks
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Tasks</param>
    static void TaskOptionsSwitch(string? userInput)
    {
        int id;
        Task newTask;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Task
                    Console.WriteLine("Enter the id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):\n");
                    newTask = ParseTask();
                    s_dal.Task.Create(newTask);
                    break;
                case "c": //Display Task
                    Console.WriteLine("Enter the id of the task you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    Task? taskToPrint = s_dal.Task.Read(id);
                    Console.WriteLine(taskToPrint + "\n");
                    break;
                case "d": //Display Task List
                    IEnumerable<Task?> taskList = s_dal.Task.ReadAll();
                    foreach (Task? task in taskList)
                    {
                        Console.WriteLine(task);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Task
                    Console.WriteLine("Enter the updated information of the task, including - id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):\n");
                    newTask = ParseTask();
                    s_dal.Task.Update(newTask);
                    break;
                case "f": //Delete Task
                    Console.WriteLine("Enter the id of the task you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    s_dal.Task.Delete(id);
                    break;
                case "g": //Reset Tasks
                    s_dal.Task.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (DalDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalDeletionImpossible ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Switch to perform requested action with Dependencies
    /// </summary>
    /// <param name="userInput">Operation user chose to perform with the Dependencies</param>
    static void DependencyOptionsSwitch(string? userInput)
    {
        int id;
        Dependency newDependency;
        try
        {
            switch (userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Dependency
                    Console.WriteLine("Enter the id, dependent task id, depends-on task id, customer email, shipping address, order creation date, shipping date, and delivery date (on seperate lines):\n");
                    newDependency = ParseDependency();
                    s_dal.Dependency.Create(newDependency);
                    break;
                case "c": //Display Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    Dependency? dependencyToPrint = s_dal.Dependency.Read(id);
                    Console.WriteLine(dependencyToPrint + "\n");
                    break;
                case "d": //Display Dependency List
                    IEnumerable<Dependency?> dependencyList = s_dal.Dependency.ReadAll();
                    foreach (Dependency? dependency in dependencyList)
                    {
                        Console.WriteLine(dependency);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Dependency
                    Console.WriteLine("Enter the updated information of the dependecy, including - id, dependent task id, depends-on task id, customer email, shipping address, order creation date, shipping date, and delivery date (on seperate lines):\n");
                    newDependency = ParseDependency();
                    s_dal.Dependency.Update(newDependency);
                    break;
                case "f": //Delete Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out id);
                    Console.WriteLine();
                    s_dal.Dependency.Delete(id);
                    break;
                case "g": //Reset Dependencies
                    s_dal.Dependency.Reset();
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...\n");
                    break;
            }
        }
        catch (DalDoesNotExistException ex)
        {
            //Exception because Entity does not exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalAlreadyExistsException ex)
        {
            //Exception because Entity already exists
            Console.WriteLine(ex.ToString());
        }
        catch (DalDeletionImpossible ex)
        {
            //Exception because deletion is impossible
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// Receives input from user for all fields of Engineer
    /// </summary>
    /// <returns>Engineer object based on user input</returns>
    static Engineer ParseEngineer()
    {
        int.TryParse(Console.ReadLine(), out int id);
        String? name = Console.ReadLine();
        String? email = Console.ReadLine();
        double cost;
        bool costConverted = double.TryParse(Console.ReadLine(), out double costValue);
        cost = costConverted ? costValue : 0;
        Enum.TryParse(Console.ReadLine(), out EngineerExperience level);
        Engineer newEngineer = new(id, name, email, cost, level);
        Console.WriteLine();

        return newEngineer;
    }

    /// <summary>
    /// Receives input from user for all fields of Task
    /// </summary>
    /// <returns>Task object based on user input</returns>
    static Task ParseTask()
    {
        int.TryParse(Console.ReadLine(), out int id);
        bool.TryParse(Console.ReadLine(), out bool isMilestone);
        Enum.TryParse(Console.ReadLine(), out EngineerExperience level);
        int? assignedEngineerId;
        bool engineerConverted = int.TryParse(Console.ReadLine(), out int assignedId);
        assignedEngineerId = engineerConverted ? assignedId : null;
        string? nickname = Console.ReadLine();
        string? description = Console.ReadLine();
        string? deliverables = Console.ReadLine();
        string? notes = Console.ReadLine();
        DateTime? dateCreated;
        bool dateCreatedConverted = DateTime.TryParse(Console.ReadLine(), out DateTime dateCreatedValue);
        dateCreated = dateCreatedConverted ? dateCreatedValue : null;
        DateTime? projectedStartDate;
        bool projectedDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime projectedDateValue);
        projectedStartDate = projectedDateConverted ? projectedDateValue : null;
        DateTime? actualStartDate;
        bool startDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime startDateValue);
        actualStartDate = startDateConverted ? startDateValue : null;
        TimeSpan? duration;
        bool durationConverted = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan durationValue);
        duration = durationConverted ? durationValue : null;
        DateTime? deadline;
        bool deadlineConverted = DateTime.TryParse(Console.ReadLine(), out DateTime deadlineValue);
        deadline = deadlineConverted ? deadlineValue : null;
        DateTime? actualEndDate;
        bool endDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime endDateValue);
        actualEndDate = endDateConverted ? endDateValue : null;
        Console.WriteLine();

        Task newTask = new(id, isMilestone, level, assignedEngineerId, nickname, description, deliverables, notes, dateCreated, projectedStartDate, actualStartDate, duration, deadline, actualEndDate);
        return newTask;
    }

    /// <summary>
    /// Receives input from user for all fields of Dependency
    /// </summary>
    /// <returns>Dependency object based on user input</returns>
    static Dependency ParseDependency()
    {
        int.TryParse(Console.ReadLine(), out int id);
        int.TryParse(Console.ReadLine(), out int dependentTask);
        int.TryParse(Console.ReadLine(), out int dependsOnTask);
        string? customerEmail = Console.ReadLine();
        string? shippingAddress = Console.ReadLine();
        DateTime? orderCreationDate;
        bool dateCreatedConverted = DateTime.TryParse(Console.ReadLine(), out DateTime dateCreatedValue);
        orderCreationDate = dateCreatedConverted ? dateCreatedValue : null;
        DateTime? shippingDate;
        bool shippingDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime shippingDateValue);
        shippingDate = shippingDateConverted ? shippingDateValue : null;
        DateTime? deliveryDate;
        bool deliveryDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime deliveryDateValue);
        deliveryDate = deliveryDateConverted ? deliveryDateValue : null;
        Console.WriteLine();

        Dependency newDependency = new(id, dependentTask, dependsOnTask);
        return newDependency;
    }
}
