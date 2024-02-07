namespace BlApi;

/// <summary>
/// Interface for Business Layer methods of Config
/// </summary>
public interface IConfig
{
    void SetProjectStartDate(DateTime startDate);
    void SetProjectEndDate(DateTime endDate);
    public bool inProduction();
    void Reset();

}