using System;
using System.Collections.Generic;

using lifeEngine.astar;

namespace lifeEngine
{
    //KAI: This class is JANKY. I'll replace it with something cleaner layer.

    // Assumption: the author was trying to reduce the GC overhead from the creation of 1000's of objects, but
    // my instinct is that those objects were just Point objects.  Our Point<> (and Unity's Vector) are structs,
    // so logically you could create big immovable arrays of them to store results and index things around to
    // acheive the same memory re-use... *if* you needed to.  This should really be replaced with a better
    // implementation, but I need to get rolling.
    public class MapAStar<TTile> : AStar<Point<int>, Cost>
    {
        const int baseOrthogonalCost = 5;
        const int baseDiagonalCost = 7;

        protected override void AddNeighbours(Node node, PriorityQueue<Node> openList)
        {
            int parentIndex = ToIndex(node.position);
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (!(x == 0 && y == 0))
                    {
                        Point<int> newPos = new Point<int>(node.position.x + x, node.position.y + y);
                        if (newPos.x >= 0 && newPos.x < _layer.width && newPos.y >= 0 && newPos.y < _layer.height)
                        {
                            if (_isTilePassable(_layer.Get(newPos.x, newPos.y)))
                            {
                                int distanceCost = node.cost.distanceTravelled +
                                    ((x == 0 || y == 0) ? baseOrthogonalCost : baseDiagonalCost);
                                openList.Insert(new Node(newPos,
                                    new Cost(parentIndex, distanceCost, distanceCost +
                                        GetDistance(newPos, _end))));
                            }
                        }
                    }
                }
            }
        }

        protected override bool CheckDestination(Point<int> position)
        {
            if (position == _end)
            {
                solution = new Node(position, _closedList[position]);
                return true;
            }
            return false;
        }

        int ToIndex(Point<int> position) { return position.y * _layer.width + position.x; }
        Point<int> ToPosition(int index) { return new Point<int>(index % _layer.width, index / _layer.width); }

        static int GetDistance(Point<int> src, Point<int> dest)
        {
            int dx = Math.Abs(dest.x - src.x);
            int dy = Math.Abs(dest.y - src.y);
            int diagonal = Math.Min(dx, dy);
            int orthogonal = dx + dy - 2 * diagonal;

            return diagonal * baseDiagonalCost + orthogonal * baseOrthogonalCost;
        }

        // This is an optimized implementation, which makes it ugly;  below we'll hide the author's intended interface
        // under one that's easier to use
        Layer<TTile> _layer;
        Func<TTile, bool> _isTilePassable;
        Point<int> _end;
        Node? solution;
        public Dictionary<Point<int>, Cost> _closedList = new Dictionary<Point<int>, Cost>();  // exposing an implementation detail, but good for debugging

        // public interface
        public List<Point<int>> result { private set; get; }
        public MapAStar()
        {
            this.result = new List<Point<int>>();
        }
        public void PathFind(Layer<TTile> layer, Func<TTile, bool> isTilePassable, Point<int> start, Point<int> end)
        {
            // Apparently this traverses in reverse order...
            _end = start;
            start = end;

            _layer = layer;
            _isTilePassable = isTilePassable;

            // these could be cached between calls to PathFind as an optimization;  I find this dubious
            var openList = new PriorityQueue<AStar<Point<int>, Cost>.Node>();
            //var closedList = new Dictionary<Point<int>, Cost>();

            Graph(new Node(start, new Cost(-1, 0, GetDistance(start, end))), openList, _closedList);

            // now complete the un-janking, by filling in the array
            if (solution != null)
            {
                result.Add(solution.Value.position);

                var cost = solution.Value.cost;
                do
                {
                    var pos = ToPosition(cost.parentIndex);
                    result.Add(pos);

                    cost = _closedList[pos];
                }
                while (cost.parentIndex >= 0);
            }
        }
    }
}
