namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class DependencyImplementation : IDependency
{
    readonly string s_dependencies_xml = "dependencies";

    public int Create(Dependency item)
    {
        int _id = Config.NextDependencyId;
        Dependency _dependency = item with { id = _id };
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        _dependencies.Add(_dependency);
        XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
        return _id;
    }

    public void Delete(int id)
    {
        Dependency? _dependency = Read(id);
        if (_dependency == null)
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
        }
        else
        {
            //Get list of dependencies
            XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
            //Find the dependency with id
            XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Attribute("id")! == id)!;

            if (_dependencyElement == null)
            {
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
            }
            //Set it as deleted
            _dependencyElement.Element("active")?.SetValue(false);
            XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
        }
    }

    public Dependency? Read(int id)
    {
        //Get liost of dependencies
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Attribute("id")! == id)!;
        //Create Dependency object from the element

        //TODO: Parse the data
        //
        //
        //
        return null;
    }

    public Dependency? Read(Func<Dependency, bool> filter)
    {
        //TODO: Implement
        //
        //
        //
        //

        throw new NotImplementedException();
    }

    public IEnumerable<Dependency?> ReadAll(Func<Dependency, bool>? filter = null)
    {
        //TODO: Implement
        //
        //
        //
        //

        throw new NotImplementedException();
    }

    public void Reset()
    {
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        _dependencies.RemoveAll();
        XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
    }

    public void Update(Dependency item)
    {
        Dependency? _old = Read(item.id);
        if (_old != null)
        {
            //Get list of dependencies
            XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
            //Find the dependency with id
            XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Attribute("id")! == _old.id)!;

            if (_dependencyElement == null)
            {
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");
            }
            
            //update it
            //_dependencyElement.Element(
            //TODO: Implement
            //
            //
            //

            XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.id} does not exist");
        }
    }
}
