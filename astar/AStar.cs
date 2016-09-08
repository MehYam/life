using System;
using System.Collections.Generic;

namespace life.astar
{ 
    /// <summary>
    /// From http://xfleury.github.io/graphsearch.html
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class AStar<TKey, TValue> where TValue : IComparable<TValue>
    {
        protected void Graph(Node start, PriorityQueue<Node> openList, Dictionary<TKey, TValue> closedList)
        {
            openList.Insert(start);
            while (openList.Count > 0)
            {
                Node node = openList.RemoveRoot();
                if (closedList.ContainsKey(node.position)) continue;
                closedList.Add(node.position, node.cost);
                if (CheckDestination(node.position)) return;
                AddNeighbours(node, openList);
            }
        }
        protected abstract void AddNeighbours(Node node, PriorityQueue<Node> openList);
        protected abstract bool CheckDestination(TKey position);
        public struct Node : IComparable<Node>
        {
            public TKey position;
            public TValue cost;
            public Node(TKey position, TValue cost)
            {
                this.position = position;
                this.cost = cost;
            }
            public int CompareTo(Node other) { return cost.CompareTo(other.cost); }
        }
    }
}
