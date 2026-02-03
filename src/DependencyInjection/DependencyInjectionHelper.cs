using Core.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public class DependencyInjectionHelper : IDependencyInjectionHelper
    {
        public T Resolve<T>()
        {
            return IoC.Resolve<T>();
        }

        public object Resolve(Type obj)
        {
            return IoC.Resolve(obj);
        }

        public T Resolve<T>(string name)
        {
            return IoC.Resolve<T>(name);
        }

        public void Register<TAbstract, TConcrete>() where TConcrete : TAbstract
        {
            IoC.Register<TAbstract, TConcrete>();
        }
    }
}
