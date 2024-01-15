namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// Implementation of Dependency Interface to access Dependencies from XML
/// </summary>
internal class DependencyImplementation : IDependency
{
    readonly string s_dependencies_xml = "dependencies"; //Name of XML file to access

    /// <summary>
    /// Add Dependency to XML File
    /// </summary>
    /// <param name="item">The Dependency to add</param>
    /// <returns>The ID of the Dependency</returns>
    public int Create(Dependency item)
    {
        int _id = Config.NextDependencyId;
        Dependency _dependency = item with { id = _id };
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        _dependencies.Add(_dependency);
        XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
        return _id;
    }

    /// <summary>
    /// Delete a Dependency from the XML File
    /// </summary>
    /// <param name="id">ID of the Dependency to delete</param>
    /// <exception cref="DalDoesNotExistException">Thrown if there is no Dependency with this ID in the database</exception>
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

    /// <summary>
    /// Retreive a Dependency from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Dependency</param>
    /// <returns>The Dependency object requested</returns>
    public Dependency? Read(int id)
    {
        //Get list of dependencies
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Attribute("id")! == id && (bool)e.Element("active")!)!;
        
        //Create Dependency object from the element
        if (_dependencyElement is not null)
        {
            Dependency _dependency = new Dependency(
                id: (int)_dependencyElement.Attribute("id")!,
                dependentTask: (int)_dependencyElement.Element("dependentTask")!,
                dependsOnTask: (int)_dependencyElement.Element("dependsOnTask")!,
                customerEmail: (string?)_dependencyElement.Element("customerEmail"),
                shippingAddress: (string?)_dependencyElement.Element("shippingAddress"),
                orderCreationDate: (DateTime?)_dependencyElement.Element("orderCreationDate"),
                shippingDate: (DateTime?)_dependencyElement.Element("shippingDate"),
                deliveryDate: (DateTime?)_dependencyElement.Element("deliveryDate"),
                active: (bool)_dependencyElement.Element("active")!
            );
            return _dependency;
        }

        return null;
    }

    /// <summary>
    /// Retreive an Engineer from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Dependency? Read(Func<Dependency, bool> filter)
    {
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        
        IEnumerable<XElement> _dependencyElements = _dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("active")!);

        Dependency? result = _dependencyElements
            .Select(_dependencyElement => new Dependency(
                id: (int)_dependencyElement.Attribute("id")!,
                dependentTask: (int)_dependencyElement.Element("dependentTask")!,
                dependsOnTask: (int)_dependencyElement.Element("dependsOnTask")!,
                customerEmail: (string?)_dependencyElement.Element("customerEmail"),
                shippingAddress: (string?)_dependencyElement.Element("shippingAddress"),
                orderCreationDate: (DateTime?)_dependencyElement.Element("orderCreationDate"),
                shippingDate: (DateTime?)_dependencyElement.Element("shippingDate"),
                deliveryDate: (DateTime?)_dependencyElement.Element("deliveryDate"),
                active: (bool)_dependencyElement.Element("active")!
            ))
            .FirstOrDefault(filter);

        return result;
    }

    /// <summary>
    /// Retreive all Dependencies from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Dependencies</returns>
    public IEnumerable<Dependency?> ReadAll(Func<Dependency, bool>? filter = null)
    {
        //Get list of dependencies
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);

        IEnumerable<XElement> _dependencyElements = _dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("active")!);

        IEnumerable<Dependency?> _result = _dependencyElements
            .Select(_dependencyElement => new Dependency(
                id: (int)_dependencyElement.Attribute("id")!,
                dependentTask: (int)_dependencyElement.Element("dependentTask")!,
                dependsOnTask: (int)_dependencyElement.Element("dependsOnTask")!,
                customerEmail: (string?)_dependencyElement.Element("customerEmail"),
                shippingAddress: (string?)_dependencyElement.Element("shippingAddress"),
                orderCreationDate: (DateTime?)_dependencyElement.Element("orderCreationDate"),
                shippingDate: (DateTime?)_dependencyElement.Element("shippingDate"),
                deliveryDate: (DateTime?)_dependencyElement.Element("deliveryDate"),
                active: (bool)_dependencyElement.Element("active")!
            ))
            .Where(dependency => filter == null || filter(dependency));

        return _result;
    }

    /// <summary>
    /// Removes all Dependencies from the XML File
    /// </summary>
    public void Reset()
    {
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        _dependencies.RemoveAll();
        XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
    }

    /// <summary>
    /// Updates a Dependency in the XML File
    /// </summary>
    /// <param name="item">New Dependency information</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no Dependency with the same ID exists</exception>
    public void Update(Dependency item)
    {
        Dependency? _old = Read(item.id);
        if (_old != null)
        {
            //Get list of dependencies
            XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
            //Find the dependency with id
            XElement _dependencyElement = _dependencies.Elements("Dependency")
                .FirstOrDefault(e => (int)e.Attribute("id")! == item.id)!;
            //Update values of the Element
            if (_dependencyElement == null)
            {
                _dependencyElement!.SetElementValue("dependentTask", item.dependentTask);
                _dependencyElement.SetElementValue("dependsOnTask", item.dependsOnTask);
                _dependencyElement.SetElementValue("customerEmail", item.customerEmail);
                _dependencyElement.SetElementValue("shippingAddress", item.shippingAddress);
                _dependencyElement.SetElementValue("orderCreationDate", item.orderCreationDate);
                _dependencyElement.SetElementValue("shippingDate", item.shippingDate);
                _dependencyElement.SetElementValue("deliveryDate", item.deliveryDate);
                _dependencyElement.SetElementValue("active", item.active);

                XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
            }
            else
            {
                throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.id} does not exist");
            }
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.id} does not exist");
        }
    }
}
