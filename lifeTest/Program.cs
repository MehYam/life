using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lifeEngine;
using System.IO;

namespace lifeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("------");
            //ArrayTest();
            //PointTest();
            //PerlinTest();
            //MapTest();
            //PathfindTest();
            //PathfindTravelTest();
            //FloodFillTest();
            //RoomDetectionTest();
            ThermodynamicsTest();
        }
        static void ArrayTest()
        {
            var foo = new int[3, 5];

            Console.WriteLine(string.Format("rank {0}, rows {1}, cols {2}",
                foo.Rank,
                foo.GetLength(0),
                foo.GetLength(1)
                ));
        }
        static void PointTest()
        {
            var a = new Point<int>(0, 0);
            var b = new Point<int>(2, 2);

            var c = new Point<float>(2.5f, 3.5f);
            var d = new Point<float>(1, 2);

            Console.WriteLine("IEquatable (should be true): " + (new Point<int>(2, 2) == b));
            Console.WriteLine("IEquatable (should be false): " + (a == b));
            Console.WriteLine("Copy (should be true): " + (new Point<int>(b) == b));
            Console.WriteLine(string.Format("Addition ({0}) ({1})", Util.Add(a, b), Util.Add(c, d)));
            Console.WriteLine(string.Format("Addition ({0}) ({1})", Util.Add(a, 3), Util.Add(c, 3.5f)));
            Console.WriteLine(string.Format("Subtraction ({0}) ({1})", Util.Subtract(a, b), Util.Subtract(c, d)));
            Console.WriteLine(string.Format("Subtraction ({0}) ({1})", Util.Subtract(a, 3), Util.Subtract(c, 3.5f)));
            Console.WriteLine(string.Format("Multiplication ({0}) ({1})", Util.Multiply(a, b), Util.Multiply(c, d)));
            Console.WriteLine(string.Format("Multiplication ({0}) ({1})", Util.Multiply(b, 3), Util.Multiply(c, 3)));
            Console.WriteLine(string.Format("Division ({0}) ({1})", Util.Divide(b, b), Util.Divide(c, d)));
            Console.WriteLine(string.Format("Division ({0}) ({1})", Util.Divide(b, 2), Util.Divide(c, 10)));
        }
        static void PerlinTest()
        {
            var noise = new Perlin();
            var steps = 30;
            for (double y = 0; y < steps; ++y)
            {
                for (double x = 0; x < steps; ++x)
                {
                    double perlin = noise.perlin(x + 0.5, y + 0.5, 0);
                    Console.Write(string.Format("{0:0.0} ", perlin));
                }
                Console.WriteLine();
            }
        }
        struct TestTile
        {
            public static char[] types = { 'O', 'o', '.', ' ', '#' };
            public static bool IsPassable(char type) { return type == ' ' || type == '.'; }

            public readonly int layer;
            public readonly Point<int> pos;
            public readonly char type;
            public TestTile(char type)
            {
                this.layer = 0;
                this.pos = new Point<int>(0, 0);
                this.type = type;
            }
            public TestTile(int layer, int x, int y, char type)
            {
                this.layer = layer;
                this.pos = new Point<int>(x, y);
                this.type = type;
            }
            public override string ToString()
            {
                return type.ToString();
            }
            public string ToStringLong()
            {
                return string.Format("L:{0}, {1} T:{2:00} |", layer, pos, type);
            }
        }
        static void PopulateLayer(Layer<TestTile> layer)
        {
            var noise = new Perlin();
            layer.Fill((x, y, oldTile) =>
            {
                double perlin = noise.perlin(x + 0.5, y + 0.5, 0);

                char tile = TestTile.types[(int)(perlin * TestTile.types.Length)];
                return new TestTile(0, x, y, tile);
            });
        }
        static void PopulateMap(Map<TestTile> map)
        {
            map.ForEachLayer(layer => PopulateLayer(layer));
        }
        static void MapTest()
        {
            // create a two-layer 3x4 map
            var map = new Map<int>(2, 3, 4);
            Console.WriteLine(map);

            // populate it
            var random = new Random();
            map.ForEachLayer(layer =>
            {
                layer.Fill((int x, int y, int oldTile) => random.Next(0, 100));
            });
            Console.WriteLine(map);

            var map2 = new Map<TestTile>(2, 50, 20);

            // create a noisy map
            PopulateMap(map2);
            Console.WriteLine(map2);
        }
        static void RenderPath(Layer<TestTile> layer, MapAStar<TestTile> search)
        {
            if (search.result.Count > 0)
            {
                for (int step = 0; step < search.result.Count; ++step)
                {
                    layer.Set(search.result[step], new TestTile((step % 10).ToString()[0]));
                }
                layer.Set(search.result[0], new TestTile('S'));
                Console.WriteLine(layer);
            }
            else
            {
                Console.WriteLine("No path?");
            }
        }
        static void RenderSearchExtent(Layer<TestTile> layer, MapAStar<TestTile> search)
        {
            foreach (var loc in search._closedList)
            {
                layer.Set(loc.Key, new TestTile('*'));
            }
            Console.WriteLine(layer);
        }
        static void PathfindTest()
        {
            var map = new Map<TestTile>(1, 100, 20);
            PopulateMap(map);
            Console.WriteLine(map);

            var search = new MapAStar<TestTile>();
            search.PathFind(map.Get(0), t => TestTile.IsPassable(t.type), new Point<int>(2, 0), new Point<int>(98, 19));

            RenderPath(map.Get(0), search);

            var solutionRender = new Layer<TestTile>(map.width, map.height);
            RenderSearchExtent(solutionRender, search);

            var layer = new Layer<TestTile>(100, 20);

            // create a bunch of walls
            layer.Fill((x, y, oldTile) =>
            {
                return new TestTile('.');
            });
            for (int c = 0; c < layer.size.x; c += 2)
            {
                for (int r = 1; r < layer.size.y; ++r)
                {
                    layer.Set(c, r, new TestTile('x'));
                }
                c += 2;
                for (int r = 0; r < layer.size.y - 1; ++r)
                {
                    layer.Set(c, r, new TestTile('x'));
                }
            }
            Console.WriteLine(layer);

            search = new MapAStar<TestTile>();
            search.PathFind(layer, (TestTile t) => TestTile.IsPassable(t.type), new Point<int>(0, 0), new Point<int>(99, 19));
            RenderPath(layer, search);
        }
        static void PathfindTravelTest()
        {
            var world = new World(100, 20);
            var noise = new Perlin();
            world.map.Fill((int x, int y, Tile oldTile) =>
            {
                double perlin = noise.perlin(x + 0.5, y + 0.5, 0);

                char tile = Tile.types[(int)(perlin * Tile.types.Length)];
                return new Tile(tile);
            });
            Console.WriteLine(world.map);

            var actorA = new Actor();
            var actorB = new Actor();
            actorA.pos = new Point<float>(3, 0);
            actorB.pos = new Point<float>(3, 14);
            world.AddActor(actorA);
            world.AddActor(actorB);

            actorA.AddPriority(new lifeEngine.behavior.MoveTo(world.map, actorA, actorB.pos.ToInt()));
            world.RunSimulation(10);
        }
        static void FloodFillTest()
        {
            var layer = Operations.LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt");

            //LayerFloodFill(layer, new Point<int>(0, 0), '-');
            //LayerFloodFill(layer, new Point<int>(4, 1), '-');
            Util.LayerFloodFill(layer, new Point<int>(5, 8), '-');
            //LayerFloodFill(layer, new Point<int>(34, 8), '-');
            //LayerFloodFill(layer, new Point<int>(layer.width-2, layer.height-1), '-');

            Console.WriteLine(layer);
        }
        static void RoomDetectionTest()
        {
            var layer = Operations.LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt");
            var world = new World(layer);
            world.RecalculateRooms();

            Console.WriteLine(world.rooms);
        }
        static Layer<string> RenderHeat(Layer<float> temps)
        {
            var retval = new Layer<string>(temps.size.x, temps.size.y);
            temps.ForEach((x, y, temp) =>
            {
//                retval.Set(x, y, string.Format("{0:0.0}|", temp));
                retval.Set(x, y, string.Format("{0} ", Math.Round(temp)));
            });
            return retval;
        }
        static void ThermodynamicsTest()
        {
            var layer = Operations.LoadLayerFile("c:\\source\\cs\\life\\simplerooms2.txt");
            Console.WriteLine(layer);

            var world = new World(layer);
            world.RecalculateRooms();

            // first, set the outside temperatures to 0 and indoors to 9
            const float AMBIENT = 0;
            const float INDOOR = 9;

            world.outdoorTemperature = AMBIENT;
            world.temps.Fill((x, y, tile) => world.rooms.Get(x, y).IsOutside ? world.outdoorTemperature : INDOOR);

            // next, punch ticks into the world, and render the heat as it conducts
            Console.WriteLine(RenderHeat(world.temps));
            while (true)
            {
                Console.ReadKey();

                world.Tick(0, 1);  // tick the world, one second at a time
                Console.WriteLine(RenderHeat(world.temps));
            }
        }
    }
}
