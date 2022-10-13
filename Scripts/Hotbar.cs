using Godot;
using System;

public class Hotbar : GridContainer {
    public override void _Ready() {
        base._Ready();
        Connect("mouse_entered", this, nameof(HandleMouseEnter));
        Connect("mouse_exited", this, nameof(HandleMouseExit));
    }

    private void HandleMouseEnter() {
    }

    private void HandleMouseExit() {
    }
}
