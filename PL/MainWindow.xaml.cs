using PL.Engineer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MainWindow()
    {
        InitializeComponent();
    }

    private void btnAdminUser_Click(object sender, RoutedEventArgs e)
    {
        new AdminUser.AdminUserWindow().ShowDialog();
    }

    private void btnEngineerUser_Click(object sender, RoutedEventArgs e)
    {
        //new EngineerUserWindow().ShowDialog();
        //TODO: Uncomment once implemented
    }

    private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to initialize the database?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            DalTest.Initialization.Do();
        }
    }

    private void btnResetSystem_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to reset the system?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
            s_bl.Config.Reset();
        }
    }

    private void btnChangeSystemClock_Click(object sender, RoutedEventArgs e)
    {
        //new SystemClockWindow().ShowDialog();
        //TODO: Uncomment once implemented
    }


}