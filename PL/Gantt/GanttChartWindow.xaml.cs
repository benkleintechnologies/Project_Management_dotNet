using BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Gantt;

/// <summary>
/// Interaction logic for GanttChartWindow.xaml
/// </summary>
public partial class GanttChartWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public DataTable DataTable { get; set; }
    public DataView DataView { get; set; }


    public GanttChartWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        List<BO.TaskInList> tasks = s_bl.Task.GetListOfTasks().ToList();
        List<BO.Milestone> milestones = s_bl.Milestone.GetListOfMilestones().Select(m => s_bl.Milestone.GetMilestone(m.ID)).ToList();
        List<DateTime> milestoneEndDates = milestones.Select(m => m.ActualEndDate ?? m.ProjectedEndDate ?? DateTime.MinValue).ToList();

        DataTable = new DataTable();

        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now;

        if (s_bl.Config.GetProjectStartDate() is not null && s_bl.Config.GetProjectEndDate() is not null)
        {
            startDate = s_bl.Config.GetProjectStartDate()!.Value;
            endDate = s_bl.Config.GetProjectEndDate()!.Value;
        }
        else
        {
            MessageBox.Show("The project start date or end date is not set. Please set the project start date and end date in the configuration window.");
            this.Close();
        }
        
        double diffResult = (endDate - startDate).TotalDays;
        //Add a column for the task name
        DataTable.Columns.Add("TASK NAME");
        //Add a column for each day between the start and end date
        for (int i = 0; i <= diffResult; i++)
        {
            //Add a column for each milestone
            if (milestoneEndDates.Any(m => m.Date == startDate.AddDays(i).Date))
            {
                DataTable.Columns.Add(milestones.First(m => m.ActualEndDate == startDate.AddDays(i) || m.ProjectedEndDate == startDate.AddDays(i)).Name);
            }
            //TODO: Change the width of the column just added to 20

            DataTable.Columns.Add(startDate.AddDays(i).ToString("MMM dd yy"));
        }

        //Add a row for each task
        DataRow dr = DataTable.NewRow();
        for (int i = 0; i < tasks.Count(); i++)
        {
            //Get the Task's start and end date
            BO.Task task = s_bl.Task.GetTask(tasks[i].ID);
            DateTime? taskStartDate = task.ActualStartDate ?? task.ProjectedStartDate;
            DateTime? taskEndDate = task.ActualEndDate ?? task.ProjectedEndDate;
            if (taskStartDate is null || taskEndDate is null)
            {
                MessageBox.Show("The task " + tasks[i].Name + " does not have a start or end date. Please set the start and end date for all tasks.");
                this.Close();
            }

            //Go through each column and add the task status to the corresponding dates
            for (int j = 0; j < DataTable.Columns.Count; j++)
            {
                if (DataTable.Columns[j].ToString() == "TASK NAME")
                {
                    if (dr[j].ToString() != tasks[i].Name!.ToString())
                    {
                        if (dr[j].ToString() != "")
                        {
                            DataTable.Rows.Add(dr);
                            dr = DataTable.NewRow();
                        }
                        dr[j] = tasks[i].Name!.ToString(); //Add the task name to the row
                    }
                }
                else if (milestones.Any(m => m.Name == DataTable.Columns[j].ToString())) //if the column is a milestone
                {
                    dr[j] = milestones.First(m => m.Name == DataTable.Columns[j].ToString()).Status.ToString();
                }
                else if (Convert.ToDateTime(DataTable.Columns[j].ToString(), new CultureInfo("en-US")) >= taskStartDate && Convert.ToDateTime(DataTable.Columns[j].ToString(), new CultureInfo("en-US")) <= taskEndDate) //if the date is between the start and end date of the task
                {
                    dr[j] = tasks[i].Status.ToString();
                }
            }

        }
        DataTable.Rows.Add(dr);
        DataView = DataTable.DefaultView;
    }

    private void DataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        // Ensure that the DataView is not null before setting it as the ItemsSource
        if (DataView != null)
        {
            ((DataGrid)sender).ItemsSource = DataView;
        }
    }
}