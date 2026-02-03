using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Exceptions
{
    public class InvalidDependencyRegistrationException : CustomBaseException
    {
        public InvalidDependencyRegistrationException(Type baseType, Type implType)
            : base(string.Format("Type {0} cannot be used to register type {1}", implType.Name, baseType.Name))
        {
        }
    }
}
