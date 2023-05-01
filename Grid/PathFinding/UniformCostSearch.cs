using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grid.PathFinding
{
    class UniformCostSearch:BasePathFinding
    {
        private NodeHandler _nodeHandler;
        public UniformCostSearch(Node startNode, Node endNode, NodeHandler nodeHandler) : base(startNode, endNode)
        {
            this._nodeHandler = nodeHandler;
        }

        public override List<Node> FindPath()
        {
            Node node = _startNode;
            List<Node> orderedOpenList = new List<Node>();
            List<Node> expanded = new List<Node>();
            orderedOpenList.Add(_startNode);

            
            while (orderedOpenList.Count == 0)
            {
                node = orderedOpenList.First(); 
                orderedOpenList.Remove(node);
                if (node.TopLeft.Equals(_endNode.TopLeft))
                {
                    return new List<Node>();
                } 
                expanded.Add(node); 
                List<Node> neighbors = _nodeHandler.GetNeighbors(node); 
                neighbors.ForEach(neighbor =>
                {
                    if (!expanded.Contains(neighbor) && !orderedOpenList.Contains(neighbor))
                    {
                        orderedOpenList.Add(neighbor);
                    }
                });
            }
            return null;
        }
    }
}
