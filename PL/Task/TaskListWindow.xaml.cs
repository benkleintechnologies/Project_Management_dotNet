﻿using PL.Engineer;
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
    
    public TaskListWindow()
    {
        InitializeComponent();
    }
    
    //The event handler for the window activation
    private void activated(object sender, EventArgs e)
    {
        TaskList = s_bl?.Task.GetListOfTasks()!;
    }

    //Getters and setters for the list of tasks
    public IEnumerable<BO.TaskInList> TaskList
    {
        get { return (IEnumerable<BO.TaskInList>)GetValue(TaskListProperty); }
        set { SetValue(TaskListProperty, value); }
    }

    //Dependency Property to connect the list of engineers to the window
    public static readonly DependencyProperty TaskListProperty =
        DependencyProperty.Register("TaskInList", typeof(IEnumerable<BO.TaskInList>), typeof(TaskListWindow), new PropertyMetadata(null));

    //The selected task experience level
    public BO.EngineerExperience ExperienceLevel { get; set; } = BO.EngineerExperience.All;

    //The event handler for the selection of the experience level (to filter the list of tasks)
    private void cbDifficultySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
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

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new TaskWindow().ShowDialog();
    }
}
