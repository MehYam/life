using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
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
            public static bool IsPassable(char type) { return type == ' ' || type == '.';  }

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
            for (int c = 0; c < layer.width; c += 2)
            {
                for (int r = 1; r < layer.height; ++r)
                {
                    layer.Set(c, r, new TestTile('x'));
                }
                c += 2;
                for (int r = 0; r < layer.height - 1; ++r)
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
            actorA.pixelPos = World.TileToPixels(new Point<int>(3, 0));
            actorB.pixelPos = World.TileToPixels(new Point<int>(3, 14));
            world.AddActor(actorA);
            world.AddActor(actorB);

            actorA.AddPriority(new behavior.MoveTo(world.map, actorA, actorB));
            world.StartSimulation(10);
        }
        static Layer<Tile> LoadLayerFile(string path)
        {
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                var lines = contents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var retval = new Layer<Tile>(lines[0].Length, lines.Length);

                for (int y = 0; y < lines.Length; ++y)
                {
                    var line = lines[y];
                    var extent = Math.Min(line.Length, retval.width);
                    for (int x = 0; x < line.Length; ++x)
                    {
                        retval.Set(new Point<int>(x, y), new Tile(line[x]));
                    }
                }
                return retval;
            }
            Console.Error.WriteLine("Couldn't open " + path);
            return null;
        }
        static int LayerFloodFill(Layer<Tile> layer, Point<int> start, char fillColor)
        {
            return LayerFloodFill(layer, start, layer.Get(start).type, fillColor);
        }
        static int LayerFloodFill(Layer<Tile> layer, Point<int> start, char targetColor, char fillColor)
        {
            return LayerFloodFill(
                layer, 
                start,
                point => layer.Get(point).type == targetColor, // fillCondition
                point => layer.Set(point, new Tile(fillColor)) // fillAction
                ); 
        }
        static int LayerFloodFill(Layer<Tile> layer, Point<int> point, Func<Point<int>, bool> fillCondition, Action<Point<int>> fillAction)
        {
            // adapted from https://simpledevcode.wordpress.com/2015/12/29/flood-fill-algorithm-using-c-net/
            if (!fillCondition(point))
            {
                return 0;
            }
            Stack<Point<int>> points = new Stack<Point<int>>();
            points.Push(point);
            int pointsColored = 0;
            while (points.Count > 0)
            {
                Point<int> temp = points.Pop();
                int y1 = temp.y;
                while (y1 >= 0 && fillCondition(new Point<int>(temp.x, y1)))
                {
                    --y1;
                }
                ++y1;

                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < layer.height && fillCondition(new Point<int>(temp.x, y1)))
                {
                    fillAction(new Point<int>(temp.x, y1));
                    ++pointsColored;

                    if (!spanLeft && temp.x > 0 && fillCondition(new Point<int>(temp.x - 1, y1)))
                    {
                        points.Push(new Point<int>(temp.x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.x - 1 == 0 && fillCondition(new Point<int>(temp.x - 1, y1)))
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.x < layer.width - 1 && fillCondition(new Point<int>(temp.x + 1, y1)))
                    {
                        points.Push(new Point<int>(temp.x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.x < layer.width - 1 && fillCondition(new Point<int>(temp.x + 1, y1)))
                    {
                        spanRight = false;
                    }
                    ++y1;
                }
            }
            return pointsColored;
        }
        static void FloodFillTest()
        {
            var layer = LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt");

            //LayerFloodFill(layer, new Point<int>(0, 0), '-');
            //LayerFloodFill(layer, new Point<int>(4, 1), '-');
            LayerFloodFill(layer, new Point<int>(5, 8), '-');
            //LayerFloodFill(layer, new Point<int>(34, 8), '-');
            //LayerFloodFill(layer, new Point<int>(layer.width-2, layer.height-1), '-');

            Console.WriteLine(layer);
        }
        static Layer<Tile> DetectRooms(Layer<Tile> layer)
        {
            // flood fill each unique contiguous empty region with something unique.  We'll make a copy first so as to
            // not disturb the original map
            var layerCopy = new Layer<Tile>(layer);
            Console.WriteLine(layerCopy);

            char region = 'a';
            layerCopy.ForEach((x, y, tile) =>
            {
                int tilesFilled = LayerFloodFill(layerCopy, new Point<int>(x, y), ' ', region);
                if (tilesFilled > 0)
                {
                    ++region;
                }
            });
            return layerCopy;
        }
        static void RoomDetectionTest()
        {
            var layer = LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt");
            var layerRooms = DetectRooms(layer);

            Console.WriteLine(layerRooms);
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
        static Layer<char> RenderHeat(Layer<float> temps)
        {
            var retval = new Layer<char>(temps.width, temps.height);
            temps.ForEach((x, y, temp) =>
            {
                char tempChar = '@';
                if (temp < -15) tempChar = ' ';
                else if (temp < -10) tempChar = '.';
                else if (temp < -5) tempChar = ',';
                else if (temp < 0) tempChar = 'c';
                else if (temp < 5) tempChar = 'o';
                else if (temp < 10) tempChar = 'd';
                else if (temp < 15) tempChar = 'B';
                else if (temp < 20) tempChar = '8';

                retval.Set(x, y, tempChar);
            });
            return retval;
        }
        static void ThermodynamicsTest()
        {
            var layer = DetectRooms(LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt"));

            Console.WriteLine(layer);

            var temps = new Layer<float>(layer.width, layer.height);

            // first, set the outside temperature to -20
            temps.Fill((x, y, tile) => layer.Get(x, y).IsOutside ? -20 : 20);

            while (true)
            {
                Console.WriteLine(RenderHeat(temps));

                // next, loop all tiles, radiating warmer temps into colder ones
                temps.ForEach((x, y, tile) =>
                {
                    if (x > 0) ExchangeHeat(layer, temps, x, y, x - 1, y);
                    if (y > 0) ExchangeHeat(layer, temps, x, y, x, y - 1);
                    if (x < layer.width - 1) ExchangeHeat(layer, temps, x, y, x + 1, y);
                    if (y < layer.height - 1) ExchangeHeat(layer, temps, x, y, x, y + 1);
                });

                Console.ReadKey();
            }
        }
    }
}
