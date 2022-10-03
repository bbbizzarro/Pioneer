using Godot;
using System;
using System.Collections.Generic;

public class SpriteGroup : Node2D {
    [Export] string GroupName = "Flippable";
    AnimationPlayer _animationPlayer;
    bool flipped = false;
    List<Node2D> _sprites = new List<Node2D>();
    public Held Held {private set; get; }

    public override void _Ready() {
        _animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        Held = (Held)GetNode("Held");
        GetSpriteReferences();
    }

    private void GetSpriteReferences() {
        foreach (Node sprite in GetChildren()) {
            if (sprite.IsInGroup(GroupName)) {
                _sprites.Add((Node2D)sprite);
            }
        }
    }

    public void FlipH(bool flip) {
        if (flip != flipped) {
            flipped = flip;
            foreach (Node2D sprite in _sprites) 
                sprite.Scale = new Vector2(-1 * sprite.Scale.x, sprite.Scale.y);
        }
    }

    public void FaceTarget(Vector2 target) {
        var dir = GetDirectionToward(target);
        if (dir.x < 0) FlipH(false);
        else FlipH(true);
        PositionHeld(target);
    }

    private Vector2 GetDirectionToward(Vector2 target) {
        return (target - GlobalPosition).Normalized();
    }

    public void AnimateAttack() {
        Held.PlayAttackAnimation();
    }

    public bool IsAnimatingAttack() {
        return Held.IsAnimatingAttack();
    }

    public void AnimateRun() {
        _animationPlayer.Play("Run");
    }
    public void AnimateIdle() {
        _animationPlayer.Play("Idle");
    }
    private void PositionHeld(Vector2 target) {
        Held.RotateTowardTarget(GlobalPosition, GetGlobalMousePosition());
    }
}
