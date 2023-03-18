using Godot;
using System;
using System.Collections.Generic;

public class DisjointSet<T> {
    HashSet<T> forest;
    HashSet<Vertex> sets;

    public DisjointSet() {
        forest = new HashSet<T>();
        sets = new HashSet<Vertex>();
	}

    public void MakeSet(T value) { 
        if (!forest.Contains(value)) {
            sets.Add(new Vertex(value));
		}
	}

    public Vertex Find(Vertex vertex) { 
        if (vertex.Parent != vertex) {
            vertex.Parent = Find(vertex.Parent);
            return vertex.Parent;
		}
        else {
            return vertex;
		}
	}

    public class Vertex {
        public Vertex Parent;
        public T Value;

        public Vertex(T value) {
            Parent = this;  Value = value;
		}

        public Vertex(Vertex parent, T value) {
            Parent = parent; Value = value;
		}
	}
}

