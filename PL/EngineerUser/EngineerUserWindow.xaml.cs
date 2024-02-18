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

        public EngineerUserWindow(int id)
        {
            InitializeComponent();
            BO.TaskInEngineer? taskInEngineer = s_bl.Engineer.GetEngineer(id).Task;
            if (taskInEngineer != null)
            {
                _taskToUse = s_bl.Task.GetTask(taskInEngineer!.ID);
                CurrentTask = _taskToUse.ToString();
                MarkTaskComplete = true; // task exists so could mark completed
                ViewRelevantTasks = false;

            }
            else
            {
                CurrentTask = "No task assigned to this engineer";
                MarkTaskComplete = false;
                ViewRelevantTasks = true; // no task so look for relevant tasks
            }

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnMarkTaskCompleted_Click(object sender, RoutedEventArgs e)
        {
            _taskToUse!.ActualEndDate = s_bl.Config.GetSystemClock();
            s_bl.Task.UpdateTask(_taskToUse);
            MessageBox.Show("Marked task complete");
        }

        private void btnViewRelevantTasks_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
