using Core.Application;
using System;
using System.Collections.Generic;
using System.IO;

namespace CustomerDataUpload
{
    public class AppContextStore : IAppContextStore
    {
        [ThreadStatic]
        private static Dictionary<string, object> Dictionary;

        public AppContextStore()
        {
            if (Dictionary == null) Dictionary = new Dictionary<string, object>();
        }

        public void AddItem(string key, object item)
        {
            if (Dictionary.ContainsKey(key))
                throw new InvalidDataException("Already an object exists.");

            if (item == null) throw new InvalidOperationException("Cannot add null to the Storage.");

            Dictionary.Add(key, item);
        }

        public void UpdateItem(string key, object item)
        {
            if (!Dictionary.ContainsKey(key))
                AddItem(key, item);
            else
            {
                Dictionary[key] = item;
            }
        }

        public object Get(string key)
        {
            if (Dictionary.ContainsKey(key))
                return Dictionary[key];

            return null;
        }

        public void Remove(string key)
        {
            if (Dictionary.ContainsKey(key))
                Dictionary.Remove(key);
        }

        public void ClearStorage()
        {
            foreach (var o in Dictionary)
            {
                if (o.Value is IDisposable)
                {
                    (o.Value as IDisposable).Dispose();
                }
            }

            Dictionary.Clear();
        }
    }
}
