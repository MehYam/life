using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lifeEngine
{
    public class Tile
    {
        public static char[] types = { 'O', 'o', '.', ' ', '#' };

        public readonly char type;
        public Tile(char type)
        {
            this.type = type;
        }
        public bool IsEmpty {  get { return type == ' ' || type == '.' ;  } }
        public float Mass { get { return type == '#' ? 50 : 1; } }  // temporary, until we figure this out
        public override string ToString()
        {
            return type.ToString();
        }
    }
    public sealed class World
    {
        public Layer<Tile> ground { get; set; }
        public Layer<Tile> walls { get; set; }  //KAI: abstract these, appropriately.
        public Layer<int> rooms { get; private set; }  // index into the room #.  0 == not a room
        public Layer<float> temps { get; set; }
        public float outdoorTemperature { get; set; }

        /// <summary>
        /// How quickly heat spreads.  It's defined as the percentage of heat difference transferred per sec, range (0-1).
        /// So, heatConductivity = 1 means it takes one second for heat to completely transfer, 0 means perfect insultation.
        /// </summary>
        public float heatConductivity { get; set; }

        public World(int width, int height)
        {
            Init(new Layer<Tile>(width, height), width, height);
        }
        public World(Layer<Tile> mapLayer)
        {
            Init(mapLayer, mapLayer.size.x, mapLayer.size.y);
        }
        void Init(Layer<Tile> walls, int width, int height)
        {
            this.walls = walls;
            temps = new Layer<float>(width, height);
            rooms = new Layer<int>(width, height);

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
                    ExchangeHeat(walls, temps, x, y, x - 1, y, maxHeatExchange);
                    if (x < walls.size.x - 1) ExchangeHeat(walls, temps, x, y, x + 1, y, maxHeatExchange);
                }
            }
            for (var y = 1; y < temps.size.y; y += 2)
            {
                for (var x = 0; x < temps.size.x; ++x)
                {
                    ExchangeHeat(walls, temps, x, y, x, y - 1, maxHeatExchange);
                    if (y < walls.size.y - 1) ExchangeHeat(walls, temps, x, y, x, y + 1, maxHeatExchange);
                }
            }
        }
        void ApplyAmbientTemperature(float deltaTime)
        {
            // trend outdoor tiles to the ambient weather-influenced temperature
            temps.ForEach((x, y, tile) =>
            {
                if (IsOutside(x, y))
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
        static Layer<int> DetectRooms(Layer<Tile> walls)
        {
            // use flood fill to find all the contiguous regions that aren't walls.

            // start by generating a stencil of the walls.
            var rooms = Operations.CreateLayerMask(walls);

            int roomIndex = 1;
            rooms.ForEach((x, y, tile) =>
            {
                int tilesFilled = Util.LayerFloodFill(rooms, new Point<int>(x, y), 0, roomIndex);
                if (tilesFilled > 0)
                {
                    ++roomIndex;
                }
            });
            return rooms;
        }
        public void RecalculateRooms()
        {
            rooms = DetectRooms(walls);
        }
        public bool IsOutside(int x, int y)
        {
            var roomId = rooms.Get(x, y);
            return (roomId == 0 || roomId == 1) && walls.Get(x, y).IsEmpty;
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
}
