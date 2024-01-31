namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

/// <summary>
/// Implementation of Engineer interface using XML
/// </summary>
internal class EngineerImplementation : IEngineer
{
    readonly string s_engineers_xml = "engineers";

    /// <summary>
    /// Add Engineer to XML File
    /// </summary>
    /// <param name="item">The Engineer to add</param>
    /// <returns>The ID of the Engineer</returns>
    /// <exception cref="DalAlreadyExistsException">Thrown if this Engineer doesn't exist on the database/exception>
    public int Create(Engineer item)
    {
        if (InternalRead(item.ID) is not null)
        {
            throw new DalAlreadyExistsException($"An object of type Engineer with ID {item.ID} already exists");
        }
        List<Engineer> engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        engineers.Add(item);
        XMLTools.SaveListToXMLSerializer<Engineer>(engineers, s_engineers_xml);
        return item.ID;
    }

    /// <summary>
    /// Delete an Engineer from the XML File
    /// </summary>
    /// <param name="id">ID of the Engineer to delete</param>
    public void Delete(int id)
    {
        Engineer engineer = Read(id); //Check that this item exists
        List<Engineer> engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        engineers.Remove(engineer);
        Engineer newEngineer = engineer with { Active = false };
        engineers.Add(newEngineer);
        XMLTools.SaveListToXMLSerializer<Engineer>(engineers, s_engineers_xml);
    }

    /// <summary>
    /// Retrieve a Engineer from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Engineer with this ID exists</exception>
    public Engineer Read(int id)
    {
        Engineer? engineer = InternalRead(id);
        if (engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist");
        }
        return engineer;
    }

    /// <summary>
    /// Retrieve an Engineer from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Engineer with this ID and filter exists</exception>
    /// 
    public Engineer Read(Func<Engineer, bool> filter)
    {
        List<Engineer> engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        Engineer? engineer = engineers.Where(item => item.Active).FirstOrDefault(filter);
        if (engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with this filter does not exist");
        }
        return engineer;
    }

    /// <summary>
    /// Retrieve all Engineer from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Engineer</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Engineers in the list</exception>
    public IEnumerable<Engineer> ReadAll(Func<Engineer, bool>? filter = null)
    {
        List<Engineer> engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        IEnumerable<Engineer> activeEngineers = engineers.Where(item => item.Active);
        if (activeEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        if (filter == null)
        {
            return activeEngineers;
        }

        IEnumerable<Engineer> filteredEngineers = activeEngineers.Where(filter);
        if (filteredEngineers.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Engineer exists");
        }
        return filteredEngineers;
    }

    /// <summary>
    /// Removes all Engineers from the XML File
    /// </summary>
    public void Reset()
    {
        XMLTools.SaveListToXMLSerializer<Engineer>(new List<Engineer>(), s_engineers_xml);
    }

    /// <summary>
    /// Updates an Engineer in the XML File
    /// </summary>
    /// <param name="item">New Engineer information</param>
    public void Update(Engineer item)
    {
        Engineer? _old = Read(item.ID);
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        _engineers.Remove(_old);
        _engineers.Add(item);
        XMLTools.SaveListToXMLSerializer<Engineer>(_engineers, s_engineers_xml);
    }
    private Engineer? InternalRead(int id)
    {
       List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
       return _engineers.FirstOrDefault(item => item.ID == id && item.Active);
    }

}
