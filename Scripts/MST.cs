using Godot;
using System;
using System.Collections.Generic;

//https://en.wikipedia.org/wiki/Kruskal%27s_algorithm

public class MST {
    public HashSet<PositionalEdge> MakeMST(IEnumerable<IPositional> nodes) {
        var ds = new DisjointSet<IPositional>();
        var heap = new BinaryHeap<PositionalEdge>(100, new EdgeComparer());
        var vertices = new List<IPositional>(nodes);
        var connections = new HashSet<PositionalEdge>();
        // Create all possible edges.
        for (int i = 0; i < vertices.Count; ++i) {
            for (int j = i + 1; j < vertices.Count; ++j) {
                heap.Insert(new PositionalEdge(vertices[i], vertices[j]));
            }
        }
        // Begin Kruskals.
        foreach (IPositional p in nodes) {
            ds.MakeSet(p);
        }
        while (!heap.IsEmpty()) {
            var edge = heap.Pop();
            if (!ds.Find(edge.a).Equals(ds.Find(edge.b))) {
                connections.Add(edge);
                ds.Union(edge.a, edge.b);
            }
        }
        return connections;
    }
}

public class Edge<T> {
    public T a;
    public T b;
    public Edge(T a, T b) {
        this.a = a;
        this.b = b;
    }
    public override bool Equals(object obj){
        Edge<T> other = (Edge<T>)obj;
        return (a.Equals(other.a) && b.Equals(other.b))
            || (a.Equals(other.b) && b.Equals(other.a));
    }

    public override int GetHashCode() {
        return a.GetHashCode() + b.GetHashCode();
    }
}

public class PositionalEdge : Edge<IPositional> {
    public float Length {private set; get; }
    public PositionalEdge(IPositional a, IPositional b) : base(a, b) {
        Length = (b.GetPos() - a.GetPos()).Length();
    }
}

public class EdgeComparer : IComparer<PositionalEdge> {
    public int Compare(PositionalEdge x, PositionalEdge y) {
        if (x.Length < y.Length) return -1;
        else if (x.Length == y.Length) return 0;
        else return 1;
    }
}

public interface IPositional {
    Vector2 GetPos();
    void Connect(IPositional p);
}
