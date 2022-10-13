using Godot;
using System;

public delegate void ItemEventHandler(string id, int count);
public class Sensor : Area2D {
    public event ItemEventHandler ItemPickupEvent;

    public override void _Ready() {
    }

    public void HandleItemPickup(string id, int count) {
        ItemPickupEvent?.Invoke(id, count);
    }
}
