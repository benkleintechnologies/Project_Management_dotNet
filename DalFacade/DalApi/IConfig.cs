namespace DalApi;

/// <summary>
/// Interface to access the Config
/// </summary>
public interface IConfig
{
    void SetStartDate(DateTime startDate); // Set Start Date of the Project
    void SetEndDate(DateTime endDate); // Set End Date of the Project
    DateTime? GetStartDate(); // Get Start date of project
    DateTime? GetEndDate(); //Get end date of project
    void Reset(); // Reset all entity lists
}
