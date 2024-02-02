namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        Console.Write("Would you like to create Initial data? (Y/N)");
        string? ans = Console.ReadLine() ?? throw new FormatException("Wrong input");
        if (ans == "Y")
            DalTest.Initialization.Do();

        try
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("0. Exit the main menu\n1. Set project start and end date\n2. Generate milestones and project schedule\n3. Edit Engineers\n4. Edit Tasks\n5. Assign Engineer to a Task\n6. Edit Milestones\n7. Reset Project\n");
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
                        break;
                    case 2: // Generate milestones and project schedule

                        break;
                    case 3: // Edit Engineers
                        break;
                    case 4: // Edit Tasks
                        break;
                    case 5: // Assign an Engineer to a Task
                        break;
                    case 6: // Edit Milestone
                        break;
                    case 7: // Reset project
                        break;
                    default:
                        Console.WriteLine("Incorrect input, try again\n");
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
}
