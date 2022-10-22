using Godot;
using System;
using System.Collections.Generic;

public class NPC : Character {

    Vector2 _direction;
    State _state;
    Dictionary<States, State> _states = new Dictionary<States, State>();
    RandomNumberGenerator _rng = new RandomNumberGenerator();

    public override void _Ready() {
        base._Ready();
        _states.Add(States.Wander, new WanderState(this));
        _states.Add(States.Idle, new IdleState(this));
        SetState(States.Idle);
        _rng.Randomize(); 
    }

    public override void _Process(float delta) {
      base._Process(delta);
      _state.Update(delta);
    }

    public void Move(Vector2 direction) {
        HandleMovement(direction);
        if (direction != Vector2.Zero) {
            FaceToward(GlobalPosition + direction);
            PositionHeld(GlobalPosition + Globals.PixelsPerUnit * direction);
        }
    }

    public Vector2 GetRandomCorrection() {
        return Vector2.Zero;
    }

    public void SetDirection() {
    }

    private Vector2 GetDirection() {
        var diff = (Vector2.Zero - GlobalPosition);
        if (diff.Length() < 0.1f * Globals.PixelsPerUnit) {
            return Vector2.Zero;
        }
        else {
            return diff.Normalized();
        }
    }

    private void SetState(States state) {
        _state = _states[state];
        _state.OnEnter();
    }

    public void ChangeState(States state) {
        if (!_states.ContainsKey(state)) return;
        _state.OnExit();
        _state = _states[state];
        _state.OnEnter();
    }

    // Handle states
    // -------------

    public enum States {
        Wander,
        Idle
    }

    public class State {
        protected NPC _npc;
        public State(NPC npc) {
            _npc = npc;
        }
        public virtual void OnEnter() {}
        public virtual void Update(float delta){}
        public virtual void OnExit() {}
    }

    public class IdleState : State {
        float _timer;
        float _timeout;
        RandomNumberGenerator _rng = new RandomNumberGenerator();

        public IdleState(NPC npc) : base(npc) {
            _rng.Randomize();
        }

        public override void OnEnter() {
            _timer = 0;
            _timeout = _rng.RandfRange(1f, 3f);
        }
        public override void Update(float delta) {
            if (_timer >= _timeout) {
                _npc.ChangeState(States.Wander);
            }
            _timer += delta;
        }
    }

    public class WanderState : State {
        Vector2 _target;
        RandomNumberGenerator _rng = new RandomNumberGenerator();
        
        public WanderState(NPC npc) : base(npc) {}
        public override void OnEnter() {
            float Range = 5;
            _target = new Vector2(_rng.RandfRange(-Range * Globals.PixelsPerUnit, Range * Globals.PixelsPerUnit),
                                  _rng.RandfRange(-Range * Globals.PixelsPerUnit, Range * Globals.PixelsPerUnit));
            GD.Print(_target);
        }

        public override void Update(float delta) {
            var diff = (_target - _npc.GlobalPosition);
            if (diff.Length() < 0.1f * Globals.PixelsPerUnit) {
                _npc.ChangeState(States.Idle);
            }
            else {
                _npc.Move(diff.Normalized());
            }
        }

        public override void OnExit() {
            _npc.Move(Vector2.Zero);
        }
    }
}
