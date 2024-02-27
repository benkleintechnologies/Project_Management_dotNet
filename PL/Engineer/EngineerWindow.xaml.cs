using System.Windows;
using System.Windows.Controls;

namespace PL.Engineer;

/// <summary>
/// Interaction logic for EngineerWindow.xaml
/// </summary>
public partial class EngineerWindow : Window
{

    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public EngineerWindow(int id = 0)
    {
        if (id != 0)
        {
            try
            {
                CurrentEngineer = s_bl?.Engineer.GetEngineer(id)!;
            }
            catch (BO.BlDoesNotExistException ex)
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
            CurrentEngineer = new BO.Engineer(0, "", "", BO.EngineerExperience.Beginner, 0, null);
        }
        InitializeComponent();
    }

    public BO.Engineer CurrentEngineer
    {
        get { return (BO.Engineer)GetValue(MyEngineerProperty); }
        set { SetValue(MyEngineerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MyEngineerProperty.
    public static readonly DependencyProperty MyEngineerProperty =
        DependencyProperty.Register("CurrentEngineer", typeof(BO.Engineer), typeof(EngineerWindow), new PropertyMetadata(null));

    //Button action for adding or updating an engineer
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
                    s_bl?.Engineer.AddEngineer(CurrentEngineer);
                    // Check that it was added and then close the window
                    try
                    {
                        s_bl?.Engineer.GetEngineer(CurrentEngineer.ID);
                        MessageBox.Show("The Engineer was added to the database.");
                        this.Close();
                    }
                    catch (BO.BlDoesNotExistException)
                    {
                        MessageBox.Show("The Engineer was not added to the database.");
                    }
                }
                else if (buttonText == "Update")
                {
                    // Call the method for updating the item
                    s_bl?.Engineer.UpdateEngineer(CurrentEngineer);
                    // Check that it was updated and then close the window
                    try
                    {
                        //Check that the engineer was updated
                        BO.Engineer engineer = s_bl?.Engineer.GetEngineer(CurrentEngineer.ID)!;
                        if (engineer.Name != CurrentEngineer.Name || engineer.Email != CurrentEngineer.Email || engineer.Cost != CurrentEngineer.Cost || engineer.Experience != CurrentEngineer.Experience)
                        {
                            MessageBox.Show("The Engineer was not updated in the database.");
                        }
                        else
                        {
                            MessageBox.Show("The Engineer was updated in the database.");
                        }
                        this.Close();
                    }
                    catch (BO.BlDoesNotExistException)
                    {
                        MessageBox.Show("The Engineer was not updated in the database.");
                    }
                }
            }
            catch (BO.BlAlreadyExistsException)
            {
                MessageBox.Show("The Engineer could not be added because there is already another Engineer with this ID.");
            }
            catch (BO.BlDoesNotExistException)
            {
                MessageBox.Show("The Engineer could not be updated because there is no Engineer with this ID.");
            }
            catch (BO.BlInvalidInputException)
            {
                MessageBox.Show("One of the fields you entered was not valid.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Error: Button not found");
        }
    }
}
