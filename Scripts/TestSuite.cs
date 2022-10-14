using Godot;
using System;

public class TestSuite : Node2D {
    public override void _Ready() {
        var inventoryTests = new InventoryTests();
        inventoryTests.Test();
    }
}
