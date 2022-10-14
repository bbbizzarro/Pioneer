using Godot;
using System;

public class PlayerController : KinematicBody2D {
    [Export] float _speed;
    SpriteGroup _spriteGroup;
    Vector2 _velocity;
    bool _isInputPaused;
    public Inventory Inventory {private set; get;}
    Sensor _Sensor;

    public override void _Ready() {
        _spriteGroup = (SpriteGroup)GetNode("SpriteGroup");
        _Sensor = (Sensor)GetNode("Sensor");
        _Sensor.ItemPickupEvent += HandleItemPickup;
        Inventory = new Inventory(8);
    }

    public override void _Process(float delta) {
        ParseInput();
        HandleAnimations();
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
        _spriteGroup.FaceTarget(GetGlobalMousePosition());
    }

    private void HandleMovement(Vector2 direction) {
        float modifier = (_spriteGroup.IsAnimatingAttack()) ? 0.3f : 1f;
        _velocity = modifier * _speed * Globals.PixelsPerUnit * direction;
        MoveAndSlide(_velocity);
    }

    private void HandleAnimations() {
        if (_velocity.Length() > 0) 
            _spriteGroup.AnimateRun();
        else 
            _spriteGroup.AnimateIdle();
    }

    private void HandleAttack() {
        _spriteGroup.AnimateAttack();
    }
}
