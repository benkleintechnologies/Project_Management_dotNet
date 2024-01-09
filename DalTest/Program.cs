using Dal;
using DalApi;
using DO;
using System.Reflection.Emit;
using System.Xml.Linq;

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
                    int _inputNumber;
                    int.TryParse(_input, out _inputNumber);
                    switch (_inputNumber)
                    {
                        case 0:
                            _exit = true;
                            break;
                        case 1:
                            EngineerOptionsPrint();
                            string _userInput = Console.ReadLine();
                            EngineerOptionsSwitch(_userInput);
                            break;
                        case 2:
                            Console.WriteLine("" +
                                "a) Go back\n" +
                                "b) Add a task to the task list\n" +
                                "c) Display a task using a task's id\n" +
                                "d) Display the task list\n" +
                                "e) Update a task object\n" +
                                "f) Delete a task from the task list\n" +
                                "g) Reset tasks");
                            break;
                        case 3:
                            Console.WriteLine("" +
                                "a) Go back\n" +
                                "b) Add a dependency to the dependency list\n" +
                                "c) Display a dependency using a dependency's id\n" +
                                "d) Display the dependency list\n" +
                                "e) Update a dependency object\n" +
                                "f) Delete a dependecy from the task list\n" +
                                "g) Reset dependencies");
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
                                "e) Update an engineer object\n" +
                                "f) Delete an engineer from the engineer list\n" +
                                "g) Reset engineers");
        }
        static void EngineerOptionsSwitch(string _userInput)
        {
            int _id;
            switch (_userInput)
            {
                case "a":
                    break;
                case "b":
                    Console.WriteLine("Enter the id, name, email, cost, and experience level:");
                    int.TryParse(Console.ReadLine(), out _id);
                    String _name = Console.ReadLine();
                    String _email = Console.ReadLine();
                    double _cost;
                    double.TryParse(Console.ReadLine(), out _cost);
                    EngineerExperience _level;
                    Enum.TryParse(Console.ReadLine(), out _level);
                    Engineer _newEngineer = new(_id, _name, _email, _cost, _level);
                    s_dalEngineer.Create(_newEngineer);
                    break;
                case "c":
                    Console.WriteLine("Enter the id of an engineer you would like to display:");
                    int.TryParse(Console.ReadLine(), out _id);
                    Engineer engineerToPrint = s_dalEngineer.Read(_id);
                    Console.WriteLine(engineerToPrint);
                    break;
                case "d":
                    List<Engineer> _engineerList =  s_dalEngineer.ReadAll();
                    foreach(Engineer _engineer in _engineerList )
                    {
                        Console.WriteLine(_engineer);
                    }
                    break;
                case "e": // Update engineer object

            }
        }
    }
}
