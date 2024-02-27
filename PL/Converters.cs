using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL;

class ConvertIdToContent : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value == 0 ? "Add" : "Update";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertIdToMode : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value == 0 ? true : false; //true for Add mode, false for Update mode
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


class ConvertProjectDateToBool : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (DateTime?)value != null ? false : true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

class ConvertBoolToContentKey : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Add" : "Select This Task";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ForegroundConvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value as string;
        if (str == null) return Brushes.Black;

        BO.Status status;
        if (!Enum.TryParse(str, out status)) return Brushes.Black;

        if (status == BO.Status.Scheduled) return Brushes.Blue;
        else if (status == BO.Status.OnTrack) return Brushes.Yellow;
        else if (status == BO.Status.InJeopardy) return Brushes.Red;
        else if (status == BO.Status.Done) return Brushes.Green;
        else return Brushes.Black; //unscheduled - shouldn't happen
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public class ValueColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value as string;
        if (str == null) return Brushes.Black;

        BO.Status status;
        if (!Enum.TryParse(str, out status)) return null;

        if (status == BO.Status.Scheduled) return Brushes.Blue;
        else if (status == BO.Status.OnTrack) return Brushes.Yellow;
        else if (status == BO.Status.InJeopardy) return Brushes.Red;
        else if (status == BO.Status.Done) return Brushes.Green;
        else return Brushes.Black; //unscheduled - shouldn't happen
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ConvertTaskToBool : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var task = value as BO.Task;
        return task.ID != 0 && task.ProjectedStartDate is null ? true : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}