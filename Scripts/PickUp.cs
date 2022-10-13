using Godot;
using System;

public class PickUp : KinematicBody2D {
    [Export] float Drag = 0.9f;
    [Export] float FlyTime = 1f;
    [Export] float GravityStrength = 30f;
    [Export] float InitSpeed = 1f;
    [Export] float MoveSpeed = 1f;
    [Export] float PickUpDistance = 0.1f;
    RandomNumberGenerator _rng = new RandomNumberGenerator();
    Sensor _target;

    Physics2D physics2D;
    Sprite _sprite;
    float _yLimit;
    float _currMoveSpeed = 1f;

    public override void _Ready() {
        _rng.Randomize();
        _sprite = (Sprite)GetNode("Sprite");
        physics2D = new Physics2D(GravityStrength * Globals.PixelsPerUnit, Drag);
        physics2D.Set(GetYLimit(), GetInitVelocity(), GlobalPosition, FlyTime);
        //((Area2D)GetNode("Area2D")).Connect("area_entered", this, nameof(HandleTargeting));
        CallDeferred(nameof(InitializeArea2D));
        _yLimit = GetYLimit();
    }

    private void InitializeArea2D() {
        ((Area2D)GetNode("Area2D")).Connect("area_entered", this, nameof(HandleTargeting));
    }

    private Vector2 GetInitVelocity() {
        return InitSpeed * Globals.PixelsPerUnit 
            * (new Vector2(_rng.RandfRange(-0.5f, 0.5f),-1)).Normalized();
    }

    public void HandleTargeting(Area2D newTarget) {
        if (_target == null && newTarget != null) {
            _target = (Sensor)newTarget;
        }
    }


    private float GetYLimit() {
        return GlobalPosition.y + Globals.PixelsPerUnit/2f;
    }

    public override void _Process(float delta) {
        if (physics2D.IsRunning()) {
            Vector2 newPosition = physics2D.Update(delta);
            _sprite.GlobalPosition = newPosition;
            GlobalPosition = new Vector2(newPosition.x, _yLimit);
        }
        else {
            MoveTowardTarget(delta);
        }
    }

    private void HandlePickup() {
    }

    private void MoveTowardTarget(float delta) {
        if (_target != null) {
            Vector2 diff = (_target.GlobalPosition - GlobalPosition);
            float newScale = Mathf.Min(1f, diff.Length() / Globals.PixelsPerUnit);
            MoveAndSlide(MoveSpeed * Globals.PixelsPerUnit * diff.Normalized());
            _sprite.Scale = new Vector2(newScale, newScale);
            if (diff.Length() < PickUpDistance * Globals.PixelsPerUnit) {
                _target.HandleItemPickup("Wood", 1);
                QueueFree();
            }
        }
    }

}
