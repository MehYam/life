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
            //ArrayTest();
            RunMapTest();

            Console.WriteLine("done");
            Console.ReadKey(true);
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

        struct Tile
        {
            public readonly int layer;
            public readonly int row;
            public readonly int col;
            public readonly int type;
            public Tile(int layer, int row, int col, int type)
            {
                this.layer = layer;
                this.row = row;
                this.col = col;
                this.type = type;
            }
            public override string ToString()
            {
                return string.Format("L:{0}, ({1},{2}) T:{3:00}", layer, row, col, type);
            }
        }
        static void RunMapTest()
        {
            // populate a map with layers
            var map = new Map<int>(2, 3, 4);
            var random = new Random();

            map.ForEach((int layer, int row, int col, int tile) =>
            {
                //map.
            });

            // actors can occupy the same tile - they have pixel X, Y coordinates.  Tiles have a size, so define a mapping from pixels to tile R,C
        }
    }
}
