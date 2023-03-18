using Godot;
using System;

public class Speaker : Interactable {

    public override void _Ready() {
        base._Ready();
    }

    public override void Interact() {
        GD.Print("Hello, stranger.");
    }
}
