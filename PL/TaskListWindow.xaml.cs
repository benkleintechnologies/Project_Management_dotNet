using PL.Engineer;
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

namespace PL.Task;

/// <summary>
/// Interaction logic for TaskListWindow.xaml
/// </summary>
public partial class TaskListWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    //The filter for the list of tasks
    private Func<BO.Task, bool>? _filter = null;
    //The highlighted/selected Task
    private BO.TaskInList? _selectedTask = null;
    //The Engineer who opened this window
    private BO.Engineer? _engineer = null;

    public TaskListWindow(Func<BO.Task, bool>? filter = null, int engineerID = 0)
    {
        _filter = filter;
        if (filter != null)
            FilterActive = false;
        if (engineerID != 0)
        {
            try
            {
                _engineer = s_bl.Engineer.GetEngineer(engineerID);
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("Engineer not found");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
            
        TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
        InitializeComponent();
    }
    
    //The event handler for the window activation
    private void activated(object sender, EventArgs e)
    {
        TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
    }

    //Getters and setters for the list of tasks
    public IEnumerable<BO.TaskInList> TaskList
    {
        get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
        set { SetValue(TaskListProperty, value); }
    }

    //Getters and setters for the bool representing whether the filter is active
    public bool FilterActive
    {
        get { return (bool)GetValue(FilterActiveProperty); }
        set { SetValue(FilterActiveProperty, value); }
    }

    //Dependency Property to connect the list of tasks to the window
    public static readonly DependencyProperty TaskListProperty =
        DependencyProperty.Register("TaskList", typeof(IEnumerable<BO.TaskInList>), typeof(TaskListWindow), new PropertyMetadata(null));

    //Dependency Property to connect the bool representing whether the filter is active to the window
    public static readonly DependencyProperty FilterActiveProperty =
        DependencyProperty.Register("FilterActive", typeof(bool), typeof(TaskListWindow), new PropertyMetadata(true));

    //The selected task experience level
    public BO.EngineerExperience ExperienceLevel { get; set; } = BO.EngineerExperience.All;

    //The event handler for the selection of the experience level (to filter the list of tasks)
    private void cbDifficultySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_filter == null)
            TaskList = ((ExperienceLevel == BO.EngineerExperience.All) ?
                                s_bl?.Task.GetListOfTasks()! : s_bl?.Task.GetListOfTasks(item => item.Complexity == ExperienceLevel)!);
    }    
    private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Extract the selected item from the ListView
        BO.TaskInList? task = (sender as ListView)?.SelectedItem as BO.TaskInList;

        if (task != null)
        {
            new TaskWindow(task.ID).ShowDialog();
        }
    }

    private void btnAddOrSelect_Click(object sender, RoutedEventArgs e)
    {
        if (FilterActive)
        {
            new TaskWindow().ShowDialog();
        }
        else
        {
            //Adding/assigning the selected task to the engineer
            if (_selectedTask != null)
            {
                if (_engineer is not null)
                {
                    try
                    {
                        BO.Task taskToAssign = s_bl.Task.GetTask(_selectedTask.ID);
                        if (taskToAssign != null)
                        {
                            taskToAssign.ActualStartDate = s_bl.Config.GetSystemClock();
                            taskToAssign.Engineer = new BO.EngineerInTask(_engineer.ID, _engineer.Name);
                            s_bl.Task.UpdateTask(taskToAssign);
                        }
                        MessageBox.Show("Task assigned to engineer");
                    }
                    catch (BO.BlDoesNotExistException)
                    {
                        MessageBox.Show("Engineer not found");
                    }
                    catch (BO.BlInvalidInputException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Engineer not found");
                }
            }
            else
            {
                MessageBox.Show("No task selected");
            }
            Close();
        }
    }

    private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ListViewItem? listViewItem = sender as ListViewItem;
        if (listViewItem != null && !FilterActive)
        {
            _selectedTask = listViewItem.DataContext as BO.TaskInList;
        }
    }
}
