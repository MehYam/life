using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    sealed class Layer<T>
    {
        readonly T[,] tiles;

        public Layer(int width, int height)
        {
            tiles = new T[width, height];
        }
        public T Get(int r, int c)
        {
            return tiles[r, c];
        }
        public void Set(int r, int c, T t)
        {
            tiles[r, c] = t;
        }

        /// <summary>
        ///  Iterate the layer, invoking the callback with (row, col, item) arguments
        /// </summary>
        /// <param name="callback"></param>
        public void ForEach(Action<int, int, T> callback)
        {
            var rows = tiles.GetLength(0);
            var cols = tiles.GetLength(1);
            for (var r = 0; r < rows; ++r)
            {
                for (var c = 0; c < cols; ++c)
                {
                    callback(r, c, tiles[r, c]);
                }
            }
        }
        public void Fill(Func<int, int, T, T> callback)
        {
            var rows = tiles.GetLength(0);
            var cols = tiles.GetLength(1);
            for (var r = 0; r < rows; ++r)
            {
                for (var c = 0; c < cols; ++c)
                {
                    tiles[r, c] = callback(r, c, tiles[r, c]);
                }
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            int thisRow = 0;
            ForEach((int r, int c, T t) => 
            {
                if (thisRow != r)
                {
                    thisRow = r;
                    sb.AppendLine();
                }
                sb.Append(t.ToString());
                sb.Append(' ');
            });
            return sb.ToString();
        }
    }
    sealed class Map<T>
    {
        readonly Layer<T>[] layers;

        public Map(int depth, int width, int height)
        {
            layers = new Layer<T>[depth];

            for (var d = 0; d < depth; ++d)
            {
                layers[d] = new Layer<T>(width, height);
            }
        }
        public Layer<T> Get(int i)
        {
            return layers[i];
        }
        /// <summary>
        /// Iterate all layers, invoking the callback with (layer, row, col, item) arguments
        /// </summary>
        /// <param name="callback"></param>
        public void ForEach(Action<int, int, int, T> callback)
        {
            int iLayer = 0;
            foreach (var layer in layers)
            {
                layer.ForEach((int r, int c, T t) => callback(iLayer, r, c, t));

                iLayer++;
            }
        }
        public void ForEachLayer(Action<Layer<T>> callback)
        {
            foreach (var layer in layers)
            {
                callback(layer);
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var layer in layers)
            {
                sb.AppendLine(layer.ToString());
                sb.AppendLine("--------------------------------");
            }
            return sb.ToString();
        }
    }
}
