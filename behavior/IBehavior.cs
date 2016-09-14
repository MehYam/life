using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifeEngine.behavior
{
    interface IBehavior : ITickHandler
    {
        bool IsComplete { get; }
    }
}
