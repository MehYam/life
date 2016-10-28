using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lifeEngine
{
    public static class Operations
    {
        public static Layer<Tile> LoadLayerFile(string path)
        {
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                var lines = contents.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                var validLines = 0;
                foreach (var line in lines)
                {
                    if (line.Length == lines[0].Length)
                    {
                        ++validLines;
                    }
                }
                var retval = new Layer<Tile>(lines[0].Length, validLines);

                for (int y = 0; y < lines.Length; ++y)
                {
                    var line = lines[y];
                    if (line.Length == lines[0].Length)
                    {
                        for (int x = 0; x < line.Length; ++x)
                        {
                            retval.Set(new Point<int>(x, y), new Tile(line[x]));
                        }
                    }
                }
                return retval;
            }
            return null;
        }
        /// <summary>
        /// Creates a stencil of the provided layer
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="emptyTile">the int to represent an empty tile</param>
        /// <param name="nonEmptyTile">the int to represent a non-empty tile</param>
        /// <returns></returns>
        public static Layer<int> CreateLayerMask(Layer<Tile> layer, int emptyTile = 0, int nonEmptyTile = -1)
        {
            var mask = new Layer<int>(layer.size.x, layer.size.y);
            mask.ForEach((x, y, tile) =>
            {
                mask.Set(x, y, layer.Get(x, y).IsEmpty ? emptyTile : nonEmptyTile);
            });
            return mask;
        }
    }
}
