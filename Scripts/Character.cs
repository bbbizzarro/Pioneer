using Godot;
using System;

public class Character : KinematicBody2D {

    protected SpriteGroup _spriteGroup;
    Vector2 _velocity;
    [Export] float _speed;
    bool _attackQueued;
    Vector2 _queuedDirection;
    Impulse _impulse;

    public override void _Ready() {
        base._Ready();
        _spriteGroup = (SpriteGroup)GetNode("SpriteGroup");
        _impulse = new Impulse(50f);
    }

    public override void _Process(float delta) {
        base._Process(delta);
        HandleAnimations();
        _impulse.Update(delta);
    }

    protected void HandleMovement(Vector2 direction) {
        float modifier = (_spriteGroup.IsAnimatingAttack()) ? 0.3f : 1f;
        //!//
        modifier = 1f;
        _velocity = modifier * _speed * Globals.PixelsPerUnit * direction;
        MoveAndSlide(_velocity);
        //if (_impulse.Velocity.Length() > 0.1f) {
        //    MoveAndSlide(_impulse.Velocity);
        //}
        //else {
        //    MoveAndSlide(_velocity);
        //}
    }

    private void HandleAnimations() {
        if (_velocity.Length() > 0) 
            _spriteGroup.AnimateRun();
        else 
            _spriteGroup.AnimateIdle();
        if (_attackQueued && !_spriteGroup.IsAnimatingAttack()) {
            HandleAttack(_queuedDirection);
            _attackQueued = false;
            //_queuedDirection = Vector2.Zero;
        }
    }

    protected void HandleAttack(Vector2 target) {
        Vector2 direction = (target - GlobalPosition).Normalized();
        if (_spriteGroup.IsAnimatingAttack()) {
            _attackQueued = true;
            _queuedDirection = direction;
            return;
        }
        _spriteGroup.AnimateAttack();
        _impulse.Set(5f, direction);
    }

    protected void FaceToward(Vector2 target) {
        _spriteGroup.FaceTarget(target);
    }

    protected void PositionHeld(Vector2 target) {
        _spriteGroup.PositionHeld(target);
    }
}

public class Impulse {
    float _drag;
    float _amount;
    Vector2 _direction;
    public Vector2 Velocity {private set; get;}
    float timer;

    public Impulse(float drag) {
        _drag = drag;
    }

    public void Set(float amount, Vector2 direction) {
        _amount = amount;
        _direction = direction.Normalized();
        timer = 0.5f;
    }

    public void Update(float delta) {
        Velocity = _amount * Globals.PixelsPerUnit * _direction;
        _amount = Mathf.Max(_amount - _drag * delta, 0);
    }
}