//#define DEBUG_MOVE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine.behavior
{
    public sealed class MoveTo : IBehavior
    {
        readonly Actor mover;
        readonly List<Point<int>> path;
        public MoveTo(World world, Actor mover, Point<int> target)
        {
            this.mover = mover;

            // determine the path, set the actor along it
            var search = new MapAStar<Tile>();

            search.PathFind(world.walls, t => { return t.IsEmpty; }, mover.pos.ToInt(), target);

            // There's no deque in C#.  To avoid shifting the array each time we remove an item, just reverse the list and work backward.
            //KAI: just turn this into LinkedList...
            search.result.Reverse();
            path = search.result;

#if DEBUG_MOVE
            Console.WriteLine("MoveTo evaluated path with {0} tiles", path.Count);
#endif
        }
        public void FixedUpdate(float time, float deltaTime)
        {
            if (!IsComplete)
            {
                float travelSoFar = 0;
                float speed = mover.speedTPS;
                float maxTravel = deltaTime * speed;

                while (!IsComplete && travelSoFar < maxTravel)
                {
                    Point<float> currentDestination = path.Last().ToFloat();

                    // if the current tile destination is reached, pull it off the queue and start the next one
                    if (Util.NearlyEqual(mover.pos, currentDestination))
                    {
                        path.RemoveAt(path.Count - 1);
                        continue;
                    }
#if DEBUG_MOVE
                    Point<float> debug = mover.pos;
#endif
                    // move the actor to the next tile
                    Point<float> currentVector = Util.Subtract(currentDestination, mover.pos);

                    float travelToCurrent = Util.Magnitude(currentVector);
                    float travelNow = Math.Min(maxTravel, travelToCurrent);
                    Point<float> currentVectorNormalized = Util.Divide(currentVector, travelToCurrent);

                    mover.pos = Util.Add(mover.pos, Util.Multiply(currentVectorNormalized, travelNow));
                    travelSoFar += travelNow;

#if DEBUG_MOVE
                    Console.WriteLine(string.Format("Actor move from {0} to {1}, {2} tiles remaining", debug, mover.pos, path.Count));
#endif
                }
            }
        }
        public bool IsComplete { get { return path.Count == 0; } }
    }
}
