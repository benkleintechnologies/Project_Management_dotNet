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
    }
    private void AddRow()
    {
        DataRow dr = DataTable.NewRow();
        dr["Test"] = "Wert1";
        DataTable.Rows.Add(dr);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        List<TaskInList> tasks = s_bl.Task.GetListOfTasks().ToList();

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
        DataTable.Columns.Add("TASK NAME");
        for (int i = 0; i <= diffResult; i++)
        {
            DataTable.Columns.Add(startDate.AddDays(i).ToString("MMM dd yy"));
        }

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
                        dr[j] = tasks[i].Name!.ToString();
                    }

                }
                else if (Convert.ToDateTime(DataTable.Columns[j].ToString(), new CultureInfo("en-US")) >= taskStartDate && Convert.ToDateTime(DataTable.Columns[j].ToString(), new CultureInfo("en-US")) <= taskEndDate) //if the date is between the start and end date of the task
                {
                    dr[j] = tasks[i].Status.ToString();
                }
            }

        }
        DataTable.Rows.Add(dr);
        FooBar1.ItemsSource = DataTable.DefaultView;
    }
}