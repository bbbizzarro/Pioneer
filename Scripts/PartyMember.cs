using Godot;
using System;
using System.Collections.Generic;

public class PartyMember : KinematicBody2D {
	[Export] float speed = 3f;

	Sprite sprite;
	Vector2 targetPosition;
	float offsetRadius;
	float offsetAngle;
	float randRadiusScale = 1f;
	Lerp speedLerp;

	// Animations
	float idleAnimationSpeed;
	float runAnimationSpeed;
	AnimationPlayer animationPlayer;
	RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready() {
		sprite = (Sprite)GetNode("Pivot/Sprite");
		rng.Randomize();
		SetRandomValues();
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		speedLerp = new Lerp(0, speed, 0);
		AnimateIdle();
    }

	public override void _Process(float delta) {
		Move(delta);
	}

	// Move target destination by direction intersecting area 2ds, that way you don't get into the situation
	// of an ifinite travel time.

	public void Initialize(float offsetRadius, float offsetAngle) {
		this.offsetAngle = offsetAngle;
		this.offsetRadius = offsetRadius;
	}

	public void SetTargetPosition(Vector2 target) {
		//Vector2 offsetDir = new Vector2(-Mathf.Sin(offsetAngle), Mathf.Cos(offsetAngle));
		//targetPosition = target + offsetRadius * offsetDir;
		SetTargetPosition(target, 1f);
	}

	public void SetTargetPosition(Vector2 target, float randRadiusScale) {
		Vector2 offsetDir = new Vector2(-Mathf.Sin(offsetAngle), Mathf.Cos(offsetAngle));
		targetPosition = target + randRadiusScale * offsetRadius * offsetDir;
		speedLerp = new Lerp(0, speed, rng.RandfRange(0, 0.4f));
		SetRandomValues();
	}

	private void SetRandomValues() {
		idleAnimationSpeed = rng.RandfRange(1.2f, 1.4f);
		runAnimationSpeed = rng.RandfRange(2.0f, 2.6f);
	}

	public void SetPositionImmediate(Vector2 target) {
		SetTargetPosition(target);
		GlobalPosition = targetPosition;
	}

	public void SetTexture(Texture texture) {
		this.sprite.Texture = texture;
	}

	public void SetDirection(Vector2 direction) {
		sprite.FlipH = direction.x > 0;
		//if (direction.x > 0)
		//	sprite.Scale = new Vector2(-1f, 1f);
	}

    private void Move(float delta) {
		if (!IsAtTarget()) {
			var velocity = GetVelocity(speedLerp.Advance(delta));
			MoveAndSlide(velocity);
			AnimateRun();
			SetDirection(velocity);
		}
		else 
			AnimateIdle();
	}

	private Vector2 GetVelocity(float speed) {
		return speed * Globals.PixelsPerUnit * (targetPosition - GlobalPosition).Normalized();
	}

	private bool IsAtTarget() {
		return (targetPosition - GlobalPosition).Length() <= 0.1f * Globals.PixelsPerUnit;
	}

	private void AnimateIdle() {
		animationPlayer.Play("Idle", customBlend: 0.25f, customSpeed: idleAnimationSpeed);
	}
	private void AnimateRun() {
		animationPlayer.Play("Run", customBlend: 0f, customSpeed: runAnimationSpeed);
	}
}

public class Lerp {
	float start;
	float end;
	float time;
	float duration;
	public Lerp(float start, float end, float duration) {
		this.start = start; this.end = end; this.duration = duration;
		time = 0;
	}

	public float Advance(float delta) {
		time = Mathf.Clamp(time + delta, 0, duration);
		if (duration == 0) return end;
		float t = time / duration;
		return end * t + start * (1f - t);
	}
}
