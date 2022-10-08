using Godot;
using System;

public class Particles : CPUParticles2D {
    [Export] float _lifeTime = 1f;

    Timer _timer;

    public override void _Ready() {
        base._Ready();
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(OnTimeout));
        _timer.Start(_lifeTime);
    }

    private void OnTimeout() {
        QueueFree();
    }
}
