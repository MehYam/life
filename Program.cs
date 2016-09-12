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
            RoomDetectionTest();
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
        struct Tile
        {
            public static char[] types = { 'O', 'o', '.', ' ', '#' };
            public static bool IsPassable(char type) { return type == ' ' || type == '.';  }

            public readonly int layer;
            public readonly Point<int> pos;
            public readonly char type;
            public Tile(char type)
            {
                this.layer = 0;
                this.pos = new Point<int>(0, 0);
                this.type = type;
            }
            public Tile(int layer, int x, int y, char type)
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
        static void PopulateLayer(Layer<Tile> layer)
        {
            var noise = new Perlin();
            layer.Fill((int x, int y, Tile oldTile) =>
            {
                double perlin = noise.perlin(x + 0.5, y + 0.5, 0);

                char tile = Tile.types[(int)(perlin * Tile.types.Length)];
                return new Tile(0, x, y, tile);
            });
        }
        static void PopulateMap(Map<Tile> map)
        {
            map.ForEachLayer((Layer<Tile> layer) => PopulateLayer(layer));
        }
        static void MapTest()
        { 
            // create a two-layer 3x4 map
            var map = new Map<int>(2, 3, 4);
            Console.WriteLine(map);

            // populate it
            var random = new Random();
            map.ForEachLayer((Layer<int> layer) =>
            {
                layer.Fill((int x, int y, int oldTile) => random.Next(0, 100));
            });
            Console.WriteLine(map);

            var map2 = new Map<Tile>(2, 50, 20);

            // create a noisy map
            PopulateMap(map2);
            Console.WriteLine(map2);
        }
        static void RenderPath(Layer<Tile> layer, MapAStar<Tile> search)
        {
            if (search.result.Count > 0)
            {
                for (int step = 0; step < search.result.Count; ++step)
                {
                    layer.Set(search.result[step], new Tile((step % 10).ToString()[0]));
                }
                layer.Set(search.result[0], new Tile('S'));
                Console.WriteLine(layer);
            }
            else
            {
                Console.WriteLine("No path?");
            }
        }
        static void RenderSearchExtent(Layer<Tile> layer, MapAStar<Tile> search)
        {
            foreach (var loc in search._closedList)
            {
                layer.Set(loc.Key, new Tile('*'));
            }
            Console.WriteLine(layer);
        }
        static void PathfindTest()
        {
            var map = new Map<Tile>(1, 100, 20);
            PopulateMap(map);
            Console.WriteLine(map);

            var search = new MapAStar<Tile>();
            search.PathFind(map.Get(0), (Tile t) => Tile.IsPassable(t.type), new Point<int>(2, 0), new Point<int>(98, 19));

            RenderPath(map.Get(0), search);

            var solutionRender = new Layer<Tile>(map.width, map.height);
            RenderSearchExtent(solutionRender, search);

            var layer = new Layer<Tile>(100, 20);

            // create a bunch of walls
            layer.Fill((int x, int y, Tile oldTile) =>
            {
                return new Tile('.');
            });
            for (int c = 0; c < layer.width; c += 2)
            {
                for (int r = 1; r < layer.height; ++r)
                {
                    layer.Set(c, r, new Tile('x'));
                }
                c += 2;
                for (int r = 0; r < layer.height - 1; ++r)
                {
                    layer.Set(c, r, new Tile('x'));
                }
            }
            Console.WriteLine(layer);

            search = new MapAStar<Tile>();
            search.PathFind(layer, (Tile t) => Tile.IsPassable(t.type), new Point<int>(0, 0), new Point<int>(99, 19));
            RenderPath(layer, search);
        }
        static void PathfindTravelTest()
        {
            var world = new World(100, 20);
            var noise = new Perlin();
            world.map.Fill((int x, int y, life.Tile oldTile) =>
            {
                double perlin = noise.perlin(x + 0.5, y + 0.5, 0);

                char tile = life.Tile.types[(int)(perlin * life.Tile.types.Length)];
                return new life.Tile(tile);
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
        static Layer<life.Tile> LoadLayerFile(string path)
        {
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                var lines = contents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var retval = new Layer<life.Tile>(lines[0].Length, lines.Length);

                for (int y = 0; y < lines.Length; ++y)
                {
                    var line = lines[y];
                    var extent = Math.Min(line.Length, retval.width);
                    for (int x = 0; x < line.Length; ++x)
                    {
                        retval.Set(new Point<int>(x, y), new life.Tile(line[x]));
                    }
                }
                return retval;
            }
            Console.Error.WriteLine("Couldn't open " + path);
            return null;
        }
        static void LayerFloodFill(Layer<life.Tile> layer, Point<int> point, char fillColor)
        {
            // adapted from https://simpledevcode.wordpress.com/2015/12/29/flood-fill-algorithm-using-c-net/
            Stack<Point<int>> points = new Stack<Point<int>>();

            var targetColor = layer.Get(point).type;
            if (targetColor == fillColor)
            {
                return;
            }

            points.Push(point);
            while (points.Count > 0)
            {
                Point<int> temp = points.Pop();
                int y1 = temp.y;
                while (y1 >= 0 && layer.Get(temp.x, y1).type == targetColor)
                {
                    --y1;
                }
                ++y1;

                bool spanLeft = false;
                bool spanRight = false;
                while (y1 < layer.height && layer.Get(temp.x, y1).type == targetColor)
                {
                    layer.Set(temp.x, y1, new life.Tile(fillColor));
                    if (!spanLeft && temp.x > 0 && layer.Get(temp.x - 1, y1).type == targetColor)
                    {
                        points.Push(new Point<int>(temp.x - 1, y1));
                        spanLeft = true;
                    }
                    else if (spanLeft && temp.x - 1 == 0 && layer.Get(temp.x - 1, y1).type != targetColor)
                    {
                        spanLeft = false;
                    }
                    if (!spanRight && temp.x < layer.width - 1 && layer.Get(temp.x + 1, y1).type == targetColor)
                    {
                        points.Push(new Point<int>(temp.x + 1, y1));
                        spanRight = true;
                    }
                    else if (spanRight && temp.x < layer.width - 1 && layer.Get(temp.x + 1, y1).type != targetColor)
                    {
                        spanRight = false;
                    }
                    ++y1;
                }
            }
        }
        static void RoomDetectionTest()
        {
            var layer = LoadLayerFile("c:\\source\\cs\\life\\simplerooms1.txt");

            Console.WriteLine(layer);

            //LayerFloodFill(layer, new Point<int>(0, 0), '-');
            //LayerFloodFill(layer, new Point<int>(4, 1), '-');
            LayerFloodFill(layer, new Point<int>(5, 8), '-');
            //LayerFloodFill(layer, new Point<int>(34, 8), '-');
            //LayerFloodFill(layer, new Point<int>(layer.width-2, layer.height-1), '-');

            Console.WriteLine(layer);

        }
    }
}
