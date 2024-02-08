namespace BlImplementation;
using BlApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    
    public void AddEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.ID <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !isValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.ID} was invalid");
            }

            //If we're in production
            if (_dal.Config.GetStartDate().HasValue)
            {
                //Update assigned task in DL (add assignedEngineerId to the task this engineer is assigned to)
                if (engineer.Task is not null)
                {
                    DO.Task toAssignTask;
                    try
                    {
                        toAssignTask = _dal.Task.Read(engineer.Task.ID);
                    }
                    catch (DalDoesNotExistException)
                    {
                        throw new BO.BlDoesNotExistException($"The Task with ID {engineer.Task.ID} which you are trying to assign to Engineer with ID {engineer.ID} does not exist.");
                    }

                    if (toAssignTask is not null)
                    {
                        //Check that the engineer has the same or higher experience level than the task
                        if ((DO.EngineerExperience)engineer.Experience < toAssignTask.DegreeOfDifficulty)
                        {
                            throw new BO.BlInvalidInputException($"The Engineer with ID {engineer.ID} could not be assigned to the Task with ID {toAssignTask.ID} because their experience level is too low.");
                        }

                        _dal.Task.Update(toAssignTask with { AssignedEngineerId = engineer.ID });
                    }
                }
            }

            //Try to add the Engineer to the data layer
            DO.Engineer newEngineer = new(engineer.ID, engineer.Name, engineer.Email, engineer.Cost, (DO.EngineerExperience)engineer.Experience, true);

            _dal.Engineer.Create(newEngineer);
        } 
        catch(DO.DalAlreadyExistsException exc) 
        {
            throw new BO.BlAlreadyExistsException(exc.Message);
        }   
    }

    public void DeleteEngineer(int id)
    {
        try
        {
            try
            {
                //Make sure the engineer is not assigned to a task
                if (_dal.Task.Read(t => t.AssignedEngineerId == id) is not null)
                {
                    throw new BO.BlCannotBeDeletedException($"This engineer with ID {id} could not be deleted because they are assigned to a Task.");
                }
            }
            catch (DO.DalDoesNotExistException)
            {
                // Not an error just means no assigned task
            }
            
            //try to delete the engineer from the DAL
            _dal.Engineer.Delete(id);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

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

    public void UpdateEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.ID <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !isValidEmail(engineer.Email))
            {
                throw new BO.BlInvalidInputException($"One of the fields of the Engineer with id {engineer.ID} was invalid");
            }

            //Get Engineer from DAL
            DO.Engineer dlEngineer = _dal.Engineer.Read(engineer.ID);

            //Check the Experience level is the same or higher and only change it if it is
            DO.EngineerExperience engineerExperience = (DO.EngineerExperience)engineer.Experience >= dlEngineer.Level ? (DO.EngineerExperience)engineer.Experience : dlEngineer.Level;

            //Make update DL type Engineer
            DO.Engineer newEngineer = new(engineer.ID, engineer.Name, engineer.Email, engineer.Cost, engineerExperience, true);

            //Call update on DL
            _dal.Engineer.Update(newEngineer);

            //Update the Task the Engineer is assigned to.
            //If we're in production
            if (_dal.Config.GetStartDate().HasValue)
            {
                //Update assigned task in DL (add assignedEngineerId to the task this engineer is assigned to)
                if (engineer.Task is not null)
                {
                    DO.Task toAssignTask;
                    try
                    {
                        toAssignTask = _dal.Task.Read(engineer.Task.ID);
                    }
                    catch (DalDoesNotExistException)
                    {
                        throw new BO.BlDoesNotExistException($"The Task with ID {engineer.Task.ID} which you are trying to assign to Engineer with ID {engineer.ID} does not exist.");
                    }

                    if (toAssignTask is not null)
                    {
                        //Check that the engineer has the same or higher experience level than the task
                        if ((DO.EngineerExperience)engineer.Experience < toAssignTask.DegreeOfDifficulty)
                        {
                            throw new BO.BlInvalidInputException($"The Engineer with ID {engineer.ID} could not be assigned to the Task with ID {toAssignTask.ID} because their experience level is too low.");
                        }

                        _dal.Task.Update(toAssignTask with { AssignedEngineerId = engineer.ID });
                    }
                }
            }
        }
        catch (DO.DalDoesNotExistException exc)
        {
            throw new BO.BlDoesNotExistException(exc.Message);
        }
    }

    public void Reset()
    {
        _dal.Engineer.Reset();
    }

    private static bool isValidEmail(string email)
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
        BO.TaskInEngineer? taskInEngineer = null;
        try
        {
            //Get task assigned to engineer
            DO.Task taskAssigned = _dal.Task.Read(t => t.AssignedEngineerId == engineer.ID);
            //Create TaskInEngineer
            taskInEngineer = new(taskAssigned.ID, taskAssigned.Nickname);
        }
        catch (DO.DalDoesNotExistException exc)
        {
            //This engineer isn't assigned to a task. Not an error...
        }
        
        //Make a BL type Engineer
        BO.Engineer blEngineer = new(engineer.ID, engineer.Name, engineer.Email, (BO.EngineerExperience)engineer.Level, engineer.Cost, taskInEngineer);

        return blEngineer;
    }
}
