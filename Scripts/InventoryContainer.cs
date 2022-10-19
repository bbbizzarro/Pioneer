using Godot;
using System;

public class InventoryContainer : NinePatchRect {


    public override void _Process(float delta) {
        HandleInput();
    }

    private void HandleInput() {
        if (Input.IsActionJustPressed("Inventory")) {
            Visible = !Visible;
        }
    }
}
