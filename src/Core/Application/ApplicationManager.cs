using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
    public static class ApplicationManager
    {
        public static IDependencyInjectionHelper DependencyInjection { get; set; }
        public static ISettings Settings { get; set; }
    }
}
