using Godot;
using System;

public class HitArea : Area2D {

    public override void _Ready() {
        Connect("area_entered", this, nameof(HandleHit));
    }

    public void HandleHit(Area2D area) {
        if (area.IsInGroup("Targetable")) {
            ((HitBox)area).HandleHit(10);
        }
    }
}
