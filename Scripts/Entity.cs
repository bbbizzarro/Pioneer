using Godot;
using System;

public class Entity : KinematicBody2D {
	[Export] NodePath NodeToFollowPath;
	[Export] float restDistance = 8f;

	Node2D followNode;

	// Godot Built-ins
	public override void _Ready() {
		if (NodeToFollowPath != null) { 
			followNode = (Node2D)GetNode(NodeToFollowPath);
		}
	}

	public override void _Process(float delta) {
	}

	public override void _PhysicsProcess(float delta) {
		if (followNode != null) {
			MoveToward(followNode.GlobalPosition);
		}
	}
	// ------------------------------------------ 

	// Set the velocity of entity in the direction of a target point.
	private void MoveToward(Vector2 target) {
		Vector2 direction = target - GlobalPosition;
		if (direction.Length() <= restDistance) return;
		float speed = 0.25f;
		MoveAndSlide(speed * Globals.PixelsPerUnit * direction.Normalized());
	}
}
