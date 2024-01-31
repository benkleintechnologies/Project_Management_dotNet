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
        int id = Config.NextDependencyId;
        //Create Dependency with auto-incrementing ID
        Dependency dependency = item with { ID = id };
        //Load XML data of Dependencies
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        // Convert Dependency object to XElement
        XElement dependencyElement = new XElement("Dependency",
            new XElement("ID", dependency.ID),
            new XElement("DependentTask", dependency.DependentTask),
            new XElement("DependsOnTask", dependency.DependsOnTask),
            new XElement("Active", dependency.Active)
        );
        //Add Dependency to the data loaded from XML
        dependencies.Add(dependencyElement);
        //Save back to the XML
        XMLTools.SaveListToXMLElement(dependencies, s_dependencies_xml);
        return id;
    }

    /// <summary>
    /// Delete a Dependency from the XML File
    /// </summary>
    /// <param name="id">ID of the Dependency to delete</param>
    public void Delete(int id)
    {
        Dependency? dependency = Read(id);
        
        //Get list of dependencies
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement dependencyElement = dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Element("ID")! == id)!;
        //Set it as deleted
        dependencyElement.Element("Active")?.SetValue(false);
            XMLTools.SaveListToXMLElement(dependencies, s_dependencies_xml);
    }

    /// <summary>
    /// Retrieve a Dependency from the XML File by ID
    /// </summary>
    /// <param name="id">ID of the Dependency</param>
    /// <returns>The Dependency object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Dependency with this ID and filter exists</exception>
    public Dependency Read(int id)
    {
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Get enumerable of all active dependencies in XML
        IEnumerable<XElement> dependencyElements = dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("Active")!);

        //Find the Dependency (after converting each Element to Dependency) with the correct ID if it exists
        Dependency? result = dependencyElements
            .Select(dependencyElement => new Dependency(
                ID: int.Parse(dependencyElement.Element("ID")!.Value),
                DependentTask: int.Parse(dependencyElement.Element("DependentTask")!.Value),
                DependsOnTask: int.Parse(dependencyElement.Element("DependsOnTask")!.Value),
                Active: bool.Parse(dependencyElement.Element("Active")!.Value)
                ))
            .FirstOrDefault(d => d.ID == id);

        if (result is null)
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {id} does not exist");

        return result;
    }

    /// <summary>
    /// Retrieve an Engineer from the XML File based on a filter
    /// </summary>
    /// <param name="filter">The criteria of the requested Engineer</param>
    /// <returns>The Engineer object requested</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Dependency with this ID exists</exception>
    public Dependency Read(Func<Dependency, bool> filter)
    {
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        
        IEnumerable<XElement> dependencyElements = dependencies.Elements("Dependency")
        .Where(e => (bool)e.Element("Active")!);

        Dependency? result = dependencyElements
            .Select(dependencyElement => new Dependency(
                ID: int.Parse(dependencyElement.Element("ID")!.Value),
                DependentTask: int.Parse(dependencyElement.Element("DependentTask")!.Value),
                DependsOnTask: int.Parse(dependencyElement.Element("DependsOnTask")!.Value),
                Active: bool.Parse(dependencyElement.Element("Active")!.Value)
                ))
            .FirstOrDefault(filter);

        if (result is null)
            throw new DalDoesNotExistException($"Object of type Dependency with given filter does not exist");

        return result;
    }

    /// <summary>
    /// Retrieve all Dependencies from the XML File
    /// </summary>
    /// <param name="filter">Optional filter to limit list</param>
    /// <returns>Requested Enumerable of Dependencies</returns>
    /// <exception cref="DalDoesNotExistException">Thrown if no Dependencies exist in the list</exception>
    public IEnumerable<Dependency> ReadAll(Func<Dependency, bool>? filter = null)
    {
        //Get list of dependencies
        XElement dependenciesElement = XMLTools.LoadListFromXMLElement(s_dependencies_xml);

        IEnumerable<Dependency> dependencies = dependenciesElement.Elements("Dependency").Select(elem => new Dependency(
            ID: int.Parse(elem.Element("ID")!.Value),
            DependentTask: int.Parse(elem.Element("DependentTask")!.Value),
            DependsOnTask: int.Parse(elem.Element("DependsOnTask")!.Value),
            Active: bool.Parse(elem.Element("Active")!.Value)
            ));
        IEnumerable<Dependency> activeDependencies = dependencies.Where(item => item.Active);
        if (activeDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }

        if (filter == null)
        {
            return activeDependencies;
        }
        IEnumerable<Dependency> filteredDependencies = activeDependencies.Where(filter);
        if (filteredDependencies.Count() == 0)
        {
            throw new DalDoesNotExistException($"No Object of type Dependency exists");
        }
        return filteredDependencies;
    }

    /// <summary>
    /// Removes all Dependencies from the XML File
    /// </summary>
    public void Reset()
    {
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        dependencies.RemoveAll();
        XMLTools.SaveListToXMLElement(dependencies, s_dependencies_xml);
    }

    /// <summary>
    /// Updates a Dependency in the XML File
    /// </summary>
    /// <param name="item">New Dependency information</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no Dependency with the same ID exists</exception>
    public void Update(Dependency item)
    {
        //Get list of dependencies
        XElement dependencies = XMLTools.LoadListFromXMLElement(s_dependencies_xml);
        //Find the dependency with id
        XElement dependencyElement = dependencies.Elements("Dependency").FirstOrDefault(e => (int)e.Element("ID")! == item.ID)!;

        //Update values of the Element
        if (dependencyElement is not null)
        {
            dependencyElement!.SetElementValue("DependentTask", item.DependentTask);
            dependencyElement.SetElementValue("DependsOnTask", item.DependsOnTask);
            dependencyElement.SetElementValue("Active", item.Active);

            XMLTools.SaveListToXMLElement(dependencies, s_dependencies_xml);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Dependency with identifier {item.ID} does not exist");
        }
    }
}
