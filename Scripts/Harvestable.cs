using Godot;
using System;

public class Harvestable : Node2D {
    HitBox _hitBox;
    Timer _timer;
    Sprite _sprite;
    ShaderMaterial _spriteMaterial;
    Health _health;

    public override void _Ready() {
        _hitBox = (HitBox)GetNode("HitBox");
        _hitBox.HitEvent += HandleHit;
        _sprite = (Sprite)GetNode("Sprite");
        _spriteMaterial = (ShaderMaterial)_sprite.Material;
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(ResetShaderMaterial));
        _health = new Health(30);
        _health.HealthZeroEvent += HandleHealthZero;
    }

    public void HandleHit(float value) {
        GD.Print(String.Format("Hit with value of {0}", value));
        _health.Increment(-value);
        _spriteMaterial.SetShaderParam("amount", 0.8f);
        _timer.Start(0.05f);
    }

    public void ResetShaderMaterial() {
        _spriteMaterial.SetShaderParam("amount", 0f);
    }

    public void HandleHealthZero() {
        QueueFree();
    }

}
