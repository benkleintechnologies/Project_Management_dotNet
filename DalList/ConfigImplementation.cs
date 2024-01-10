namespace Dal;
using DalApi;
using System;

/// <summary>
/// Implementation of Config Interface, which allows setting start and end dates of project, and resetting
/// </summary>
public class ConfigImplementation : IConfig
{
    public void reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Tasks.Clear();
        DataSource.Dependencies.Clear();
        DataSource.Config._startDate = null;
        DataSource.Config._endDate = null;
    }

    public void setEndDate(DateTime endDate)
    {
        DataSource.Config._endDate = endDate;
    }

    public void setStartDate(DateTime startDate)
    {
        DataSource.Config._startDate = startDate;
    }
    
}
