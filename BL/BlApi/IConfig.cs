namespace BlApi;

/// <summary>
/// Interface for Business Layer methods of Config
/// </summary>
public interface IConfig
{
    void SetProjectStartDate(DateTime startDate);
    void SetProjectEndDate(DateTime endDate);
    DateTime? GetProjectStartDate();
    DateTime? GetProjectEndDate();
    void SetSystemClock(DateTime systemClock); // Set the system clock
    DateTime GetSystemClock(); // Get the system clock
    public bool InProduction();
    void Reset();

}