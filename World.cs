using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine
{
    public sealed class World
    {
        public Layer<Tile> map { get; set; }  //KAI: encaps.
        public Layer<Tile> items { get; set; }
        public World(int width, int height)
        {
            map = new Layer<Tile>(width, height);
            items = new Layer<Tile>(width, height);
        }
        public World(Layer<Tile> mapLayer)
        {
            map = mapLayer;
            items = new Layer<Tile>(mapLayer.size.x, mapLayer.size.y);
        }
        const long DATETIME_TICKS_PER_SEC = 10 * 1000 * 1000;
        long _lastTick;
        void Tick()
        {
            var now = DateTime.Now.Ticks;
            if (_lastTick > 0)
            {
                var delta = now - _lastTick;

                float fDelta = ((float)delta) / DATETIME_TICKS_PER_SEC;
                float fNow = ((float)now) / DATETIME_TICKS_PER_SEC;

                //Console.WriteLine(string.Format("{0:0.00000}", fDelta));
                Tick(fNow, fDelta);
            }
            _lastTick = now;
        }
        public void Tick(float time, float deltaTime)
        {
            //KAI: apples and oranges with the two Tick calls w.r.t. _lastTick
            foreach (var actor in _actors)
            {
                actor.FixedUpdate(time, deltaTime);
            }
        }
        readonly public List<Actor> _actors = new List<Actor>(); //KAI: shod
        public void AddActor(Actor a)
        {
            if (!_actors.Contains(a))
            {
                _actors.Add(a);
            }
        }
        public void RemoveActor(Actor a)
        {
            _actors.Remove(a);
        }
        public Actor FindActor(Func<Actor, bool> find)
        {
            foreach (var actor in _actors)
            {
                if (find(actor)) return actor;
            }
            return null;
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
    public class Tile
    {
        public static char[] types = { 'O', 'o', '.', ' ', '#' };

        public readonly char type;
        public Tile(char type)
        {
            this.type = type;
        }
        //KAI: hacks for now, until things get formalized a bit more
        public bool IsWall { get { return type == '#'; } }
        public bool IsOutside {  get { return type == 'a' || type == ' '; } }
        public bool IsRoom { get { return type > 'a' && type < 'z';  } }
        public bool IsPassable { get { return type == '.' || IsOutside; } }
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
