using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life.behavior
{
    interface ITickHandler
    {
        void FixedUpdate(float time, float deltaTime);
    }
}
