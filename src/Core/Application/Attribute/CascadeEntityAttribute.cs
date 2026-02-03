using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Attribute
{
    public class CascadeEntityAttribute : System.Attribute
    {
        public bool IsComposite { get; set; }

        public bool IsCollection { get; set; }

        public CascadeEntityAttribute(bool isComposite, bool isCollection = false)
        {
            IsCollection = isCollection;
            IsComposite = isComposite;
        }

        public CascadeEntityAttribute()
            : this(false)
        {
        }
    }
}
