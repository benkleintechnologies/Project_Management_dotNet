using PL.Engineer;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
/// Interaction logic for TaskWindow.xaml
/// </summary>
public partial class TaskWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public TaskWindow(int id = 0)
    {
        if (id != 0)
        {
            try
            {
                CurrentTask = s_bl?.Task.GetTask(id)!;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {   
            CurrentTask = new BO.Task(id: 0, name: "", null, status: BO.Status.Unscheduled, null, null, null, null, null, null, null, null, null, null, null, null, complexity: BO.EngineerExperience.Beginner);
        }
        InitializeComponent();
    }

    public BO.Task CurrentTask
    {
        get { return (BO.Task)GetValue(MyTaskProperty); }
        set { SetValue(MyTaskProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyEngineerProperty.
    public static readonly DependencyProperty MyTaskProperty =
        DependencyProperty.Register("CurrentTask", typeof(BO.Task), typeof(TaskWindow), new PropertyMetadata(null));

    private void btnDependencies_Click(object sender, RoutedEventArgs e)
    {
        // Get all the dependencies of the task
        IEnumerable<BO.TaskInList>? dependencies = CurrentTask.Dependencies;
        if (dependencies != null)
        {
            // Open a new window to display the dependencies
            TaskListWindow taskListWindow = new TaskListWindow((task) => dependencies.Any((dep) => dep.ID == task.ID), dependentTask: CurrentTask);
            taskListWindow.Show();
        }
        else
        {
            //Open the window to add dependencies
            TaskListWindow taskListWindow = new TaskListWindow((task) => false, dependentTask: CurrentTask);
            taskListWindow.Show();
        }
    }

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        // Call appropriate method in BL layer based on Add or Update mode
        Button clickedButton = sender as Button;
        if (clickedButton != null)
        {
            string buttonText = clickedButton.Content.ToString();
            try
            {
                if (buttonText == "Add")
                {
                    // Call the method for adding the item
                    s_bl?.Task.AddTask(CurrentTask);
                    // Check that it was added and then close the window
                    try
                    {
                        s_bl?.Task.GetTask(CurrentTask.ID);
                        MessageBox.Show("The Task was added to the database.");
                        this.Close();
                    }
                    catch (BO.BlDoesNotExistException)
                    {
                        MessageBox.Show("The Task was not added to the database.");
                    }
                }
                else if (buttonText == "Update")
                {
                    // Call the method for updating the item
                    s_bl?.Task.UpdateTask(CurrentTask);
                    // Check that it was updated and then close the window
                    try
                    {
                        //Check that the task was updated
                        BO.Task task = s_bl?.Task.GetTask(CurrentTask.ID)!;
                        if (task.Name != CurrentTask.Name ||
                            task.Description != CurrentTask.Description ||
                            task.Status != CurrentTask.Status ||
                            !Enumerable.SequenceEqual(task.Dependencies ?? Enumerable.Empty<BO.TaskInList>(), CurrentTask.Dependencies ?? Enumerable.Empty<BO.TaskInList>()) ||
                            task.Milestone != CurrentTask.Milestone ||
                            task.CreatedAtDate != CurrentTask.CreatedAtDate ||
                            task.ProjectedStartDate != CurrentTask.ProjectedStartDate ||
                            task.ActualStartDate != CurrentTask.ActualStartDate ||
                            task.ProjectedEndDate != CurrentTask.ProjectedEndDate ||
                            task.Deadline != CurrentTask.Deadline ||
                            task.ActualEndDate != CurrentTask.ActualEndDate ||
                            task.RequiredEffortTime != CurrentTask.RequiredEffortTime ||
                            task.Deliverables != CurrentTask.Deliverables ||
                            task.Notes != CurrentTask.Notes ||
                            task.Engineer != CurrentTask.Engineer ||
                            task.Complexity != CurrentTask.Complexity)
                        {
                            MessageBox.Show("The Task was not updated in the database.");
                        }
                        else
                        {
                            MessageBox.Show("The Task was updated in the database.");
                        }
                        this.Close();
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        MessageBox.Show("The Task was not updated in the database.");
                    }
                }
            }
            catch (BO.BlAlreadyExistsException)
            {
                MessageBox.Show("The Task could not be added because there is already another Task with this ID.");
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("The Task could not be updated because there is no Task with this ID.");
            }
            catch (BO.BlInvalidInputException)
            {
                MessageBox.Show("One of the fields you entered was not valid.");
            }
        }
        else
        {
            MessageBox.Show("Error: Button not found");
        }
    }
}
