namespace BlApi;

public interface IBl
{
    public IConfig Config { get;}
    public IEngineer Engineer { get; }
    public ITask Task { get; }
    public IMilestone Milestone { get; }
}
