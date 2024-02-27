using PL.Engineer;
using PL.EngineerUser;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
public partial class MainWindow : Window, INotifyPropertyChanged
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    //System clock value
    private DateTime _systemClock;

    public MainWindow()
    {
        InitializeComponent();
    }

    public DateTime SystemClock
    {
        get { return _systemClock; }
        set
        {
            _systemClock = value;
            OnPropertyChanged();
        }
    }

    private bool _isInProduction;
    public bool IsInProduction
    {
        get { return _isInProduction; }
        set
        {
            _isInProduction = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void btnAdminUser_Click(object sender, RoutedEventArgs e)
    {
        new AdminUser.AdminUserWindow().ShowDialog();
    }

    private void btnEngineerUser_Click(object sender, RoutedEventArgs e)
    {
        if (!IsInProduction)
        {
            MessageBox.Show("The system is not in production. Please set the system clock and project dates before proceeding.");
            return;
        }
            
        // Display a message box prompting the user to enter an engineer ID
        string userInput = Microsoft.VisualBasic.Interaction.InputBox("Enter the ID of the engineer you wish to find:", "Engineer ID", "");

        // Check if the user clicked cancel or provided an empty input
        if (userInput == null || userInput == "")
        {
            // User canceled or provided empty input, do nothing
            return;
        }

        // Check if the user entered a valid ID
        if (int.TryParse(userInput, out int engineerID))
        {
            try
            {
                // Check if there is an engineer with this ID and if it does then go to the Engineer View
                BO.Engineer? desiredEngineer = s_bl.Engineer.GetEngineer(engineerID);
                int validID = desiredEngineer.ID;
                new EngineerUserWindow(validID).ShowDialog();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Invalid input. Please enter a valid engineer ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
        new Clock.SystemClockWindow().ShowDialog();
    }

    private void btnSetProjectDates_Click(object sender, RoutedEventArgs e)
    {
        new Clock.ProjectDatesWindow().ShowDialog();
    }

    private void activated(object sender, EventArgs e)
    {
        SystemClock = s_bl.Config.GetSystemClock();
        IsInProduction = s_bl.Config.InProduction();
    }
}