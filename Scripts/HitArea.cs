using Godot;
using System;

public class HitArea : Area2D {

    Timer _timer;

    public override void _Ready() {
        Connect("area_entered", this, nameof(HandleHit));
        _timer = (Timer)GetNode("Timer");
    }

    public void HandleHit(Area2D area) {
        if (area.IsInGroup("Targetable")) {
            ((HitBox)area).HandleHit(10);
        }
    }
}
