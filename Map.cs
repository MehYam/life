using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lifeEngine
{
    sealed class Layer<T>
    {
        readonly T[,] tiles;

        public readonly int width;
        public readonly int height;

        public Layer(int width, int height)
        {
            tiles = new T[width, height];
            this.width = width;
            this.height = height;
        }
        public Layer(Layer<T> rhs)
        {
            tiles = new T[rhs.width, rhs.height];
            this.width = rhs.width;
            this.height = rhs.height;

            ForEach((x, y, tile) =>
            {
                //KAI: latent bug - this is a shallow copy, which happens to work now because T is always a value type (i.e. struct Tile)
                tiles[x, y] = rhs.Get(x, y);
            });
        }
        public T Get(int x, int y)
        {
            return tiles[x, y];
        }
        public void Set(int x, int y, T t)
        {
            tiles[x, y] = t;
        }
        public T Get(Point<int> pos)
        {
            return tiles[pos.x, pos.y];
        }
        public void Set(Point<int> pos, T t)
        {
            tiles[pos.x, pos.y] = t;
        }
        /// <summary>
        ///  Iterate the layer, invoking the callback with (x, y, item) arguments
        /// </summary>
        /// <param name="callback"></param>
        public void ForEach(Action<int, int, T> callback)
        {
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    callback(x, y, tiles[x, y]);
                }
            }
        }
        public void Fill(Func<int, int, T, T> callback)
        {
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    tiles[x, y] = callback(x, y, tiles[x, y]);
                }
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            int thisY = -1;
            ForEach((int x, int y, T t) => 
            {
                if (thisY != y)
                {
                    thisY = y;
                    if (thisY > 0)
                    {
                        sb.AppendLine();
                    }
                    sb.Append((thisY % 10) + " ");
                }
                sb.Append(t.ToString());
            });
            return sb.ToString();
        }
    }
    sealed class Map<T>
    {
        public readonly int width;
        public readonly int height;

        readonly Layer<T>[] layers;

        public Map(int depth, int width, int height)
        {
            this.width = width;
            this.height = height;

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
        /// Iterate all layers, invoking the callback with (layer, x, y, item) arguments
        /// </summary>
        /// <param name="callback"></param>
        public void ForEach(Action<int, int, int, T> callback)
        {
            int iLayer = 0;
            foreach (var layer in layers)
            {
                layer.ForEach((int x, int y, T t) => callback(iLayer, x, y, t));

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
