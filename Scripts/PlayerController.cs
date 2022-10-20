using Godot;
using System;

public class PlayerController : Character {
    bool _isInputPaused;
    public Inventory Inventory {private set; get;}
    Sensor _Sensor;

    public override void _Ready() {
        base._Ready();
        _Sensor = (Sensor)GetNode("Sensor");
        _Sensor.ItemPickupEvent += HandleItemPickup;
        Inventory = new Inventory(8);
    }

    public override void _Process(float delta) {
        base._Process(delta);
        ParseInput();
    }

    public void HandleItemPickup(string id, int count) {
        Inventory.Add(id, count);
        Inventory.Update();
    }

    private void ParseInput() {
        if (Input.IsActionJustPressed("Inventory")) _isInputPaused = !_isInputPaused;
        if (_isInputPaused) {
            HandleMovement(Vector2.Zero);
            return;
        }
        HandleMovement(Input.GetVector("Left", "Right", "Up", "Down"));
        if (Input.IsActionPressed("Attack")) HandleAttack();
        FaceToward(GetGlobalMousePosition());
        PositionHeld(GetGlobalMousePosition());
    }

}
