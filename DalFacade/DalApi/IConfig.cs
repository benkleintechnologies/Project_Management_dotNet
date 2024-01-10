namespace DalApi;

/// <summary>
/// Interface to access the Config
/// </summary>
public interface IConfig
{
    void setStartDate(DateTime startDate); // Set Start Date of the Project
    void setEndDate(DateTime endDate); // Set End Date of the Project
    void reset(); // Reset all entity lists
}
