using Godot;
using System;
using System.Collections.Generic;

public class SpriteGroup : Node2D {
    AnimationPlayer _animationPlayer;
    bool flipped = false;
    Node2D _sprites;

    public override void _Ready() {
        _animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _sprites = (Node2D)GetNode("Sprites");
    }

    public void FlipH(bool flip) {
        if (flip != flipped) {
            flipped = flip;
            _sprites.Scale = new Vector2(-1 * _sprites.Scale.x, _sprites.Scale.y);
        }
    }

    public void AnimateRun() {
        _animationPlayer.Play("Run");
    }
    public void AnimateIdle() {
        _animationPlayer.Play("Idle");
    }
}
