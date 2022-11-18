using Godot;
using System;

public class ExitPoint : Area2D {
    public Region target;

    public override void _Ready() {
        Connect("body_entered", this, nameof(HandlePlayerEnter));
    }

    public void HandlePlayerEnter(Node node) {
        if (node.IsInGroup("Player")) {
            GD.Print("Player has entered.");
        }
    }
}
