using Godot;
using System;

public class Cursor : Node2D {
    public Location origin;
    public Location target;

    public override void _Ready() {
    }

    public override void _Process(float delta) {
        if (origin != null && target != null) {
            DeterminePosition();
		}
    }

    private void DeterminePosition() { 
        var pos = GetGlobalMousePosition() - origin.GlobalPosition;
        var dir = (target.GlobalPosition - origin.GlobalPosition).Normalized();
        var proj = (pos.x * dir.x + pos.y * dir.y) * dir;
        var worldCoord = proj + origin.GlobalPosition;
        GlobalPosition = worldCoord;
	}
}
