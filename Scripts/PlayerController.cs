using Godot;
using System;

public class PlayerController : KinematicBody2D {
    [Export] float _speed;
    SpriteGroup _spriteGroup;
    Vector2 _velocity;

    public override void _Ready() {
        _spriteGroup = (SpriteGroup)GetNode("SpriteGroup");
    }

    public override void _Process(float delta) {
        ParseInput();
        HandleAnimations();
        FaceTowardMouse();
    }


    private void ParseInput() {
        HandleMovement(Input.GetVector("Left", "Right", "Up", "Down"));
    }

    private void HandleMovement(Vector2 direction) {
        _velocity = _speed * Globals.PixelsPerUnit * direction;
        MoveAndSlide(_velocity);
    }

    private void HandleAnimations() {
        if (_velocity.Length() > 0) 
            _spriteGroup.AnimateRun();
        else 
            _spriteGroup.AnimateIdle();
    }

    private void FaceTowardMouse() {
        var dir = GetDirectionToward(GetGlobalMousePosition());
        if (dir.x < 0) _spriteGroup.FlipH(false);
        else _spriteGroup.FlipH(true);
    }

    private Vector2 GetDirectionToward(Vector2 target) {
        return (target - GlobalPosition).Normalized();
    }
}
