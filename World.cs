﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace life
{
    sealed class World
    {
        public const int PIXELS_PER_TILE = 100;
        public static Point<int> PixelsToTile(Point<float> pixels)
        {
            return new Point<int>((int)pixels.x / PIXELS_PER_TILE, (int)pixels.y / PIXELS_PER_TILE);
        }
        public static Point<int> PixelsToTileRemainder(Point<float> pixels)
        {
            return new Point<int>((int)pixels.x % PIXELS_PER_TILE, (int)pixels.y % PIXELS_PER_TILE);
        }
        /// <summary>
        /// Returns the pixel position at the center of 'tile'
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static Point<float> TileToPixels(Point<int> tile)
        {
            return new Point<float>(
                tile.x * PIXELS_PER_TILE + PIXELS_PER_TILE / 2,
                tile.y * PIXELS_PER_TILE + PIXELS_PER_TILE / 2
            );
        }
        public Layer<Tile> map { get; set; }  //KAI: encaps.
        public Layer<Tile> items { get; set; }
        public World(int width, int height)
        {
            map = new Layer<Tile>(width, height);
            items = new Layer<Tile>(width, height);
        }
        const long DATETIME_TICKS_PER_SEC = 10 * 1000 * 1000;
        long _lastTick;
        public void Tick()
        {
            var now = DateTime.Now.Ticks;
            if (_lastTick > 0)
            {
                var delta = now - _lastTick;

                float fDelta = ((float)delta) / DATETIME_TICKS_PER_SEC;
                float fNow = ((float)now) / DATETIME_TICKS_PER_SEC;

                //Console.WriteLine(string.Format("{0:0.00000}", fDelta));

                // loop through actors, giving them their time slice
                foreach (var actor in _actors)
                {
                    actor.FixedUpdate(fNow, fDelta);
                }
            }
            _lastTick = now;
        }
        List<Actor> _actors = new List<Actor>();
        public void AddActor(Actor a)
        {
            if (!_actors.Contains(a))
            {
                _actors.Add(a);
            }
        }

        bool running;
        /// <summary>
        /// Starts the world's game loop
        /// </summary>
        /// <param name="fps">The number of physics/world frames to run per second</param>
        public void StartSimulation(int fps)
        {
            var millis = 1000 / fps;

            running = true;
            while (running)
            {
                Tick();
                System.Threading.Thread.Sleep(millis);
            }
        }
        public void StopSimulation()
        {
            running = false;
        }
    }
    struct Tile
    {
        public static char[] types = { 'O', 'o', '.', ' ', '#' };
        public static bool IsPassable(char type) { return type == ' ' || type == '.'; }

        public readonly char type;
        public Tile(char type)
        {
            this.type = type;
        }
        public override string ToString()
        {
            return type.ToString();
        }
    }
}
