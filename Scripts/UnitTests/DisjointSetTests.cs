using Godot;
using System;

public class DisjointSetTests : Node2D {
    public override void _Ready() {
        var ds = new DisjointSet<int>();
        for (int i =0; i < 10; ++i) {
            ds.MakeSet(i);
        }
        for (int i =0; i < 10; ++i) {
            GD.Print(ds.Find(i));
        }
    }
}
