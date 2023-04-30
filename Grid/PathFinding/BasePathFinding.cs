using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Grid
{
    public abstract class BasePathFinding
    {
        protected Node _startNode;
        protected Node _endNode;

        public BasePathFinding(Node startNode, Node endNode)
        {
            _startNode = startNode;
            _endNode = endNode;
        }

        public abstract List<Node> FindPath();
        public abstract void VisitedMark();
        public abstract void CurrentMark();

    }
}