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
        public Layer<Tile> rooms { get; private set; }
        public Layer<float> temps { get; set; }
        public float outdoorTemperature { get; set; }

        /// <summary>
        /// How quickly heat spreads.  It's defined as the percentage of heat difference transferred per sec, range (0-1).
        /// So, heatConductivity = 1 means it takes one second for heat to completely transfer, 0 means perfect insultation.
        /// </summary>
        public float heatConductivity { get; set; }

        public World(int width, int height)
        {
            map = new Layer<Tile>(width, height);
            items = new Layer<Tile>(width, height);
            temps = new Layer<float>(width, height);

            heatConductivity = 0.25f;
        }
        public World(Layer<Tile> mapLayer)
        {
            map = mapLayer;
            items = new Layer<Tile>(mapLayer.size.x, mapLayer.size.y);
            temps = new Layer<float>(mapLayer.size.x, mapLayer.size.y);

            heatConductivity = 0.25f;
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
            SpreadTemperature(deltaTime);
            ApplyAmbientTemperature(deltaTime);

            //KAI: apples and oranges with the two Tick calls w.r.t. _lastTick
            foreach (var actor in _actors)
            {
                actor.FixedUpdate(time, deltaTime);
            }
        }
        void ExchangeHeat(Layer<Tile> layer, Layer<float> layerTemps, int ax, int ay, int bx, int by, float deltaTime)
        {
            float Ta = layerTemps.Get(ax, ay);
            float Tb = layerTemps.Get(bx, by);

            if (!Util.NearlyEqual(Ta, Tb))
            {
                var a = layer.Get(ax, ay);
                var b = layer.Get(bx, by);

                // Heat = Mass x Temp in our simplified thermodynamics model
                float Ha = a.Mass * Ta;
                float Hb = b.Mass * Tb;
                float Htotal = Ha + Hb;

                // Calculate the temperature both tiles would reach if they fully stabilized to equilibrium
                float Tequilibrium = Htotal / (a.Mass + b.Mass);

                // Exchange the heat over time
                float Hdelta = (Tequilibrium - Ta) * a.Mass;
                float HdeltaMitigatedOverTime = Math.Min(Hdelta, Hdelta * deltaTime * heatConductivity);

                layerTemps.Set(ax, ay, Ta + (HdeltaMitigatedOverTime / a.Mass));
                layerTemps.Set(bx, by, Tb - (HdeltaMitigatedOverTime / b.Mass));

                //if (ax == 3 && ay == 1)
                //Console.WriteLine(string.Format("({0},{1}) <=> ({2},{3}), Ta {4:0.00} Tb {5:0.00}, Teq {6:0.00}, Ta' {7:0.00}, Tb' {8:0.00}",
                //    ax,
                //    ay,
                //    bx,
                //    by,
                //    Ta,
                //    Tb,
                //    Tequilibrium,
                //    layerTemps.Get(ax, ay),
                //    layerTemps.Get(bx, by)
                //    ));
            }
        }
        void SpreadTemperature(float deltaTime)
        {
            // loop all tiles, radiating warmer temps into colder ones.  Do this in two passes for each of the horizontal
            // and vertical directions to make the spread more uniform.
            float maxHeatExchange = deltaTime * heatConductivity;

            for (var x = 1; x < temps.size.x; x += 2)
            {
                for (var y = 0; y < temps.size.y; ++y)
                {
                    ExchangeHeat(map, temps, x, y, x - 1, y, maxHeatExchange);
                    if (x < map.size.x - 1) ExchangeHeat(map, temps, x, y, x + 1, y, maxHeatExchange);
                }
            }
            for (var y = 1; y < temps.size.y; y += 2)
            {
                for (var x = 0; x < temps.size.x; ++x)
                {
                    ExchangeHeat(map, temps, x, y, x, y - 1, maxHeatExchange);
                    if (y < map.size.y - 1) ExchangeHeat(map, temps, x, y, x, y + 1, maxHeatExchange);
                }
            }
        }
        void ApplyAmbientTemperature(float deltaTime)
        {
            // trend outdoor tiles to the ambient weather-influenced temperature
            temps.ForEach((x, y, tile) =>
            {
                if (rooms.Get(x, y).IsOutside)
                {
                    var temp = temps.Get(x, y);
                    if (temp != outdoorTemperature)
                    {
                        temps.Set(x, y, Util.Lerp(temp, outdoorTemperature, heatConductivity * deltaTime));
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
        //KAI: very leaky abstractions here, need to think about the integrity of the World object
        static Layer<Tile> DetectRooms(Layer<Tile> layer)
        {
            // flood fill each unique contiguous empty region with something unique.  We'll make a copy first so as to
            // not disturb the original map
            var layerCopy = new Layer<Tile>(layer);

            char region = 'a';
            layerCopy.ForEach((x, y, tile) =>
            {
                int tilesFilled = Util.LayerFloodFill(layerCopy, new Point<int>(x, y), ' ', region);
                if (tilesFilled > 0)
                {
                    ++region;
                }
            });
            return layerCopy;
        }
        public void RecalculateRooms()
        {
            rooms = DetectRooms(map);
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

        public float Mass { get { return IsWall ? 50 : 1; } }
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
