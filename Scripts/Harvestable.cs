using Godot;
using System;

public class Harvestable : StaticBody2D, IEntity {
    HitBox _hitBox;
    Timer _timer;
    Sprite _sprite;
    ShaderMaterial _spriteMaterial;
    Health _health;
    AnimationPlayer _animationPlayer;
    PackedScene _particleScene = (PackedScene)ResourceLoader.Load("res://Scenes/Particles.tscn");
    PackedScene _pickUpScene = (PackedScene)ResourceLoader.Load("res://Scenes/PickUp.tscn");
    event EntityEventHandler OnDestroyedEvent;
    event PositionEventHandler OnPositionChangedEvent; 

    public override void _Ready() {
        _hitBox = (HitBox)GetNode("HitBox");
        _hitBox.HitEvent += HandleHit;
        _sprite = (Sprite)GetNode("SpriteGroup/Sprite");
        _spriteMaterial = (ShaderMaterial)_sprite.Material;
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(ResetShaderMaterial));
        _health = new Health(50);
        _health.HealthZeroEvent += HandleHealthZero;
        _animationPlayer = (AnimationPlayer)GetNode("SpriteGroup/AnimationPlayer");
    }

    public void HandleHit(float value) {
        //GD.Print(String.Format("Hit with value of {0}", value));
        _health.Increment(-value);
        //_spriteMaterial.SetShaderParam("amount", 0.8f);
        //_timer.Start(0.05f);
        _animationPlayer.Play("Hit");
    }

    public void ResetShaderMaterial() {
        _spriteMaterial.SetShaderParam("amount", 0f);
    }

    public void HandleHealthZero() {
        LoadParticleEffect();
        for (int i = 0; i < 3; ++i) {
            LoadPickUp();
        }
        Destroy();
    }

    private void Destroy() {
        OnDestroyedEvent?.Invoke(this);
        QueueFree();
    }

    private void LoadParticleEffect() {
        Particles particles = (Particles)_particleScene.Instance();
        particles.Emitting = true;
        particles.GlobalPosition = GlobalPosition;
        GetParent().AddChild(particles);
    }

    private void LoadPickUp() {
        PickUp pickUp = (PickUp)_pickUpScene.Instance();
        pickUp.Initialize("Log");
        pickUp.GlobalPosition = GlobalPosition;
        CallDeferred(nameof(ParentChild), pickUp);
        //GetParent().AddChild(pickUp);
    }

    private void ParentChild(Node node) {
        GetParent().AddChild(node);
    }

    public void SubscribeToOnDestroyed(EntityEventHandler call) {
        OnDestroyedEvent += call;
    }

    public void SubscribeToPositionChangedEvent(PositionEventHandler call) {
        OnPositionChangedEvent += call;
    }
}
