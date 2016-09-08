using System;
using System.Collections.Generic;
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
            PathfindTest();
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
            var b = new Point<int>(1, 1);

            Console.WriteLine("IEquatable (should be true): " + (new Point<int>(1, 1) == b));
            Console.WriteLine("IEquatable (should be false): " + (a == b));
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
            var noise = new Perlin();

            int iLayer = 0;
            map2.ForEachLayer((Layer<Tile> layer) =>
            {
                layer.Fill((int x, int y, Tile oldTile) =>
                {
                    double perlin = noise.perlin(x + 0.5, y + 0.5, iLayer * 0.1);

                    char tile = Tile.types[(int)(perlin * Tile.types.Length)];
                    return new Tile(iLayer, x, y, tile);
                });
                ++iLayer;
            });
            Console.WriteLine(map2);
        }
        static void PathfindTest()
        {
            var map = new Map<Tile>(1, 50, 20);

            var noise = new Perlin();
            map.Get(0).Fill((int x, int y, Tile oldTile) =>
            {
                double perlin = noise.perlin(x + 0.5, y + 0.5, 0.1);

                char tile = Tile.types[(int)(perlin * Tile.types.Length)];
                return new Tile(0, x, y, tile);
            });
            Console.WriteLine(map);

            var search = new MapAStar<Tile>();
            //search.PathFind(map.Get(0), (Tile t) => t.type == ' ' || t.type == '.', new Point<int>(2, 0), new Point<int>(8, 19));
            search.PathFind(map.Get(0), (Tile t) => t.type == ' ' || t.type == '.', new Point<int>(2, 0), new Point<int>(48, 19));

            var solutionRender = new Layer<Tile>(map.width, map.height);
            if (search.solution != null)
            {
                map.Get(0).Set(search.solution.Value.position, new Tile('S'));

                var cost = search.solution.Value.cost;
                int step = 0;
                do
                {
                    var pos = search.ToPosition(cost.parentIndex);
                    cost = search._closedList[pos];
                    map.Get(0).Set(pos, new Tile((step++ % 10).ToString()[0]));
                }
                while (cost.parentIndex >= 0);

                Console.WriteLine(map);
            }
            else
            {
                Console.WriteLine("No solution?");
            }
            // this is the closed list - see the article
            foreach (var loc in search._closedList)
            {
                solutionRender.Set(loc.Key, new Tile('*'));
            }
            Console.WriteLine(solutionRender);
        }
    }
}
