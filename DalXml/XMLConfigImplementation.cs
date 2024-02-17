namespace Dal;
using DalApi;
using DO;
using System;

/// <summary>
/// Config Implementation for the XML files Config data
/// </summary>
public class XMLConfigImplementation : IConfig
{
    public DateTime? GetEndDate()
    {
        return Config.EndDate;
    }

    public DateTime? GetStartDate()
    {
        return Config.StartDate;
    }

    public void SetSystemClock(DateTime systemClock)
    {
        Config.SystemClock = systemClock;
    }

    public DateTime GetSystemClock()
    {
        return Config.SystemClock ?? DateTime.Now;
    }

    public void Reset()
    {
        Config.Reset();
    }

    /// <summary>
    /// Setter for the project end date
    /// </summary>
    /// <param name="endDate"></param>
    /// <exception cref="DateCannotBeChangedException">Thrown if trying to set the end date while there is still data in the XML files</exception>
    public void SetEndDate(DateTime endDate)
    {
        if (!GetEndDate().HasValue)
        {
            Config.EndDate = endDate;
        }
        else
        {
            throw new DateCannotBeChangedException("End date can only be changed if the rest of the system has been reset or uninitialized");
        }
    }

    /// <summary>
    /// Setter for the project start date
    /// </summary>
    /// <param name="startDate"></param>
    /// <exception cref="DateCannotBeChangedException">Thrown if trying to set the start date while there is still data in the XML files</exception>
    public void SetStartDate(DateTime startDate)
    {
        if (!GetStartDate().HasValue)
        {
            Config.StartDate = startDate;
        }
        else
        {
            throw new DateCannotBeChangedException("Start date can only be changed if the rest of the system has been reset or uninitialized");
        }
    }
}
