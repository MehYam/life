using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine.behavior
{
    public interface ITickHandler
    {
        void FixedUpdate(float time, float deltaTime);
    }
}
