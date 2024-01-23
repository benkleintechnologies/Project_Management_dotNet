namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public void addEngineer(Engineer engineer)
    {
        throw new NotImplementedException();
    }

    public void deleteEngineer(int Id)
    {
        throw new NotImplementedException();
    }

    public Engineer getEngineer(int Id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Engineer> getListOfEngineers(Func<Engineer, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void updateEngineer(Engineer engineer)
    {
        throw new NotImplementedException();
    }
}
