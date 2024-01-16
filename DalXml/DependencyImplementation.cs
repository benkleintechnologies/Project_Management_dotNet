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
        // Convert Dependency object to XElement
        XElement _dependencyElement = new XElement("Dependency",
            new XElement("id", _dependency.id),
            new XElement("dependentTask", _dependency.dependentTask),
            new XElement("dependsOnTask", _dependency.dependsOnTask),
            new XElement("customerEmail", _dependency.customerEmail),
            new XElement("shippingAddress", _dependency.shippingAddress),
            new XElement("orderCreationDate", _dependency.orderCreationDate),
            new XElement("shippingDate", _dependency.shippingDate),
            new XElement("deliveryDate", _dependency.deliveryDate),
            new XElement("active", _dependency.active)
        );
        _dependencies.Add(_dependencyElement);
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
        
        //Get list of dependencies
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Element("id")! == id)!;
        //Set it as deleted
        _dependencyElement.Element("active")?.SetValue(false);
            XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
    }

    /// <summary>
    /// Retrieve a Dependency from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Dependency</param>
    /// <returns>The Dependency object requested</returns>
    public Dependency? Read(int id)
    {
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        
        IEnumerable<XElement> _dependencyElements = _dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("active")!);

        Dependency? _result = _dependencyElements
            .Select(_dependencyElement => new Dependency(
                id: int.Parse(_dependencyElement.Element("id")!.Value),
                dependentTask: int.Parse(_dependencyElement.Element("dependentTask")!.Value),
                dependsOnTask: int.Parse(_dependencyElement.Element("dependsOnTask")!.Value),
                customerEmail: _dependencyElement.Element("customerEmail")?.Value,
                shippingAddress: _dependencyElement.Element("shippingAddress")?.Value,
                orderCreationDate: DateTime.TryParse(_dependencyElement.Element("orderCreationDate")!.Value, out var creationDate) ? creationDate : null,
                shippingDate: DateTime.TryParse(_dependencyElement.Element("shippingDate")!.Value, out var shipDate) ? shipDate : null,
                deliveryDate: DateTime.TryParse(_dependencyElement.Element("deliveryDate")!.Value, out var deliverDate) ? deliverDate : null,
                active: bool.Parse(_dependencyElement.Element("active")!.Value)
                ))
            .FirstOrDefault(d => d.id == id);

        if (_result is null)
            throw new DalDoesNotExistException($"Object of type Dependency with given filter does not exist");

        return _result;
    }

    /// <summary>
    /// Retrieve an Engineer from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    public Dependency? Read(Func<Dependency, bool> filter)
    {
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        
        IEnumerable<XElement> _dependencyElements = _dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("active")!);

        Dependency? _result = _dependencyElements
            .Select(_dependencyElement => new Dependency(
                id: int.Parse(_dependencyElement.Element("id")!.Value),
                dependentTask: int.Parse(_dependencyElement.Element("dependentTask")!.Value),
                dependsOnTask: int.Parse(_dependencyElement.Element("dependsOnTask")!.Value),
                customerEmail: _dependencyElement.Element("customerEmail")?.Value,
                shippingAddress: _dependencyElement.Element("shippingAddress")?.Value,
                orderCreationDate: DateTime.TryParse(_dependencyElement.Element("orderCreationDate")!.Value, out var creationDate) ? creationDate : null,
                shippingDate: DateTime.TryParse(_dependencyElement.Element("shippingDate")!.Value, out var shipDate) ? shipDate : null,
                deliveryDate: DateTime.TryParse(_dependencyElement.Element("deliveryDate")!.Value, out var deliverDate) ? deliverDate : null,
                active: bool.Parse(_dependencyElement.Element("active")!.Value)
                ))
            .FirstOrDefault(filter);

        if (_result is null)
            throw new DalDoesNotExistException($"Object of type Dependency with given filter does not exist");

        return _result;
    }

    /// <summary>
    /// Retrieve all Dependencies from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Dependencies</returns>
    public IEnumerable<Dependency?> ReadAll(Func<Dependency, bool>? filter = null)
    {
        //Get list of dependencies
        XElement _dependenciesElement = XMLTools.LoadListFromXMLElement(s_dependencies_xml);

        IEnumerable<Dependency> _dependencies = _dependenciesElement.Elements("Dependency").Select(elem => new Dependency(
            id: int.Parse(elem.Element("id")!.Value),
            dependentTask: int.Parse(elem.Element("dependentTask")!.Value),
            dependsOnTask: int.Parse(elem.Element("dependsOnTask")!.Value),
            customerEmail: elem.Element("customerEmail")?.Value,
            shippingAddress: elem.Element("shippingAddress")?.Value,
            orderCreationDate: DateTime.TryParse(elem.Element("orderCreationDate")!.Value, out var creationDate) ? creationDate : null,
            shippingDate: DateTime.TryParse(elem.Element("shippingDate")!.Value, out var shipDate) ? shipDate : null,
            deliveryDate: DateTime.TryParse(elem.Element("deliveryDate")!.Value, out var deliverDate) ? deliverDate : null,
            active: bool.Parse(elem.Element("active")!.Value)
            ));

        if (filter == null)
        {
            return _dependencies.Where(item => item.active);
        }

        return _dependencies.Where(filter).Where(item => item.active);
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
        //Get list of dependencies
        XElement _dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement _dependencyElement = _dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Element("id")! == item.id)!;

        //Update values of the Element
        if (_dependencyElement is not null)
        {
            _dependencyElement!.SetElementValue("dependentTask", item.dependentTask);
            _dependencyElement.SetElementValue("dependsOnTask", item.dependsOnTask);
            _dependencyElement.SetElementValue("customerEmail", item.customerEmail);
            _dependencyElement.SetElementValue("shippingAddress", item.shippingAddress);
            _dependencyElement.SetElementValue("orderCreationDate", item.orderCreationDate?.ToString("dd/MM/yyyy") ?? "");//Fix this line
            _dependencyElement.SetElementValue("shippingDate", item.shippingDate?.ToString("dd/MM/yyyy") ?? ""); //Fix this line
            _dependencyElement.SetElementValue("deliveryDate", item.deliveryDate?.ToString("dd/MM/yyyy") ?? ""); //Fix this line
            _dependencyElement.SetElementValue("active", item.active);

            XMLTools.SaveListToXMLElement(_dependencies, s_dependencies_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.id} does not exist");
        }
    }
}
