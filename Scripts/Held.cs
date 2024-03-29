using Godot;
using System;

public class Held : Node2D {
    [Export] Vector2 Origin = new Vector2(0, -10);
    [Export] float Radius = 20;

    [Export] float RotationOffset = 0;
    AnimationPlayer _animationPlayer;
    Sprite _sprite;
    bool _isRotatable = true;

    public override void _Ready() {
        _sprite = (Sprite)GetNode("Sprite");
        _animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _animationPlayer.Play("HeldIdle");
    }

    public void RotateTowardTarget(Vector2 source, Vector2 target) {
        var v = target - source + Origin;
        float rotationOffset = ((v.x >= 0) ? 1 : -1) * RotationOffset;
        Position = Radius * v.Normalized().Rotated(rotationOffset) + Origin;
        if (_isRotatable) {
            _sprite.Rotation = Mathf.Atan2(v.y, v.x) + Mathf.Pi + rotationOffset;
            _sprite.FlipV = v.x >= 0;
        }
        else  {
            _sprite.Rotation = 0;
            _sprite.FlipV = false;
        }
    }

    public void PlayAttackAnimation() {
        _animationPlayer.Play("HeldAttack");
    }

    public bool IsAnimatingAttack() {
        return _animationPlayer.IsPlaying();
    }

    private Vector2 GetDirectionToward(Vector2 target) {
        return (target - GlobalPosition).Normalized();
    }

    public void SetHeld(string itemID) {
        _sprite.Texture = SpriteDB.Instance.GetSprite(itemID);
        var item = ItemDB.Instance.GetItem(itemID);
        _isRotatable = item.Type == ItemType.Tool;
    }

    public void ClearHeld() {
        _sprite.Texture = null;
    }
}
