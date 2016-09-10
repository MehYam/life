using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life.behavior
{
    class MoveTo : IBehavior
    {
        readonly Actor mover;
        readonly Actor target;  // this should instead be a generic target, which can be either an Actor or location on the map

        const float speed = 1;

        //KAI: this astar interface is shod
        readonly MapAStar<Tile> search;
        Point<int> currentTileStep;

        public MoveTo(Layer<Tile> map, Actor mover, Actor target)
        {
            this.mover = mover;
            this.target = target;

            // determine the path, set the actor along it
            search = new MapAStar<Tile>();
            search.PathFind(map, (Tile t) => Tile.IsPassable(t.type), World.PixelsToTile(mover.pixelPos), World.PixelsToTile(target.pixelPos));
        }
        public void FixedUpdate(float time, float deltaTime)
        {
            if (!IsComplete)
            {
                const float speedInPixels = speed * World.PIXELS_PER_TILE;

                Point<float> currentDestination = World.TileToPixels(currentTileStep);
                Point<float> distanceVector = Util.Subtract(currentDestination, mover.pixelPos);

                Console.WriteLine(distanceVector);
                // if reached destination
                // cost = search._closedList[currentTile];
                // if (

                // next destination =>  currentTile = 
            }
        }
        public bool IsComplete
        {
            get
            {
                return false;
                //return search.solution == null || search._closedList[currentTileStep].parentIndex == 0;
            }
        }
    }
}
