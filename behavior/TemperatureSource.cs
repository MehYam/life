using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine.behavior
{
    public sealed class TemperatureSource : IBehavior
    {
        readonly World world;
        readonly Point<int> position;
        public TemperatureSource(World world, Point<int> position, float temp)
        {
            this.world = world;
            this.position = position;
            this.temp = temp;
        }
        public float temp { get; set; }

        public bool IsComplete
        {
            get
            {
                return false;
            }
        }

        public void FixedUpdate(float time, float deltaTime)
        {
            //KAI: not quite right - this should probably lerp temperatures instead of just setting them
            world.temps.Set(position, temp);
        }
    }
}
