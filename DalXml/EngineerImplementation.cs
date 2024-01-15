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
    /// <exception cref="DalAlreadyExistsException">This Engineer doesn't exist on the database/exception>
    public int Create(Engineer item)
    {
        if (Read(item.id) is not null)
        {
            throw new DalAlreadyExistsException($"An object of type Engineer with id {item.id} already exists");
        }
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        _engineers.Add(item);
        XMLTools.SaveListToXMLSerializer<Engineer>(_engineers, s_engineers_xml);
        return item.id;
    }

    /// <summary>
    /// Delete an Engineer from the XML File
    /// </summary>
    /// <param name="id">ID of the Engineer to delete</param>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Engineer with this ID in the database</exception>
    public void Delete(int id)
    {
        Engineer? _engineer = Read(id);
        if (_engineer == null)
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {id} does not exist, so it cannot be deleted.");
        }
        else
        {
            List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
            _engineers.Remove(_engineer);
            Engineer _newEngineer = _engineer with { active = false };
            _engineers.Add(_newEngineer);
            XMLTools.SaveListToXMLSerializer<Engineer>(_engineers, s_engineers_xml);
        }
    }

    /// <summary>
    /// Retreive a Engineer from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Engineer? Read(int id)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        return _engineers.FirstOrDefault(item => item.id == id && item.active);
    }

    /// <summary>
    /// Retreive an Engineer from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Engineer? Read(Func<Engineer, bool> filter)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        return _engineers.Where(item => item.active).FirstOrDefault(filter);
    }

    /// <summary>
    /// Retreive all Engineer from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Engineer</returns>
    public IEnumerable<Engineer?> ReadAll(Func<Engineer, bool>? filter = null)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        if (filter == null)
        {
            return _engineers.Where(item => item.active);
        }
        return _engineers.Where(filter).Where(item => item.active);
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
    /// <exception cref="DalDoesNotExistException">Thrown if no Engineer with the same ID exists</exception>
    public void Update(Engineer item)
    {
        Engineer? _old = Read(item.id);
        if (_old != null)
        {
            List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
            _engineers.Remove(_old);
            _engineers.Add(item);
            XMLTools.SaveListToXMLSerializer<Engineer>(_engineers, s_engineers_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Engineer with identifier {item.id} does not exist");
        }
    }
}
