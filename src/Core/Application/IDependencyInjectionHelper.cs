using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
    public interface IDependencyInjectionHelper
    {
        T Resolve<T>();
        object Resolve(Type obj);
        T Resolve<T>(string name);
        void Register<TAbstract, TConcrete>() where TConcrete : TAbstract;
    }
}
