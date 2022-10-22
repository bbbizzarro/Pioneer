using Godot;
using System;

public class ItemDB : Node {
    public static ItemDB Instance;

    public override void _Ready() {
        if (Instance != null) QueueFree();
        else {
            Instance = this;
        }
    }
}
