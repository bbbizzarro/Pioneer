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
        _spriteGroup.FaceTarget(GetGlobalMousePosition());
    }


    private void ParseInput() {
        HandleMovement(Input.GetVector("Left", "Right", "Up", "Down"));
        if (Input.IsActionPressed("Attack")) HandleAttack();
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
