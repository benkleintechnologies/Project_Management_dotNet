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

namespace PL.Milestone;

    /// <summary>
    /// Interaction logic for MilestoneListWindow.xaml
    /// </summary>
public partial class MilestoneListWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MilestoneListWindow()
    {
        InitializeComponent();
    }

    //The event handler for the window activation
    private void activated(object sender, EventArgs e)
    {
        MilestoneList = s_bl?.Milestone.GetListOfMilestones()!;
    }

    //Getters and setters for the list of milestones
    public IEnumerable<BO.MilestoneInList> MilestoneList
    {
        get { return (IEnumerable<BO.MilestoneInList>)GetValue(MilestoneListProperty); }
        set { SetValue(MilestoneListProperty, value); }
    }

    //Dependency Property to connect the list of engineers to the window
    public static readonly DependencyProperty MilestoneListProperty =
        DependencyProperty.Register("MilestoneList", typeof(IEnumerable<BO.MilestoneInList>), typeof(MilestoneListWindow), new PropertyMetadata(null));

    private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Extract the selected item from the ListView
        BO.MilestoneInList? milestone = (sender as ListView)?.SelectedItem as BO.MilestoneInList;

        if (milestone != null)
        {
            new MilestoneWindow(milestone.ID).ShowDialog();
        }
    }
}
