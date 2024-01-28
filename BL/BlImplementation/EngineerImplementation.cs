namespace BlImplementation;
using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    
    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="engineer"></param>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlAlreadyExistsException"></exception>
    public void addEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !IsValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Try to add the Engineer to the data layer
            DO.Engineer _newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, (DO.EngineerExperience)engineer.Experience, true);

            _dal.Engineer.Create(_newEngineer);
        } 
        catch(DO.DalAlreadyExistsException exc) 
        {
            throw new BO.BlAlreadyExistsException(exc.Message);
        }   
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="Id"></param>
    /// <exception cref="BO.BlCannotBeDeletedException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void deleteEngineer(int Id)
    {
        try
        {
            //Make sure the engineer is not assigned to a task
            if (_dal.Task.Read(t => t.assignedEngineerId == Id) is null)
            {
                throw new BO.BlCannotBeDeletedException($"This engineer with ID {Id} could not be deleted because they are assigned to a Task.");
            }
            //try to delete the engineer from the DAL
            _dal.Engineer.Delete(Id);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Engineer getEngineer(int Id)
    {
        try
        {
            //Get Engineer from DAL
            DO.Engineer _engineer = _dal.Engineer.Read(Id);
            //Make a BL type Engineer
            return toBlEngineer(_engineer);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses Linq, let, and sort
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public IEnumerable<BO.Engineer> getListOfEngineers(Func<BO.Engineer, bool>? filter = null)
    {
        try
        {
            // Convert Dl engineers to BL engineers and filter it before putting it into a list
            IEnumerable<BO.Engineer> result = from engineer in _dal.Engineer.ReadAll()
                         let blEngineer = toBlEngineer(engineer)
                         where filter == null || filter(blEngineer)
                         select blEngineer;

            return result;
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    /// <summary>
    /// -- Uses nothing
    /// </summary>
    /// <param name="engineer"></param>
    /// <exception cref="BO.BlInvalidInputException"></exception>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public void updateEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !IsValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Get Engineer from DAL
            DO.Engineer _dlEngineer = _dal.Engineer.Read(engineer.Id);

            //Check the Experience level is the same or higher and only change it if it is
            DO.EngineerExperience _engineerExperience = (DO.EngineerExperience)engineer.Experience >= _dlEngineer.level ? (DO.EngineerExperience)engineer.Experience : _dlEngineer.level;

            //Make update DL type Engineer
            DO.Engineer _newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, _engineerExperience, true);

            //Call update on DL
            _dal.Engineer.Update(_newEngineer);

            //Update the Task the Engineer is assigned to.
            //Find the task the engineer is currently assigned to
            DO.Task _assignedTask = _dal.Task.Read(t => t.assignedEngineerId == engineer.Id);

            // Unassign the engineer from the old task if there was a change in assignment
            if (engineer.Task?.Id != _assignedTask?.id && _assignedTask is not null)
            {
                _dal.Task.Update(_assignedTask with { assignedEngineerId = null });
            }

            // If the engineer is assigned to a new task, update the task with the engineer's ID
            if (engineer.Task is not null)
            {
                DO.Task _toAssignTask = _dal.Task.Read(engineer.Task.Id);

                if (_toAssignTask is not null)
                {
                    _dal.Task.Update(_toAssignTask with { assignedEngineerId = engineer.Id });
                }
            }
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    static bool IsValidEmail(string email)
    {
        // Define a regular expression pattern for a simple email validation
        string _pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        // Create a Regex object and match the email against the pattern
        Regex _regex = new Regex(_pattern);
        Match _match = _regex.Match(email);

        // Return true if there is a match, indicating a valid email
        return _match.Success;
    }

    /// <summary>
    /// Converts an Engineer object from the DL to a BL Engineer with its assigned task
    /// </summary>
    /// <param name="engineer">The Data Layer Engineer object</param>
    /// <returns>The Business Layer Engineer Object</returns>
    private BO.Engineer toBlEngineer(DO.Engineer engineer)
    {
        //Get task assigned to engineer
        DO.Task _taskAssigned = _dal.Task.Read(t => t.assignedEngineerId == engineer.id);
        //Create TaskInEngineer
        BO.TaskInEngineer _taskInEngineer = new(_taskAssigned.id, _taskAssigned.nickname);
        //Make a BL type Engineer
        BO.Engineer _blEngineer = new(engineer.id, engineer.name, engineer.email, (BO.EngineerExperience)engineer.level, engineer.cost, _taskInEngineer);

        return _blEngineer;
    }

}
