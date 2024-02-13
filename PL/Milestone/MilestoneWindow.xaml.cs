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

namespace PL.Milestone;

/// <summary>
/// Interaction logic for MilestoneWindow.xaml
/// </summary>
public partial class MilestoneWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public MilestoneWindow(int id = 0)
    {
        if (id != 0)
        {
            try
            {
                CurrentMilestone = s_bl?.Milestone.GetMilestone(id)!;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            CurrentMilestone = new BO.Milestone(0, "", "", null, BO.Status.Unscheduled, null, null, null, 0, "", null);
        }
        InitializeComponent();
    }

    public BO.Milestone CurrentMilestone
    {
        get { return (BO.Milestone)GetValue(MyMilestoneProperty); }
        set { SetValue(MyMilestoneProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyMilestoneProperty.
    public static readonly DependencyProperty MyMilestoneProperty =
        DependencyProperty.Register("CurrentMilestone", typeof(BO.Milestone), typeof(MilestoneWindow), new PropertyMetadata(null));

    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        //Update the milestone in the BL layer
        s_bl?.Milestone.UpdateMilestone(CurrentMilestone.ID, CurrentMilestone.Name, CurrentMilestone.Description, CurrentMilestone.Notes);
        //Check if the milestone was updated
        try
        {
            if (s_bl?.Milestone.GetMilestone(CurrentMilestone.ID) != CurrentMilestone)
            {
                MessageBox.Show("The milestone was not updated");
            }
            else
            {
                MessageBox.Show("The milestone was updated");
            }
        }
        catch (BO.BlDoesNotExistException)
        {
            MessageBox.Show("The Milestone was not updated in the database.");
        }
    }
}
