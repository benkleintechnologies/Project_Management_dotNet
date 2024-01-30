namespace Dal;
using DalApi;
using System;
using DO;

/// <summary>
/// Implementation of Config Interface, which allows setting start and end dates of project, and resetting
/// </summary>
public class ConfigImplementation : IConfig
{
    public DateTime? getEndDate()
    {
        return DataSource.Config._endDate;
    }

    public DateTime? getStartDate()
    {
        return DataSource.Config._startDate;
    }

    /// <summary>
    /// Reset all items in the database
    /// </summary>
    public void Reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Tasks.Clear();
        DataSource.Dependencies.Clear();
        DataSource.Config._startDate = null;
        DataSource.Config._endDate = null;
        DataSource.Config.resetDependencyId();
        DataSource.Config.resetTaskId();
    }

    /// <summary>
    /// Set the end date - *Only if the rest of the system has been reset or uninitialized*
    /// </summary>
    /// <param name="endDate"></param>
    public void setEndDate(DateTime endDate)
    {
        if (DataSource.Engineers.Count == 0 && DataSource.Tasks.Count == 0 && DataSource.Dependencies.Count == 0)
        {
            DataSource.Config._endDate = endDate;
        }
        else
        {
            throw new DateCannotBeChangedException("End date can only be changed if the rest of the system has been reset or uninitialized");
        }
    }

    /// <summary>
    /// Set the start date - *Only if the rest of the system has been reset or uninitialized*
    /// </summary>
    /// <param name="startDate"></param>
    public void setStartDate(DateTime startDate)
    {
        if (DataSource.Engineers.Count == 0 && DataSource.Tasks.Count == 0 && DataSource.Dependencies.Count == 0)
        {
            DataSource.Config._startDate = startDate;
        }
        else
        {
            throw new DateCannotBeChangedException("Start date can only be changed if the rest of the system has been reset or uninitialized");
        }
    }
    
}
