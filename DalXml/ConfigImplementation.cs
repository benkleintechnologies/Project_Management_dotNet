namespace Dal;
using DalApi;
using System;

/// <summary>
/// Implementation of Config Interface, which allows setting start and end dates of project, and resetting the XML
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Resets all items in the XML
    /// </summary>
    public void reset()
    {
        DalXml.Engineer.Reset();
        DalXml.Task.Reset();
        DalXml.Dependency.Reset();
        Config._startDate = null;
        Config._endDate = null;
    }

    public void setEndDate(DateTime endDate)
    {
        Config._endDate = endDate;
    }

    public void setStartDate(DateTime startDate)
    {
        Config._startDate = startDate;
    }
}
