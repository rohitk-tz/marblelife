namespace Jobs
{
    public interface ITaskScheduler
    {
        string Name { get; }
        void Run();
        void Stop();
    }
}
