using PL.Engineer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
public partial class TaskListWindow : Window, INotifyPropertyChanged
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    //The filter for the list of tasks
    private Func<BO.Task, bool>? _filter = null;
    //The highlighted/selected Task
    private BO.TaskInList? _selectedTask = null;
    //The Engineer who opened this window
    private BO.Engineer? _engineer = null;
    //The Tasks which opened this window of dependencies
    private BO.Task? _dependentTask = null;
    //The bool representing whether the window is in Task mode or Dependency mode
    private bool _isDependencyMode = false;
    //Window title
    private string _windowTitle = "Task List";
    //The bool representing whether in Adding Dependency mode
    private bool _addingDependencies = false;
    //The text for the Add/Select button
    private string _addSelectButtonText = "Add";


    public TaskListWindow(Func<BO.Task, bool>? filter = null, int engineerID = 0, BO.Task? dependentTask = null, bool addingDependencies = false)
    {
        _filter = filter;
        if (filter != null)
        {
            FilterActive = false;
            _addSelectButtonText = "Select Task";
        }

        //Check if we're dealing with an Engineer's Tasks
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

        //Check if we're dealing with a Task's dependencies
        if (dependentTask != null)
        {
            _dependentTask = dependentTask;
            IsDependencyMode = true;
            
            //Check if we're adding dependencies
            if (addingDependencies)
            {
                //Activate proper buttons
                AddingDependencyMode = true;
                AddSelectButtonText = "Add Dependency";
            }
        }



        try
        {
            TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
        }
        catch (BO.BlDoesNotExistException) // when no tasks exist
        {
            TaskList = Enumerable.Empty<BO.TaskInList>();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        InitializeComponent();
    }

    // Event declaration for dependency added with custom event arguments
    public event EventHandler<RoutedEventArgs> DependencyAdded;

    // Method to raise the dependency added event with the relevant information
    protected virtual void OnDependencyAdded()
    {
        DependencyAdded?.Invoke(this, new RoutedEventArgs());
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    //The event handler for the window activation
    private void activated(object sender, EventArgs e)
    {
        TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
    }

    public string WindowTitle
    {
        get { return _windowTitle; }
        set { _windowTitle = value; }
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

    //Getters and setters for the bool representing whether the window is in Task mode or Dependency mode
    public bool IsDependencyMode
    {
        get { return _isDependencyMode; }
        set
        {
            _isDependencyMode = value;
            OnPropertyChanged(nameof(IsDependencyMode));
            OnPropertyChanged(nameof(AddSelectButtonVisibility));
            OnPropertyChanged(nameof(DependencyButtonVisibility));
            SetWindowTitle();
        }
    }

    public bool AddingDependencyMode
    {
        get { return _addingDependencies; }
        set
        {
            _addingDependencies = value;
            OnPropertyChanged(nameof(AddingDependencyMode));
            OnPropertyChanged(nameof(AddSelectButtonVisibility));
            OnPropertyChanged(nameof(DependencyButtonVisibility));
            SetWindowTitle();
        }
    }

    public string AddSelectButtonText
    {
        get { return _addSelectButtonText; }
        set
        {
            _addSelectButtonText = value;
            OnPropertyChanged(nameof(AddSelectButtonText));
        }
    }

    //Visibility of the Add/Select buttons
    public Visibility AddSelectButtonVisibility => IsDependencyMode && !AddingDependencyMode ? Visibility.Collapsed : Visibility.Visible;
    //Visibility of the Dependency buttons
    public Visibility DependencyButtonVisibility => IsDependencyMode && !AddingDependencyMode ? Visibility.Visible : Visibility.Collapsed;

    //The selected task experience level
    public BO.EngineerExperience ExperienceLevel { get; set; } = BO.EngineerExperience.All;

    //The event handler for the selection of the experience level (to filter the list of tasks)
    private void cbDifficultySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (_filter == null)
                TaskList = ((ExperienceLevel == BO.EngineerExperience.All) ?
                                    s_bl?.Task.GetListOfTasks()! : s_bl?.Task.GetListOfTasks(item => item.Complexity == ExperienceLevel)!);
        }
        catch (BO.BlDoesNotExistException)
        {
            TaskList = Enumerable.Empty<BO.TaskInList>();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    //Change the window title based on the mode
    private void SetWindowTitle()
    {
        if (IsDependencyMode)
        {
            if (AddingDependencyMode)
                WindowTitle = "Add Dependency";
            else
                WindowTitle = "Task Dependencies";
        }
        else
        {
            if (FilterActive)
                WindowTitle = "Task List";
            else
                WindowTitle = "Tasks for Engineer";
        }
    }

    private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        //Disable when viewing dependencies
        if (_dependentTask != null)
            return;

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
            
            if (_selectedTask != null)
            {
                if (_engineer is not null)//Adding/assigning the selected task to the engineer
                {
                    try
                    {
                        BO.Task taskToAssign = s_bl.Task.GetTask(_selectedTask.ID);
                        if (taskToAssign != null)
                        {
                            taskToAssign.ActualStartDate = s_bl.Config.GetSystemClock();
                            taskToAssign.Engineer = new BO.EngineerInTask(_engineer.ID, _engineer.Name);
                            s_bl.Task.UpdateTask(taskToAssign);
                            OnPropertyChanged(nameof(TaskList));
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
                else if (_dependentTask is not null) //Adding the selected task as a dependency
                {
                    try
                    {
                        //Add the dependency
                        _dependentTask.Dependencies = _dependentTask.Dependencies!.Append(_selectedTask);
                        s_bl.Task.UpdateTask(_dependentTask);
                        TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
                        MessageBox.Show("Dependency added");
                        OnDependencyAdded();
                    }
                    catch (BO.BlDoesNotExistException)
                    {
                        MessageBox.Show("Dependency not found");
                    }
                    catch (BO.BlInvalidInputException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (BO.BlCircularDependencyException ex)
                    {
                        //Remove the dependency because it would cause a circular dependency
                        _dependentTask.Dependencies = _dependentTask.Dependencies!.Where(d => d.ID != _selectedTask.ID);
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("No Task or Engineer selected");
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

    private void btnRemoveDependency_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedTask != null)
        {
            if (_dependentTask is not null)
            {
                try
                {
                    //Remove the dependency
                    _dependentTask.Dependencies = _dependentTask.Dependencies!.Where(d => d.ID != _selectedTask.ID);
                    s_bl.Task.UpdateTask(_dependentTask);
                    _filter = (task) => _dependentTask.Dependencies.Any((dep) => dep.ID == task.ID);
                    TaskList = s_bl?.Task.GetListOfTasks(_filter)!;
                    MessageBox.Show("Dependency removed");
                }
                catch (BO.BlDoesNotExistException)
                {
                    MessageBox.Show("Dependency not found");
                }
                catch (BO.BlInvalidInputException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (BO.BlCircularDependencyException ex)
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
                MessageBox.Show("Task not found");
            }
        }
        else
        {
            MessageBox.Show("No task selected");
        }
    }

    private void btnAddDependency_Click(object sender, RoutedEventArgs e)
    {
        if (_dependentTask.Dependencies is null)
            _dependentTask.Dependencies = new List<BO.TaskInList>();
        var childWindow = new TaskListWindow((task) => _dependentTask.Dependencies.All((dep) => dep.ID != task.ID), dependentTask: _dependentTask, addingDependencies: true);

        // Subscribe to the DependencyAdded event in the child window
        childWindow.DependencyAdded += ChildWindow_DependencyAdded;

        // Show the child window
        childWindow.ShowDialog();
    }

    // Event handler for the DependencyAdded event in the child window
    private void ChildWindow_DependencyAdded(object sender, RoutedEventArgs e)
    {
        //Update the filter to include the new dependency
        _filter = (task) => _dependentTask.Dependencies.Any((dep) => dep.ID == task.ID);
        TaskList = s_bl?.Task.GetListOfTasks(_filter)!;

        // Close the child window
        (sender as TaskListWindow)?.Close();
    }
}