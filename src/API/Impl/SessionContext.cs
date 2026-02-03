using System;
using Core.Application;
using Core.Users.ViewModels;

namespace Api
{
    //Dependency injector is defined in api
    public class SessionContext : ISessionContext
    {
        private const string Key = "_UserSessionModel_";
        private const string TokenKey = "_UserSessionToken_";
        private readonly IAppContextStore _appContextStore;
        public SessionContext(IAppContextStore appContextStore)
        {
            _appContextStore = appContextStore;
        }

        public string Token
        {
            get
            {

                return _appContextStore.Get(TokenKey) as string;
            }
            set
            {
                if (_appContextStore.Get(TokenKey) != null)
                {
                    _appContextStore.Remove(TokenKey);
                }
                _appContextStore.AddItem(TokenKey, value);
            }
        }

        public UserSessionModel UserSession
        {
            get
            {
                
                return _appContextStore.Get(Key) as UserSessionModel;
            }
            set
            {
                if (_appContextStore.Get(Key) != null)
                {
                    _appContextStore.Remove(Key);
                }
                _appContextStore.AddItem(Key, value);
            }
        }
    }
}