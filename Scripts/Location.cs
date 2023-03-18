using Godot;
using System;
using System.Collections.Generic;

public delegate void LocationClickHandler(Location location);
public class Location : Node2D {
	List<Location> adjacents = new List<Location>();
	public event LocationClickHandler LocationClickEvent;
	Area2D area;
	Sprite sprite;
	public Party Enemy;

	List<Node2D> props = new List<Node2D>();

	public static Location CurrentSelected;

	public override void _Ready() {
		area = (Area2D)GetNode("Area2D");
		area.Connect("mouse_entered", this, nameof(HandleMouseEnter));
		area.Connect("mouse_exited", this, nameof(HandleMouseExit));
		//sprite = (Sprite)GetNode("Sprite");
	}

	public void Initialize(WorldMap worldMap) {
		foreach (Node node in GetChildren()){
			if (node.IsInGroup("Prop")) {
				var prop = (Node2D)node; 
				props.Add(prop);
				RemoveChild(prop);
				worldMap.AddChild(prop);
				prop.GlobalPosition += GlobalPosition;
			}
		}
	}

	public void AddEnemy(Party party) {
		Enemy = party;
		party.SetLocation(this);
		party.PartyDestroyedEvent += HandleEnemyDestruction;
	}

	public bool HasEnemy() {
		return Enemy != null;
	}

	public void HandleEnemyDestruction(Party party) {
		if (Enemy == party) {
			Enemy = null;
		}
	}

	public void HandleMouseEnter() {
		CurrentSelected = this;
		//sprite.Scale = new Vector2(1.2f, 1.2f);
	}
	public void HandleMouseExit() {
		if (CurrentSelected == this) {
			CurrentSelected = null;
		}
		//sprite.Scale = Vector2.One;
	}

	public void AddAdjacent(Location location) {
		adjacents.Add(location);
	}
	public bool IsAdjacentsTo(Location location) {
		return adjacents.Contains(location);
	}

	public void OnMouseClick() {
		LocationClickEvent?.Invoke(this);
	}
}
