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
                var retval = new Layer<Tile>(lines[0].Length, lines.Length);

                for (int y = 0; y < lines.Length; ++y)
                {
                    var line = lines[y];
                    var extent = Math.Min(line.Length, retval.size.x);
                    for (int x = 0; x < line.Length; ++x)
                    {
                        retval.Set(new Point<int>(x, y), new Tile(line[x]));
                    }
                }
                return retval;
            }
            return null;
        }
    }
}
