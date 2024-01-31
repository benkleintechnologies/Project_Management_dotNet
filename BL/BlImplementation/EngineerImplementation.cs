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
    public void AddEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !isValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Try to add the Engineer to the data layer
            DO.Engineer newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, (DO.EngineerExperience)engineer.Experience, true);

            _dal.Engineer.Create(newEngineer);
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
    public void DeleteEngineer(int Id)
    {
        try
        {
            //Make sure the engineer is not assigned to a task
            if (_dal.Task.Read(t => t.AssignedEngineerId == Id) is null)
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
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    public BO.Engineer GetEngineer(int id)
    {
        try
        {
            //Get Engineer from DAL
            DO.Engineer engineer = _dal.Engineer.Read(id);
            //Make a BL type Engineer
            return toBlEngineer(engineer);
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
    public IEnumerable<BO.Engineer> GetListOfEngineers(Func<BO.Engineer, bool>? filter = null)
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
    public void UpdateEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !isValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Get Engineer from DAL
            DO.Engineer dlEngineer = _dal.Engineer.Read(engineer.Id);

            //Check the Experience level is the same or higher and only change it if it is
            DO.EngineerExperience engineerExperience = (DO.EngineerExperience)engineer.Experience >= dlEngineer.Level ? (DO.EngineerExperience)engineer.Experience : dlEngineer.Level;

            //Make update DL type Engineer
            DO.Engineer newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, engineerExperience, true);

            //Call update on DL
            _dal.Engineer.Update(newEngineer);

            //Update the Task the Engineer is assigned to.
            //Find the task the engineer is currently assigned to
            DO.Task assignedTask = _dal.Task.Read(t => t.AssignedEngineerId == engineer.Id);

            // Unassign the engineer from the old task if there was a change in assignment
            if (engineer.Task?.Id != assignedTask?.ID && assignedTask is not null)
            {
                _dal.Task.Update(assignedTask with { AssignedEngineerId = null });
            }

            // If the engineer is assigned to a new task, update the task with the engineer's ID
            if (engineer.Task is not null)
            {
                DO.Task toAssignTask = _dal.Task.Read(engineer.Task.Id);

                if (toAssignTask is not null)
                {
                    _dal.Task.Update(toAssignTask with { AssignedEngineerId = engineer.Id });
                }
            }
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    static private bool isValidEmail(string email)
    {
        // Define a regular expression pattern for a simple email validation
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        // Create a Regex object and match the email against the pattern
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);

        // Return true if there is a match, indicating a valid email
        return match.Success;
    }

    /// <summary>
    /// Converts an Engineer object from the DL to a BL Engineer with its assigned task
    /// </summary>
    /// <param name="engineer">The Data Layer Engineer object</param>
    /// <returns>The Business Layer Engineer Object</returns>
    private BO.Engineer toBlEngineer(DO.Engineer engineer)
    {
        //Get task assigned to engineer
        DO.Task taskAssigned = _dal.Task.Read(t => t.AssignedEngineerId == engineer.ID);
        //Create TaskInEngineer
        BO.TaskInEngineer taskInEngineer = new(taskAssigned.ID, taskAssigned.Nickname);
        //Make a BL type Engineer
        BO.Engineer blEngineer = new(engineer.ID, engineer.Name, engineer.Email, (BO.EngineerExperience)engineer.Level, engineer.Cost, taskInEngineer);

        return blEngineer;
    }

}
