using Dal;
using DalApi;

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
                bool exit = false;
                while (!exit)
                {
                    Console.WriteLine("0. Exit the main menu\n1. Test out Engineer\n2. Test out Task\n3. Test out Dependency");
                    string? input = Console.ReadLine();
                    switch (input)
                    {
                        case "0":
                            exit = true;
                            break;
                        case "1":
                            Console.WriteLine("" +
                                "a) Go back\n" +
                                "b) Add an engineer to the engineer list\n" +
                                "c) Display an engineer using an engineer's id\n" +
                                "d) Display the engineer list\n" +
                                "e) Update an engineer object\n" +
                                "f) Delete an engineer from the engineer list\n");
                            break;
                        case "2":
                            Console.WriteLine("" +
                                "a) Go back\n" +
                                "b) Add a task to the task list\n" +
                                "c) Display a task using a task's id\n" +
                                "d) Display the task list\n" +
                                "e) Update a task object\n" +
                                "f) Delete a task from the task list\n");
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
    }
}
