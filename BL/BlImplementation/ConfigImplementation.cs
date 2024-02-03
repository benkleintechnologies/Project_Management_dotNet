namespace BlImplementation;
using BlApi;
using System;

internal class ConfigImplementation : IConfig
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public void SetProjectEndDate(DateTime endDate)
    {
        _dal.Config.SetEndDate(endDate);
    }

    public void SetProjectStartDate(DateTime startDate)
    {
        _dal.Config.SetStartDate(startDate);
    }

    public void Reset()
    {
        _dal.Config.Reset();
    }
}
