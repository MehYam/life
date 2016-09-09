using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    sealed class World
    {
        const long TICKS_PER_SECOND = 10000000;

        public Layer<Tile> map { get; set; }  //KAI: encaps.
        public Layer<Tile> items { get; set; }

        readonly int pixelsPerTile;
        public World(int width, int height, int pixelsPerTile)
        {
            map = new Layer<Tile>(width, height);
            items = new Layer<Tile>(width, height);
            this.pixelsPerTile = pixelsPerTile;
        }
        long _lastTick;
        public void Tick()
        {
            Console.WriteLine("tick");
            var now = new DateTime().Ticks;
            if (_lastTick >= 0)
            {
                var delta = now - _lastTick;

                float fDelta = ((float)delta) * TICKS_PER_SECOND;
                float fNow = ((float)now) * TICKS_PER_SECOND;

                // loop through actors, giving them their time slice
                foreach (var actor in _actors)
                {
                    actor.FixedUpdate(fNow, fDelta);
                }
            }
            _lastTick = now;
        }
        List<Actor> _actors = new List<Actor>();
        public void AddActor(Actor a)
        {
            if (!_actors.Contains(a))
            {
                _actors.Add(a);
            }
        }

        bool running;

        /// <summary>
        /// Starts the world's game loop
        /// </summary>
        /// <param name="fps">The number of physics/world frames to run per second</param>
        public void StartSimulation(int fps)
        {
            var millis = 1000 / fps;

            running = true;
            while (running)
            {
                Tick();
                System.Threading.Thread.Sleep(millis);
            }
        }
        public void StopSimulation()
        {
            running = false;
        }
    }
    struct Tile
    {
        public static char[] types = { 'O', 'o', '.', ' ', '#' };
        public static bool IsPassable(char type) { return type == ' ' || type == '.'; }

        public readonly char type;
        public Tile(char type)
        {
            this.type = type;
        }
        public Tile(int layer, int x, int y, char type)
        {
            this.type = type;
        }
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
