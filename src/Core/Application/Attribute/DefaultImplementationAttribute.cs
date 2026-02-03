using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Attribute
{
    public class DefaultImplementationAttribute : System.Attribute
    {
        public Type Interface { get; set; }
    }
}
