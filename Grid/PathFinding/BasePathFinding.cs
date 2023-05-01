﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Threading;

namespace Grid
{
    public abstract class BasePathFinding
    {
        protected const int _pseudoInfinity = Int32.MaxValue;//On the scale it fits the infinity
        protected Node _startNode;
        protected Node _endNode;

        public BasePathFinding(Node startNode, Node endNode)
        {
            _startNode = startNode;
            _endNode = endNode;
        }

        public abstract List<Node> FindPath();
        public MarkDelegate VisitedMark;
        public MarkDelegate CurrentMark;

        public delegate void MarkDelegate(Node node);
    }
}