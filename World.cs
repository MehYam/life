using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    sealed class World<T>
    {
        public Map<T> map {get;set;}
    }
}
