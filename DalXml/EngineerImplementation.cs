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

    public Engineer? Read(int id)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        return _engineers.FirstOrDefault(item => item.id == id && item.active);
    }

    public Engineer? Read(Func<Engineer, bool> filter)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        return _engineers.Where(item => item.active).FirstOrDefault(filter);
    }

    public IEnumerable<Engineer?> ReadAll(Func<Engineer, bool>? filter = null)
    {
        List<Engineer> _engineers = XMLTools.LoadListFromXMLSerializer<Engineer>(s_engineers_xml);
        if (filter == null)
        {
            return _engineers.Where(item => item.active);
        }
        return _engineers.Where(filter).Where(item => item.active);
    }

    public void Reset()
    {
        XMLTools.SaveListToXMLSerializer<Engineer>(new List<Engineer>(), s_engineers_xml);
    }

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
