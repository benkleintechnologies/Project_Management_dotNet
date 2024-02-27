using PL.Engineer;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PL.EngineerUser
{
    /// <summary>
    /// Interaction logic for EngineerUserWindow.xaml
    /// </summary>
    public partial class EngineerUserWindow : Window, INotifyPropertyChanged
    {
        //The BL instance
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Event that gets raised when a property value changes
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _currentTask;
        public string CurrentTask
        {
            get { return _currentTask; }
            set
            {
                if (_currentTask != value)
                {
                    _currentTask = value;
                    // Notify the UI that the property has changed
                    OnPropertyChanged(nameof(CurrentTask));
                }
            }
        }

        // Property to represent whether the task can be marked as complete
        private bool _markTaskComplete;
        public bool MarkTaskComplete
        {
            get { return _markTaskComplete; }
            set
            {
                if (_markTaskComplete != value)
                {
                    _markTaskComplete = value;
                    // Notify the UI that the property has changed
                    OnPropertyChanged(nameof(MarkTaskComplete));
                }
            }
        }

        // Property to represent whether relevant tasks can be viewed
        private bool _viewRelevantTasks;
        public bool ViewRelevantTasks
        {
            get { return _viewRelevantTasks; }
            set
            {
                if (_viewRelevantTasks != value)
                {
                    _viewRelevantTasks = value;
                    // Notify the UI that the property has changed
                    OnPropertyChanged(nameof(ViewRelevantTasks));
                }
            }
        }

        private BO.Task? _taskToUse;
        private int _engineerID = 0;

        public EngineerUserWindow(int id)
        {
            _engineerID = id;
            InitializeComponent();
            LoadTask(id);
        }

        private void LoadTask(int id)
        {
            BO.TaskInEngineer? taskInEngineer = s_bl.Engineer.GetEngineer(id).Task;
            if (taskInEngineer != null)
            {
                _taskToUse = s_bl.Task.GetTask(taskInEngineer!.ID);
                CurrentTask = _taskToUse.ToString();
                MarkTaskComplete = true;
                ViewRelevantTasks = false;
            }
            else
            {
                CurrentTask = "No task assigned to this engineer";
                MarkTaskComplete = false;
                ViewRelevantTasks = true;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnMarkTaskCompleted_Click(object sender, RoutedEventArgs e)
        {
            _taskToUse!.ActualEndDate = s_bl.Config.GetSystemClock();
            _taskToUse!.Engineer = null;
            s_bl.Task.UpdateTask(_taskToUse);
            MessageBox.Show("Marked task complete");
            LoadTask(_engineerID); // Reload the task after marking it complete
        }

        private void btnViewRelevantTasks_Click(object sender, RoutedEventArgs e)
        {
            BO.Engineer? engineer = s_bl.Engineer.GetEngineer(_engineerID);
            new Task.TaskListWindow((item => 
                item.Complexity <= engineer.Experience &&
                item.Status == BO.Status.Scheduled &&
                s_bl.Milestone.GetMilestone(item.Milestone.ID).Dependencies.All(dep => dep.Status == BO.Status.Done)),
                _engineerID
            ).ShowDialog();
        }

        private void openEngineerView(object sender, MouseButtonEventArgs e)
        {
            if (_taskToUse != null && _taskToUse.Engineer != null)
                new EngineerWindow(_taskToUse.Engineer.ID).ShowDialog();
        }

        private void activated(object sender, EventArgs e)
        {
            LoadTask(_engineerID);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CurrentTask));
        }
    }
}
