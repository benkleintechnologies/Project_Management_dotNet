namespace BlApi;

public interface IBl
{
    public IEngineer Engineer { get; }
    public ITask task { get; }
    public IMilestone milestone { get; }
}
