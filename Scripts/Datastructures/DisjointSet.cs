using Godot;
using System;
using System.Collections.Generic;

//https://en.wikipedia.org/wiki/Disjoint-set_data_structure

public class DisjointSet<T> {
    Dictionary<T, T> sets;

    public DisjointSet() {
        sets = new Dictionary<T, T>();
	}

    public void MakeSet(T value) { 
        if (!sets.ContainsKey(value)) {
            sets.Add(value, value);
		}
	}

    public T Find(T value) { 
        if (!sets.ContainsKey(value)) {
            return value;
        }
        if (!sets[value].Equals(value)) {
            sets[value] = Find(sets[value]);
            return sets[value];
		}
        else {
            return value;
		}
	}

    public void Union(T x, T y) {
        x = Find(x);
        y = Find(y);

        if (x.Equals(y)) return;

        sets[y] = x;
    }
}

