using Godot;
using System;

public class Physics2D {
	const float Epsilon = 0.00001f;

	Vector2 _initPos;
	Vector2 _position;
	Vector2 _velocity;

	private Vector2 _gravityDir = Vector2.Down;
	private float _gravityStrength;

	float _yLimit;
	float _drag;
	float _timer;
	float _timeLimit;

    public Physics2D(float gravityStrength, float drag) {
        _gravityStrength = gravityStrength;
        _drag = drag;
    }

	public void Set(float yLimit, Vector2 velocity, Vector2 position, float timeLimit) {
		_initPos = position;
		_position = position;
		_yLimit = yLimit;
		_velocity = velocity;
		_timeLimit = timeLimit;
		_timer = 0;
	}

	public void Stop() {
		_timer = _timeLimit + 1;
	}

	public Vector2 Update(float delta) {
		if (_timer >= _timeLimit) {
            return _position;
		}
        Advance(delta);
		Impact(_position);
        return _position;
	}

    public bool IsRunning() {
        return _timer < _timeLimit;
    }

    private void Advance(float delta) {
		_timer += delta;
		_velocity += _gravityStrength * delta * _gravityDir;
		_position += delta * _velocity; 
    }

	private void Impact(Vector2 newPosition) { 
		if (newPosition.y > _yLimit) {
			_position = new Vector2(_position.x, _yLimit);
			_velocity = new Vector2(_drag * _velocity.x, -_drag * _velocity.y);
		}
	}
}

