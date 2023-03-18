using Godot;
using System.Collections.Generic;

public delegate void PartyMapEvent(Location location);
public delegate void PartyModifiedEvent(Party party);
public class Party : KinematicBody2D {
	public event PartyMapEvent PartyArrivedAtLocation;
	public event PartyModifiedEvent PartyModifiedEvent;
	public event PartyModifiedEvent PartyDestroyedEvent;

	[Export(PropertyHint.Range, "0,96,")]
	float WorldSize;
	Stopwatch stopwatch = new Stopwatch();


	public int MaxHealth = 100;
	public int Health = 100;

	// Party attributes.
	float TurnCompletionTime = 1.2f;
	float speed = 3f;
	SmoothStep smoothSpeed;

	public bool isMoving;
	public Location current;
	Location target;

	List<PartyMember> partyMembers = new List<PartyMember>();

	public void ModifyHealth(int amount) {
		Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
		PartyModifiedEvent?.Invoke(this);
	}


	public void Initialize(WorldMap worldMap, int maxHealth) {
		MaxHealth = maxHealth;

		// Get references to party sprites
		foreach (Node node in GetChildren()) {
			if (node.IsInGroup("PartyMember")) {
				partyMembers.Add((PartyMember)node);
			}
		}
		// Add party member to world scene
		for (int i = 0; i < partyMembers.Count; ++i)  {
			foreach (string g in GetGroups()) {
				partyMembers[i].AddToGroup(g);
			}
			RemoveChild(partyMembers[i]);
			worldMap.AddChild(partyMembers[i]);
			float angle = i * (2f * Mathf.Pi )/ partyMembers.Count;
			partyMembers[i].Initialize(WorldSize, angle);
		}
	}

	public void SetLocation(Location currentLocation) {
		GlobalPosition = currentLocation.GlobalPosition;
		current = currentLocation;
		foreach (var partyMember in partyMembers) {
			partyMember.SetPositionImmediate(currentLocation.GlobalPosition);
		}
	}

	public override void _Ready() {
		smoothSpeed = new SmoothStep(speed, 0.5f);
		Reset();
	}

	public override void _Process(float delta) {
		HandleInput();
		if (stopwatch.Advance(delta)) {
			Arrive();
		}
	}

	public void Reset() {
		Health = MaxHealth;
		PartyModifiedEvent?.Invoke(this);
	}

	private void Arrive() {
		GD.Print("Arrived!");
		current = target;
		target = null;
		GlobalPosition = current.GlobalPosition;
		PartyArrivedAtLocation?.Invoke(current);
	}

	public void Destroy() {
		PartyDestroyedEvent?.Invoke(this);
		foreach (var partyMember in partyMembers) {
			partyMember.QueueFree();
		}
		QueueFree();
	}

	private void HandleInput() {
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
	}

	public void SetTarget(Location location) {
		target = location;
		stopwatch.Start(GetTimeToTarget(location.GlobalPosition));
		SetPartyMemberPositions(location.GlobalPosition);
	}

	private void SetPartyMemberPositions(Vector2 position) {
		foreach (var partyMember in partyMembers) {
			partyMember.SetTargetPosition(position);
		}
	}

	private float GetTimeToTarget(Vector2 target) {
		return (target - GlobalPosition).Length() / (speed * Globals.PixelsPerUnit);
	}
}

public class Stopwatch {
	private float length;
	private float current;
	private bool running;

	public void Start(float length) {
		this.length = length;
		running = true;
		current = 0;
	}


	public bool Advance(float delta) {
		if (running && current >= length) {
			running = false;
			return true;
		}
		current += delta;
		return false;
	}
}

public class SmoothStep {
	float value;
	float smoothValue;
	float time;
	float smoothTime;
	public SmoothStep(float value, float smoothTime) {
		this.value = value;
		this.smoothTime = smoothTime;
		time = 0;
	}

	public float Advance(float delta) {
		time += delta;
		float t; 
		if (smoothTime == 0) 
			t = 1;
		else 
			t = Mathf.Clamp(time / smoothTime, 0, 1);
		return value * t;
	}

	public void Reset() {
		time = 0;
	}
}