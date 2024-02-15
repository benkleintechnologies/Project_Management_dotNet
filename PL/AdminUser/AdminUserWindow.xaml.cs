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
        //new GanttChartWindow().ShowDialog();
        //TODO: Uncomment once implemented
    }

    private void btnAddUser_Click(object sender, RoutedEventArgs e)
    {
        //new AddUserWindow().ShowDialog();
        //TODO: Uncomment once implemented
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
}
