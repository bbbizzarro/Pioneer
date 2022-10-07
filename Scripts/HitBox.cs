using Godot;
using System;


public delegate void HitEventHandler(float value);
public class HitBox : Area2D {
    public event HitEventHandler HitEvent;

    public override void _Ready() {
    }

    public void HandleHit(float value) {
        HitEvent?.Invoke(value);
    }
}
