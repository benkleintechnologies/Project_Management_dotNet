using PL.EngineerUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.AdminUser;

/// <summary>
/// Interaction logic for AdminUserWindow.xaml
/// </summary>
public partial class AdminUserWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public AdminUserWindow()
    {
        InitializeComponent();
    }

    private void btnEngineers_Click(object sender, RoutedEventArgs e)
    {
        new Engineer.EngineerListWindow().ShowDialog();
    }

    private void btnTasks_Click(object sender, RoutedEventArgs e)
    {
        new Task.TaskListWindow().ShowDialog();
    }

    private void btnMilestones_Click(object sender, RoutedEventArgs e)
    {
        new Milestone.MilestoneListWindow().ShowDialog();
    }

    private void btnCreateSchedule_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl?.Milestone.CreateProjectSchedule();
            MessageBox.Show("The project schedule has been created");
        }
        catch (BO.BlDoesNotExistException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (BO.BlCannotBeDeletedException ex)
        {
            MessageBox.Show(ex.Message);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void btnCreateGantt_Click(object sender, RoutedEventArgs e)
    {
        //Check that all tasks have start and end dates
        if (s_bl.Config.InProduction() && s_bl.Task.AllTaskDatesSet())
            new Gantt.GanttChartWindow().ShowDialog();
        else if (s_bl.Config.InProduction())
            MessageBox.Show("The Project schedule was not successfully created, so the Gantt Chart can't be displayed.");
        else
            MessageBox.Show("The project start date or end date is not set. Please set the project start date and end date in the configuration window.");
    }

    private void btnResetSystem_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to reset the system?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            s_bl.Config.Reset();
        }
    }

    private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to initialize the database?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            DalTest.Initialization.Do();
        }
    }

    private void btnChangeSystemClock_Click(object sender, RoutedEventArgs e)
    {
        new Clock.SystemClockWindow().ShowDialog();
    }

    private void btnSetProjectDates_Click(object sender, RoutedEventArgs e)
    {
        new Clock.ProjectDatesWindow().ShowDialog();
    }

    private void btnEngineerUser_Click(object sender, RoutedEventArgs e)
    {
        // Display a message box prompting the user to enter an engineer ID
        string userInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the ID of the engineer you wish to find:", "Engineer ID", "");

        // Check if the user clicked cancel or provided an empty input
        if (userInput == null || userInput == "")
        {
            // User canceled or provided empty input, do nothing
            return;
        }

        // Check if the user entered a valid ID
        if (int.TryParse(userInput, out int engineerID))
        {
            try
            {
                // Check if there is an engineer with this ID and if it does then go to the Engineer View
                BO.Engineer? desiredEngineer = s_bl.Engineer.GetEngineer(engineerID);
                int validID = desiredEngineer.ID;
                new EngineerUserWindow(validID).ShowDialog();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Invalid input. Please enter a valid engineer ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
