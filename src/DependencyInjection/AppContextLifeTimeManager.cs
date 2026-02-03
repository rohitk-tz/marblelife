using Core.Application;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public class AppContextLifetimeManager : LifetimeManager
    {
        private readonly IAppContextStore _appContextStore;
        private readonly string _key;

        public AppContextLifetimeManager(IAppContextStore appContextStore)
        {
            _appContextStore = appContextStore;
            _key = Guid.NewGuid().ToString();
        }


        public AppContextLifetimeManager(IAppContextStore appContextStore, string key)
        {
            _appContextStore = appContextStore;
            _key = key;
        }

        public override object GetValue()
        {
            return _appContextStore.Get(_key);
        }

        public override void SetValue(object newValue)
        {
            _appContextStore.AddItem(_key, newValue);
        }

        public override void RemoveValue()
        {
            throw new NotImplementedException();
        }
    }

    public class WinJobContextLifetimeManager : LifetimeManager
    {
        private readonly string _key;
        private readonly object _instance;

        public WinJobContextLifetimeManager(string key, object instance)
        {
            _key = key;
            _instance = instance;
        }

        public override object GetValue()
        {
            return _instance;
        }

        public override void SetValue(object newValue)
        {
            throw new NotImplementedException();
        }

        public override void RemoveValue()
        {
            throw new NotImplementedException();
        }
    }
}
