namespace BlApi;

/// <summary>
/// Interface for Business Layer methods of Config
/// </summary>
public interface IConfig
{
    public void SetProjectStartDate(DateTime startDate);
    public void SetProjectEndDate(DateTime endDate);
}