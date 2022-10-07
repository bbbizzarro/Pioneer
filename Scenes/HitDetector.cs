using Godot;
using System;

public class HitDetector : Area2D {

    public override void _Ready() {
        Connect("area_entered", this, nameof(OnHit));
    }

    public void OnHit(Area2D area) {
        if (area.IsInGroup("Hit")) {
        }
    }

    public void ReceiveHit() {
    }
}
