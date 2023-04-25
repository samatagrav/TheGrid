using System.Collections.ObjectModel;

namespace Grid
{
    public abstract class BasePathFinding
    {
        public abstract Collection<Node> FindPath();
        public abstract void PreMark();
        public abstract void PostMark();

    }
}