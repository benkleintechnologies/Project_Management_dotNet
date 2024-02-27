using System.Windows;

namespace PL.Clock;

/// <summary>
/// Interaction logic for ProjectDatesWindow.xaml
/// </summary>
public partial class ProjectDatesWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    //The values of the Date Pickers
    public DateTime? ProjectStartDate { get; set; }
    public DateTime? ProjectEndDate { get; set; }
 

    public ProjectDatesWindow()
    {
        ProjectStartDate = s_bl.Config.GetProjectStartDate();
        ProjectEndDate = s_bl.Config.GetProjectEndDate();

        InitializeComponent();
    }

    private void btnSetDates_Click(object sender, RoutedEventArgs e)
    {
        // Access SelectedDate of DatePicker controls directly
        DateTime? startDate = ProjectStartDate;
        DateTime? endDate = ProjectEndDate;
        if (startDate == null || endDate == null)
        {
            MessageBox.Show("Please select both start and end dates");
            return;
        }
        if (startDate.HasValue && endDate.HasValue)
        {
            try
            {
                s_bl?.Config.SetProjectStartDate(startDate ?? DateTime.MinValue);
                s_bl?.Config.SetProjectEndDate(endDate ?? DateTime.MaxValue);
                MessageBox.Show("Project dates were set successfully");
                this.Close();
            }
            catch (BO.BlCannotChangeDateException ex)
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
            MessageBox.Show("Please select both start and end dates");
        }
    }
}
