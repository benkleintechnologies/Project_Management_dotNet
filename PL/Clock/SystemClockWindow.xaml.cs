using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace PL.Clock;

/// <summary>
/// Interaction logic for SystemClockWindow.xaml
/// </summary>
public partial class SystemClockWindow : Window, INotifyPropertyChanged
{
    //The BL instance
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    //System clock value
    private DateTime _systemClock;
    //Chosen time span
    public double ChosenTimeAmount { get; set; }
    //Chosen time span unit
    public string ChosenTimeSpanUnit { get; set; }

    public SystemClockWindow()
    {
        SystemClock = s_bl.Config.GetSystemClock();
        InitializeComponent();
    }

    private void btnBackward_Click(object sender, RoutedEventArgs e)
    {
        CalculateSystemClock(-1);
        s_bl.Config.SetSystemClock(SystemClock);
    }

    private void btnForward_Click(object sender, RoutedEventArgs e)
    {
        CalculateSystemClock(1);
        s_bl.Config.SetSystemClock(SystemClock);
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CalculateSystemClock(int direction)
    {
        TimeSpan timeSpan = ChosenTimeSpanUnit switch
        {
            "Hours" => TimeSpan.FromHours(ChosenTimeAmount),
            "Days" => TimeSpan.FromDays(ChosenTimeAmount),
            "Weeks" => TimeSpan.FromDays(ChosenTimeAmount * 7),
            "Months" => TimeSpan.FromDays(ChosenTimeAmount * 30),
            "Years" => TimeSpan.FromDays(ChosenTimeAmount * 365),
            _ => throw new ArgumentException("Invalid time span unit"),
        };

        SystemClock += timeSpan * direction;
    }


}
