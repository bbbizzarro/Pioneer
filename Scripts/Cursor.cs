using Godot;
using System;

public class Cursor : Node2D {
    public Location origin;
    public Location target;

    AnimationPlayer animationPlayer;

    public override void _Ready() {
        animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        animationPlayer.Play("CursorIdle");
        Hide();
    }

    public void SetCursor(Location location) {
        GlobalPosition = location.GlobalPosition;
        Show();
    }

    public void UnsetCursor(Location location) {
        Hide();
    }
}
