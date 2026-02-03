namespace Core.Application
{
    public interface IAppContextStore
    {
        void AddItem(string key, object item);
        void UpdateItem(string key, object item);
        object Get(string key);
        void Remove(string key);
        void ClearStorage();
    }
}
