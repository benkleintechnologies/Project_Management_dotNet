using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Engineer;

/// <summary>
/// Interaction logic for EngineerListWindow.xaml
/// </summary>
public partial class EngineerListWindow : Window
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    //Constructor for the window
    public EngineerListWindow()
    {
        InitializeComponent();
    }

    //The event handler for the window activation
    private void activated(object sender, EventArgs e)
    {
        try
        {
            EngineerList = s_bl?.Engineer.GetListOfEngineers()!;
        }
        catch (BO.BlDoesNotExistException)
        {
            EngineerList = Enumerable.Empty<BO.Engineer>();
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        
    }

    //Getters and setters for the list of engineers
    public IEnumerable<BO.Engineer> EngineerList
    {
        get { return (IEnumerable<BO.Engineer>)GetValue(EngineerListProperty); }
        set { SetValue(EngineerListProperty, value); }
    }

    //Dependency Property to connect the list of engineers to the window
    public static readonly DependencyProperty EngineerListProperty =
        DependencyProperty.Register("EngineerList", typeof(IEnumerable<BO.Engineer>), typeof(EngineerListWindow), new PropertyMetadata(null));

    //The selected engineer experience level
    public BO.EngineerExperience ExperienceLevel { get; set; } = BO.EngineerExperience.All;

    //The event handler for the selection of the experience level (to filter the list of engineers)
    private void cbExperienceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            EngineerList = (ExperienceLevel == BO.EngineerExperience.All) ?
                s_bl?.Engineer.GetListOfEngineers()! : s_bl?.Engineer.GetListOfEngineers(item => item.Experience == ExperienceLevel)!;
        }
        catch (BO.BlDoesNotExistException)
        {
            EngineerList = Enumerable.Empty<BO.Engineer>();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    //The event handler for the add button
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new EngineerWindow().ShowDialog();
    }

    //The event handler for the double click on an item in the list view to open the update dialog
    private void ListView_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        // Extract the selected item from the ListView
        BO.Engineer? engineer = (sender as ListView)?.SelectedItem as BO.Engineer;

        if (engineer != null)
        {
            new EngineerWindow(engineer.ID).ShowDialog();
        }

    }
    
}