using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine
{
    public sealed class World
    {
        public Layer<Tile> map { get; set; }  //KAI: abstract these, appropriately.
        public Layer<Tile> items { get; set; }
        public Layer<float> temps { get; set; }
        public float outdoorTemperature { get; set; }

        public World(int width, int height)
        {
            map = new Layer<Tile>(width, height);
            items = new Layer<Tile>(width, height);
        }
        public World(Layer<Tile> mapLayer)
        {
            map = mapLayer;
            items = new Layer<Tile>(mapLayer.size.x, mapLayer.size.y);
            temps = new Layer<float>(mapLayer.size.x, mapLayer.size.y);
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
            ConductHeat();
            ApplyOutdoorTemperature();

            //KAI: apples and oranges with the two Tick calls w.r.t. _lastTick
            foreach (var actor in _actors)
            {
                actor.FixedUpdate(time, deltaTime);
            }
        }
        static void ExchangeHeat(Layer<Tile> layer, Layer<float> layerTemps, int ax, int ay, int bx, int by)
        {
            var tempA = layerTemps.Get(ax, ay);
            var tempB = layerTemps.Get(bx, by);

            if (tempA != tempB)
            {
                var tileA = layer.Get(ax, ay);
                var tileB = layer.Get(bx, by);
                if (!tileA.IsOutside || !tileB.IsOutside)
                {
                    var temp = (tempA + tempB) / 2;
                    layerTemps.Set(ax, ay, temp);
                    layerTemps.Set(bx, by, temp);
                }
            }
        }
        void ConductHeat()
        {
            // loop all tiles, radiating warmer temps into colder ones
            temps.ForEach((x, y, tile) =>
            {
                if (x > 0) ExchangeHeat(map, temps, x, y, x - 1, y);
                if (y > 0) ExchangeHeat(map, temps, x, y, x, y - 1);
                if (x < map.size.x - 1) ExchangeHeat(map, temps, x, y, x + 1, y);
                if (y < map.size.y - 1) ExchangeHeat(map, temps, x, y, x, y + 1);
            });
        }
        void ApplyOutdoorTemperature()
        {
            // trend outdoor tiles to the ambient weather-influenced temperature
            temps.ForEach((x, y, tile) =>
            {
                if (map.Get(x, y).IsOutside)
                {
                    var temp = temps.Get(x, y);
                    if (temp != outdoorTemperature)
                    {
                        temps.Set(x, y, (temp + outdoorTemperature) / 2);
                    }
                }
            });
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
        public void RunSimulation(int fps)
        {
            var millis = 1000 / fps;

            running = true;
            while (running)
            {
                Tick();
                System.Threading.Thread.Sleep(millis);
            }
        }
        /// <summary>
        /// Experimental placeholder - this would have to come in on a different thread than RunSimulation()
        /// </summary>
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
