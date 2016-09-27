using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using lifeEngine.behavior;

namespace lifeEngine
{
    public class Actor : ITickHandler
    {
        public readonly char type;
        public readonly float speedTPS = 3;
        public Point<float> pos = new Point<float>(0, 0);  // Actors get sub-tile precision
        public Actor(char type = 'A')
        {
            this.type = type;
        }
        public List<IBehavior> _priorities = new List<IBehavior>();
        public void FixedUpdate(float time, float deltaTime)
        {
            if (_priorities.Count > 0)
            {
                _priorities[0].FixedUpdate(time, deltaTime);
                if (_priorities[0].IsComplete)
                {
                    _priorities.RemoveAt(0);
                }
            }
        }
        public void AddPriority(IBehavior behavior)
        {
            _priorities.Add(behavior);
        }
        public override string ToString()
        {
            return string.Format("{0}, ({1})", type, pos);
        }
    }
}
