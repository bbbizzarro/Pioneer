using Godot;
using System;

public class Character : KinematicBody2D {

    protected SpriteGroup _spriteGroup;
    Vector2 _velocity;
    [Export] float _speed;

    public override void _Ready() {
        base._Ready();
        _spriteGroup = (SpriteGroup)GetNode("SpriteGroup");
    }

    public override void _Process(float delta) {
        base._Process(delta);
        HandleAnimations();
    }

    protected void HandleMovement(Vector2 direction) {
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

    protected void HandleAttack() {
        _spriteGroup.AnimateAttack();
    }

    protected void FaceToward(Vector2 target) {
        _spriteGroup.FaceTarget(target);
    }

    protected void PositionHeld(Vector2 target) {
        _spriteGroup.PositionHeld(target);
    }
}
