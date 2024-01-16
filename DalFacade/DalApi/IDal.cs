﻿namespace DalApi;

/// <summary>
/// IDal interface that allows readonly access to each of the subinterfaces
/// </summary>
public interface IDal
{
    IEngineer Engineer { get; }
    ITask Task { get; }
    IDependency Dependency { get; }
    
}
