namespace Dal;
using DalApi;
using DO;
using System;

/// <summary>
/// Config Implementation for the XML files Config data
/// </summary>
public class XMLConfigImplementation : IConfig
{
    public void reset()
    {
        Config.Reset();
    }

    /// <summary>
    /// Setter for the project end date
    /// </summary>
    /// <param name="endDate"></param>
    /// <exception cref="DateCannotBeChangedException">Thrown if trying to set the end date while there is still data in the XML files</exception>
    public void setEndDate(DateTime endDate)
    {
        if (xmlFilesReset())
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
    public void setStartDate(DateTime startDate)
    {
        if (xmlFilesReset())
        {
            Config.StartDate = startDate;
        }
        else
        {
            throw new DateCannotBeChangedException("Start date can only be changed if the rest of the system has been reset or uninitialized");
        }
    }

    /// <summary>
    /// Check if the XML data files are all empty, meaning they are uninitialized or were reset
    /// </summary>
    /// <returns>True if all files are empty</returns>
    private bool xmlFilesReset()
    {
        List<Engineer> engineerList = XMLTools.LoadListFromXMLSerializer<Engineer>("engineers");
        List<Task> taskList = XMLTools.LoadListFromXMLSerializer<Task>("tasks");
        List<Dependency> dependencyList = XMLTools.LoadListFromXMLSerializer<Dependency>("dependencies");

        if (engineerList.Count == 0 && taskList.Count == 0 && dependencyList.Count == 0)
            return true;

        return false;
    }
}
