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
            bool _exit = false;
            while (!_exit)
            {
                Console.WriteLine("0. Exit the main menu\n1. Initialize data with random values\n2. Test out Engineer\n3. Test out Task\n4. Test out Dependency\n5. Reset Project\n");
                string? _input = Console.ReadLine();
                int.TryParse(_input, out int _inputNumber);
                Console.WriteLine();
                string? _userInput;
                switch (_inputNumber)
                {
                    case 0:
                        _exit = true;
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
                        _userInput = Console.ReadLine();
                        Console.WriteLine();
                        EngineerOptionsSwitch(_userInput);
                        break;
                    case 3:
                        TaskOptionsPrint();
                        _userInput = Console.ReadLine();
                        Console.WriteLine();
                        TaskOptionsSwitch(_userInput);
                        break;
                    case 4:
                        DependencyOptionsPrint();
                        _userInput = Console.ReadLine();
                        Console.WriteLine();
                        DependencyOptionsSwitch(_userInput);
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
    /// <param name="_userInput">Operation user chose to perform with the Engineers</param>
    static void EngineerOptionsSwitch(string? _userInput)
    {
        int _id;
        Engineer _newEngineer;
        try
        {
            switch (_userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Engineer
                    Console.WriteLine("Enter the id, name, email, cost, and experience level (on seperate lines):\n");
                    _newEngineer = ParseEngineer();
                    s_dal.Engineer.Create(_newEngineer);
                    break;
                case "c": //Display Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    Engineer? _engineerToPrint = s_dal.Engineer.Read(_id);
                    Console.WriteLine();
                    Console.WriteLine(_engineerToPrint + "\n");
                    break;
                case "d": //Display Engineer List
                    IEnumerable<Engineer?> _engineerList = s_dal.Engineer.ReadAll();
                    foreach (Engineer? _engineer in _engineerList)
                    {
                        Console.WriteLine(_engineer);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Engineer
                    Console.WriteLine("Enter the updated information of the engineer, including - id, name, email, cost, and experience level (on seperate lines):\n");
                    _newEngineer = ParseEngineer();
                    s_dal.Engineer.Update(_newEngineer);
                    break;
                case "f": //Delete Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    s_dal.Engineer.Delete(_id);
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
    /// <param name="_userInput">Operation user chose to perform with the Tasks</param>
    static void TaskOptionsSwitch(string? _userInput)
    {
        int _id;
        Task _newTask;
        try
        {
            switch (_userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Task
                    Console.WriteLine("Enter the id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):\n");
                    _newTask = ParseTask();
                    s_dal.Task.Create(_newTask);
                    break;
                case "c": //Display Task
                    Console.WriteLine("Enter the id of the task you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    Task? _taskToPrint = s_dal.Task.Read(_id);
                    Console.WriteLine(_taskToPrint + "\n");
                    break;
                case "d": //Display Task List
                    IEnumerable<Task?> _taskList = s_dal.Task.ReadAll();
                    foreach (Task? _task in _taskList)
                    {
                        Console.WriteLine(_task);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Task
                    Console.WriteLine("Enter the updated information of the task, including - id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):\n");
                    _newTask = ParseTask();
                    s_dal.Task.Update(_newTask);
                    break;
                case "f": //Delete Task
                    Console.WriteLine("Enter the id of the task you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    s_dal.Task.Delete(_id);
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
    /// <param name="_userInput">Operation user chose to perform with the Dependencies</param>
    static void DependencyOptionsSwitch(string? _userInput)
    {
        int _id;
        Dependency _newDependency;
        try
        {
            switch (_userInput)
            {
                case "a": //Go Back
                    Console.WriteLine();
                    break;
                case "b": //Add Dependency
                    Console.WriteLine("Enter the id, dependent task id, depends-on task id, customer email, shipping address, order creation date, shipping date, and delivery date (on seperate lines):\n");
                    _newDependency = ParseDependency();
                    s_dal.Dependency.Create(_newDependency);
                    break;
                case "c": //Display Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to display:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    Dependency? _dependencyToPrint = s_dal.Dependency.Read(_id);
                    Console.WriteLine(_dependencyToPrint + "\n");
                    break;
                case "d": //Display Dependency List
                    IEnumerable<Dependency?> _dependencyList = s_dal.Dependency.ReadAll();
                    foreach (Dependency? _dependency in _dependencyList)
                    {
                        Console.WriteLine(_dependency);
                    }
                    Console.WriteLine();
                    break;
                case "e": //Update Dependency
                    Console.WriteLine("Enter the updated information of the dependecy, including - id, dependent task id, depends-on task id, customer email, shipping address, order creation date, shipping date, and delivery date (on seperate lines):\n");
                    _newDependency = ParseDependency();
                    s_dal.Dependency.Update(_newDependency);
                    break;
                case "f": //Delete Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to delete:\n");
                    int.TryParse(Console.ReadLine(), out _id);
                    Console.WriteLine();
                    s_dal.Dependency.Delete(_id);
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
    /// Receives input from user for all feilds of Engineer
    /// </summary>
    /// <returns>Engineer object based on user input</returns>
    static Engineer ParseEngineer()
    {
        int.TryParse(Console.ReadLine(), out int _id);
        String? _name = Console.ReadLine();
        String? _email = Console.ReadLine();
        double? _cost;
        bool _costConverted = double.TryParse(Console.ReadLine(), out double _costValue);
        _cost = _costConverted ? _costValue : null;
        Enum.TryParse(Console.ReadLine(), out EngineerExperience _level);
        Engineer _newEngineer = new(_id, _name, _email, _cost, _level);
        Console.WriteLine();

        return _newEngineer;
    }

    /// <summary>
    /// Receives input from user for all fields of Task
    /// </summary>
    /// <returns>Task object based on user input</returns>
    static Task ParseTask()
    {
        int.TryParse(Console.ReadLine(), out int _id);
        bool.TryParse(Console.ReadLine(), out bool _isMilestone);
        Enum.TryParse(Console.ReadLine(), out EngineerExperience _level);
        int? _assignedEngineerId;
        bool _engineerConverted = int.TryParse(Console.ReadLine(), out int _assignedId);
        _assignedEngineerId = _engineerConverted ? _assignedId : null;
        string? _nickname = Console.ReadLine();
        string? _description = Console.ReadLine();
        string? _deliverables = Console.ReadLine();
        string? _notes = Console.ReadLine();
        DateTime? _dateCreated;
        bool _dateCreatedConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _dateCreatedValue);
        _dateCreated = _dateCreatedConverted ? _dateCreatedValue : null;
        DateTime? _projectedStartDate;
        bool _projectedDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _projectedDateValue);
        _projectedStartDate = _projectedDateConverted ? _projectedDateValue : null;
        DateTime? _actualStartDate;
        bool _startDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _startDateValue);
        _actualStartDate = _startDateConverted ? _startDateValue : null;
        TimeSpan? _duration;
        bool _durationConverted = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan _durationValue);
        _duration = _durationConverted ? _durationValue : null;
        DateTime? _deadline;
        bool _deadlineConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _deadlineValue);
        _deadline = _deadlineConverted ? _deadlineValue : null;
        DateTime? _actualEndDate;
        bool _endDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _endDateValue);
        _actualEndDate = _endDateConverted ? _endDateValue : null;
        Console.WriteLine();

        Task _newTask = new(_id, _isMilestone, _level, _assignedEngineerId, _nickname, _description, _deliverables, _notes, _dateCreated, _projectedStartDate, _actualStartDate, _duration, _deadline, _actualEndDate);
        return _newTask;
    }

    /// <summary>
    /// Receives input from user for all feilds of Dependency
    /// </summary>
    /// <returns>Dependency object based on user input</returns>
    static Dependency ParseDependency()
    {
        int.TryParse(Console.ReadLine(), out int _id);
        int.TryParse(Console.ReadLine(), out int _dependentTask);
        int.TryParse(Console.ReadLine(), out int _dependsOnTask);
        string? _customerEmail = Console.ReadLine();
        string? _shippingAddress = Console.ReadLine();
        DateTime? _orderCreationDate;
        bool _dateCreatedConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _dateCreatedValue);
        _orderCreationDate = _dateCreatedConverted ? _dateCreatedValue : null;
        DateTime? _shippingDate;
        bool _shippingDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _shippingDateValue);
        _shippingDate = _shippingDateConverted ? _shippingDateValue : null;
        DateTime? _deliveryDate;
        bool _deliveryDateConverted = DateTime.TryParse(Console.ReadLine(), out DateTime _deliveryDateValue);
        _deliveryDate = _deliveryDateConverted ? _deliveryDateValue : null;
        Console.WriteLine();

        Dependency _newDependency = new(_id, _dependentTask, _dependsOnTask);
        return _newDependency;
    }
}
