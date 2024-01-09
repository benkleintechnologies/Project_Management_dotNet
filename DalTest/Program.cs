using Dal;
using DalApi;
using DO;
using Task = DO.Task;

namespace DalTest
{
    internal class Program
    {
        private static IEngineer? s_dalEngineer = new EngineerImplementation();
        private static ITask? s_dalTask = new TaskImplementation();
        private static IDependency? s_dalDependency = new DependencyImplementation();
        static void Main(string[] args)
        {
            try
            {
                Initialization.Do(s_dalEngineer, s_dalDependency, s_dalTask);
                bool _exit = false;
                while (!_exit)
                {
                    Console.WriteLine("0. Exit the main menu\n1. Test out Engineer\n2. Test out Task\n3. Test out Dependency");
                    string? _input = Console.ReadLine();
                    int.TryParse(_input, out int _inputNumber);
                    string _userInput;
                    switch (_inputNumber)
                    {
                        case 0:
                            _exit = true;
                            break;
                        case 1:
                            EngineerOptionsPrint();
                            _userInput = Console.ReadLine();
                            EngineerOptionsSwitch(_userInput);
                            break;
                        case 2:
                            TaskOptionsPrint();
                            _userInput = Console.ReadLine();
                            TaskOptionsSwitch(_userInput);
                            break;
                        case 3:
                            DependencyOptionsPrint();
                            _userInput = Console.ReadLine();
                            DependencyOptionsSwitch(_userInput);
                            break;
                        default: Console.WriteLine("Incorrect input, try again"); 
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
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
                                "g) Reset engineers");
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
                                "g) Reset tasks");
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
                                "g) Reset dependencies");
        }

        static void EngineerOptionsSwitch(string _userInput)
        {
            int _id;
            Engineer _newEngineer;
            switch (_userInput)
            {
                case "a": //Go Back
                    break;
                case "b": //Add Engineer
                    Console.WriteLine("Enter the id, name, email, cost, and experience level (on seperate lines):");
                    _newEngineer = ParseEngineer();
                    s_dalEngineer.Create(_newEngineer);
                    break;
                case "c": //Display Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to display:");
                    int.TryParse(Console.ReadLine(), out _id);
                    Engineer _engineerToPrint = s_dalEngineer.Read(_id);
                    Console.WriteLine(_engineerToPrint);
                    break;
                case "d": //Display Engineer List
                    List<Engineer> _engineerList = s_dalEngineer.ReadAll();
                    foreach(Engineer _engineer in _engineerList )
                    {
                        Console.WriteLine(_engineer);
                    }
                    break;
                case "e": //Update Engineer
                    Console.WriteLine("Enter the updated information of the engineer, including - id, name, email, cost, and experience level (on seperate lines):");
                    _newEngineer = ParseEngineer();
                    s_dalEngineer.Update(_newEngineer);
                    break;
                case "f": //Delete Engineer
                    Console.WriteLine("Enter the id of the engineer you would like to delete:");
                    int.TryParse(Console.ReadLine(), out _id);
                    s_dalEngineer.Delete(_id);
                    break;
                case "g": //Reset Engineers
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...");
                    break;
            }
        }

        static void TaskOptionsSwitch(string _userInput)
        {
            int _id;
            Task _newTask;
            switch (_userInput)
            {
                case "a": //Go Back
                    break;
                case "b": //Add Task
                    Console.WriteLine("Enter the id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):");
                    _newTask = ParseTask();
                    s_dalTask.Create(_newTask);
                    break;
                case "c": //Display Task
                    Console.WriteLine("Enter the id of the task you would like to display:");
                    int.TryParse(Console.ReadLine(), out _id);
                    Task _taskToPrint = s_dalTask.Read(_id);
                    Console.WriteLine(_taskToPrint);
                    break;
                case "d": //Display Task List
                    List<Task> _taskList = s_dalTask.ReadAll();
                    foreach (Task _task in _taskList)
                    {
                        Console.WriteLine(_task);
                    }
                    break;
                case "e": //Update Task
                    Console.WriteLine("Enter the updated information of the task, including - id, isMilestone, degree of difficulty, assigned engineer id, nickname, description, deliverables, notes, date created, projected start date, actual start date, duration, deadline, and actual end date (on seperate lines):");
                    _newTask = ParseTask();
                    s_dalTask.Update(_newTask);
                    break;
                case "f": //Delete Task
                    Console.WriteLine("Enter the id of the task you would like to delete:");
                    int.TryParse(Console.ReadLine(), out _id);
                    s_dalTask.Delete(_id);
                    break;
                case "g": //Reset Tasks
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...");
                    break;
            }
        }

        static void DependencyOptionsSwitch(string _userInput)
        {
            int _id;
            Dependency _newDependency;
            switch (_userInput)
            {
                case "a": //Go Back
                    break;
                case "b": //Add Dependency
                    Console.WriteLine("Enter the id, dependent task id, depends-on task id, customer email, shipping address, and order creation date (on seperate lines):");
                    _newDependency = ParseDependency();
                    s_dalDependency.Create(_newDependency);
                    break;
                case "c": //Display Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to display:");
                    int.TryParse(Console.ReadLine(), out _id);
                    Dependency _dependencyToPrint = s_dalDependency.Read(_id);
                    Console.WriteLine(_dependencyToPrint);
                    break;
                case "d": //Display Dependency List
                    List<Dependency> _dependencyList = s_dalDependency.ReadAll();
                    foreach (Dependency _dependency in _dependencyList)
                    {
                        Console.WriteLine(_dependency);
                    }
                    break;
                case "e": //Update Dependency
                    Console.WriteLine("Enter the updated information of the dependecy, including - id, dependent task id, depends-on task id, customer email, shipping address, and order creation date (on seperate lines):");
                    _newDependency = ParseDependency();
                    s_dalDependency.Update(_newDependency);
                    break;
                case "f": //Delete Dependency
                    Console.WriteLine("Enter the id of the dependency you would like to delete:");
                    int.TryParse(Console.ReadLine(), out _id);
                    s_dalDependency.Delete(_id);
                    break;
                case "g": //Reset Dependencies
                    break;
                default:
                    Console.WriteLine("Not sure how you ended up here... Try again...");
                    break;
            }
        }

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
            return _newEngineer;
        }

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

            Task _newTask = new(_id, _isMilestone, _level, _assignedEngineerId, _nickname, _description, _deliverables, _notes, _dateCreated, _projectedStartDate, _actualStartDate, _duration, _deadline, _actualEndDate);
            return _newTask;
        }

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
            
            Dependency _newDependency = new(_id, _dependentTask, _dependsOnTask, _customerEmail, _shippingAddress, _orderCreationDate, _shippingDate, _deliveryDate);
            return _newDependency;
        }
    }
}
