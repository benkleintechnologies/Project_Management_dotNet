namespace BlImplementation;
using BlApi;
using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class EngineerImplementation : IEngineer
{
    private DalApi.IDal _dal = DalApi.Factory.Get;
    public void addEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !IsValidEmail(engineer.Email))
            {
                throw new BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Try to add the Engineer to the data layer
            DO.Engineer newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, (DO.EngineerExperience)engineer.Experience, true);

            _dal.Engineer.Create(newEngineer);
        } 
        catch(DalAlreadyExistsException exc) 
        {
            throw new BlAlreadyExistsException(exc.Message);
        }   
    }

    public void deleteEngineer(int Id)
    {
        try
        {
            //Make sure the engineer is not assigned to a task
            if (_dal.Task.Read(t => t.assignedEngineerId == Id) is null)
            {
                throw new BlCannotBeDeletedException($"This engineer with ID {Id} could not be deleted because they are assigned to a Task.");
            }
            //try to delete the engineer from the DAL
            _dal.Engineer.Delete(Id);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public BO.Engineer getEngineer(int Id)
    {
        try
        {
            //Get Engineer from DAL
            DO.Engineer engineer = _dal.Engineer.Read(Id);
            //Make a BL type Engineer
            return toBlEngineer(engineer);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public IEnumerable<BO.Engineer> getListOfEngineers(Func<BO.Engineer, bool>? filter = null)
    {
        try
        {
            //Get all Engineers from the DL
            IEnumerable<DO.Engineer> engineers = _dal.Engineer.ReadAll();
            //Filter the DL objects based on the filter
            IEnumerable<DO.Engineer> filteredDlEngineers = filter != null ? engineers.Where(e => filter(toBlEngineer(e))) : engineers;
            //Return the list of BL typ Engineers
            return filteredDlEngineers.Select(toBlEngineer);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    public void updateEngineer(BO.Engineer engineer)
    {
        try
        {
            //Check for invalid data
            if (engineer.Id <= 0 || engineer.Name == "" || engineer.Cost <= 0 || !IsValidEmail(engineer.Email))
            {
                throw new BlInvalidInputException($"One of the fields of the Engineer with id {engineer.Id} was invalid");
            }

            //Get Engineer from DAL
            DO.Engineer dlEngineer = _dal.Engineer.Read(engineer.Id);

            //Check the Experience level is the same or higher
            DO.EngineerExperience engineerExperience = (DO.EngineerExperience)engineer.Experience >= dlEngineer.level ? (DO.EngineerExperience)engineer.Experience : dlEngineer.level;

            //Make update DL type Engineer
            DO.Engineer newEngineer = new(engineer.Id, engineer.Name, engineer.Email, engineer.Cost, engineerExperience, true);

            //Call update on DL
            _dal.Engineer.Update(newEngineer);
        }
        catch (DalDoesNotExistException exc)
        {
            throw new BlDoesNotExistException(exc.Message);
        }
    }

    static bool IsValidEmail(string email)
    {
        // Define a regular expression pattern for a simple email validation
        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        // Create a Regex object and match the email against the pattern
        Regex regex = new Regex(pattern);
        Match match = regex.Match(email);

        // Return true if there is a match, indicating a valid email
        return match.Success;
    }

    private BO.Engineer toBlEngineer(DO.Engineer engineer)
    {
        //Get task assigned to engineer
        DO.Task taskAssigned = _dal.Task.Read(t => t.assignedEngineerId == engineer.id);
        //Create TaskInEngineer
        TaskInEngineer taskInEngineer = new(taskAssigned.id, taskAssigned.nickname);
        //Make a BL type Engineer
        BO.Engineer blEngineer = new(engineer.id, engineer.name, engineer.email, (BO.EngineerExperience)engineer.level, engineer.cost, taskInEngineer);

        return blEngineer;
    }

}
