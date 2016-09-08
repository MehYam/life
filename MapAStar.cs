using System;
using System.Collections.Generic;

using life.astar;

namespace life
{
    class MapAStar<TTile> : AStar<Point<int>, Cost>
    {
        const int baseOrthogonalCost = 5;
        const int baseDiagonalCost = 7;

        // "temp" vars
        //KAI: I hate this fucking paradigm.  Get this working and then fix this?
        Layer<TTile> _layer;
        Func<TTile, bool> _isTilePassable;
        Point<int> _end;
        public Node? solution;
        public Dictionary<Point<int>, Cost> _closedList = new Dictionary<Point<int>, Cost>();

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
        }

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

        public int ToIndex(Point<int> position) { return position.y * _layer.width + position.x; }
        public Point<int> ToPosition(int index) { return new Point<int>(index % _layer.width, index / _layer.width); }

        static int GetDistance(Point<int> src, Point<int> dest)
        {
            int dx = Math.Abs(dest.x - src.x);
            int dy = Math.Abs(dest.y - src.y);
            int diagonal = Math.Min(dx, dy);
            int orthogonal = dx + dy - 2 * diagonal;

            return diagonal * baseDiagonalCost + orthogonal * baseOrthogonalCost;
        }
    }
}
