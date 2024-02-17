namespace Dal;
using DalApi;
using System;
using DO;

/// <summary>
/// Implementation of Config Interface, which allows setting start and end dates of project, and resetting
/// </summary>
public class ConfigImplementation : IConfig
{
    public DateTime? GetEndDate()
    {
        return DataSource.Config.endDate;
    }

    public DateTime? GetStartDate()
    {
        return DataSource.Config.startDate;
    }

    public DateTime GetSystemClock()
    {
        return DataSource.Config.systemClock;
    }
    public void SetSystemClock(DateTime systemClock)
    {
        DataSource.Config.systemClock = systemClock;
    }

    /// <summary>
    /// Reset all items in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Tasks.Clear();
        DataSource.Dependencies.Clear();
        DataSource.Config.startDate = null;
        DataSource.Config.endDate = null;
        DataSource.Config.resetDependencyId();
        DataSource.Config.resetTaskId();
    }

    /// <summary>
    /// Set the end date - *Only if the rest of the system has been reset or uninitialized*
    /// </summary>
    /// <param name="endDate"></param>
    public void SetEndDate(DateTime endDate)
    {
        DataSource.Config.endDate = endDate;
    }

    /// <summary>
    /// Set the start date - *Only if the rest of the system has been reset or uninitialized*
    /// </summary>
    /// <param name="startDate"></param>
    public void SetStartDate(DateTime startDate)
    {
        DataSource.Config.startDate = startDate;
    }
}
