﻿namespace BlImplementation;
using BlApi;
using System;

internal class ConfigImplementation : IConfig
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public void SetProjectEndDate(DateTime endDate)
    {
        if (!_dal.Config.GetEndDate().HasValue) //Cannot set the date if it's already set
        {
            _dal.Config.SetEndDate(endDate);
        }
        else
        {
            throw new BO.BlCannotChangeDateException("The Project end date could not be changed because it was already set.");
        }
    }

    public void SetProjectStartDate(DateTime startDate)
    {
        if (!_dal.Config.GetStartDate().HasValue) //Cannot set the date if it's already set
        {
            _dal.Config.SetStartDate(startDate);
        }
        else
        {
            throw new BO.BlCannotChangeDateException("The Project start date could not be changed because it was already set.");
        }
    }

    public void Reset()
    {
        _dal.Config.Reset();
    }
}