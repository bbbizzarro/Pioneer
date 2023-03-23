using Godot;
using System;
using System.Collections.Generic;

public delegate void LocationClickHandler(Location location);
public delegate void LocationEventHandler(Location location);
public class Location : Node2D, IPositional {
	List<Location> adjacents = new List<Location>();
	List<Path> paths = new List<Path>();
	HashSet<IPositional> connections = new HashSet<IPositional>();
	public event LocationClickHandler LocationClickEvent;
	public event LocationEventHandler LocationSelected;
	public event LocationEventHandler LocationDeselected;
	Area2D area;
	Sprite sprite;
	public Party Enemy;

	List<Prop> props = new List<Prop>();
	public bool isHidden {private set; get; }
	Timer timer;

	public static Location CurrentSelected;

	public override void _Ready() {
		area = (Area2D)GetNode("Area2D");
		area.Connect("mouse_entered", this, nameof(HandleMouseEnter));
		area.Connect("mouse_exited", this, nameof(HandleMouseExit));
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(RevealProps));
		//sprite = (Sprite)GetNode("Sprite");
	}

	public void AddPath(Path path) {
		paths.Add(path);
		if (path.Start != this) 
			adjacents.Add(path.Start);
		else 
			adjacents.Add(path.End);
		
	}

	public void RevealNearbyLocations() {
		Reveal(this);
		foreach (var adj in adjacents) {
			adj.Reveal(this);
		}
		foreach (var path in paths) {
			path.Reveal(this);
		}
	}

	public void Initialize(WorldMap worldMap) {
		foreach (Node node in GetChildren()){
			if (node.IsInGroup("Prop")) {
				var prop = (Prop)node; 
				props.Add(prop);
				RemoveChild(prop);
				worldMap.AddChild(prop);
				prop.GlobalPosition += GlobalPosition;
			}
		}
		props[0].SetSpriteByCategory("Large");
		props[1].SetSpriteByCategory("Small");
		props[2].SetSpriteByCategory("Small");
		HideLocation();
	}

	public void Reveal(Location location) {
		if (!isHidden) return;
		isHidden = false;
		if (timer == null) 
			RevealProps();
		else {
			float distance = (location.GlobalPosition - GlobalPosition).Length();
			if (distance == 0) 
				RevealProps();
			else {
				float delay = 0.08f * distance/ Globals.PixelsPerUnit;
				timer.Start(delay);
			}
		}
	}

	private void RevealProps() {
		Audio.Instance.Play("Pop");
		foreach (var prop in props) {
			prop.Reveal();
		}
	}

	public void HideLocation() {
		if (isHidden) return;
		isHidden = true;
		foreach (var prop in props) {
			prop.HideProp();
		}
	}

	private void RandomizeProps() {
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
		//if (isHidden) return;
		CurrentSelected = this;
		LocationSelected?.Invoke(this);
		//sprite.Scale = new Vector2(1.2f, 1.2f);
	}
	public void HandleMouseExit() {
		if (CurrentSelected == this) {
			CurrentSelected = null;
			LocationDeselected?.Invoke(this);
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

    public void Connect(IPositional p) {
        throw new NotImplementedException();
    }

	public Vector2 GetPos() {
		return GlobalPosition;
	}
}
