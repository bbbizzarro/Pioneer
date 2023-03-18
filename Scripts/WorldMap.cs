using Godot;
using System;
using System.Collections.Generic;

public delegate void MapEvent();
public class WorldMap : Node2D {
	public event MapEvent PlayerAtFinalDest;
	public event MapEvent EnterCombat;
	public event MapEvent ExitCombat;

	CombatSystemBase combatSystem = new CombatSystemBase();	

	Party player;
	List<Location> locations = new List<Location>();
	Location finalLocation;
	State currentState = State.Travel;

	public enum State { 
		Travel,
		Combat
	}

	public abstract class WorldMapState {

		protected WorldMap worldMap;

		public WorldMapState(WorldMap worldMap) {
			this.worldMap = worldMap;
			OnEnter();
		}

		public virtual void Process() { 
		}

		protected virtual void OnEnter() {
		}
		protected virtual void OnExit() {
		}
	}

	public CombatSystemBase GetCombatSystem() {
		return combatSystem;
	}

	public class TravelState : WorldMapState { 
		public TravelState(WorldMap worldMap) : base(worldMap) {}

		public override void Process() {
			worldMap.HandleInput();
		}
	}

	public override void _Ready() {
		GD.Print("Generating world.");
		InitializePlayer();
		GenerateWorld();
	}
	public Party GetPlayer() {
		return player;
	}

	private void InitializePlayer() { 
		player = (Party)GetNode("Player");
		player.Initialize(this, 100);
		player.PartyArrivedAtLocation += HandlePlayerArrival;
	}

	private void ChangeState(State state) {
		currentState = state;
	}

	public override void _Process(float delta) {
		switch (currentState) {
			case State.Travel:
					HandleInput();
				break;
			case State.Combat:
					//HandleCombat(delta);
				break;
		}
	}

	private void HandleCombat(float delta) { 
		combatSystem.Simulate(delta);
	}

	public void Reset() {
		locations.Clear();
		foreach (Node c in GetChildren()) { 
			if (!c.IsInGroup("Permanent")) {
				c.QueueFree();
			}
		}
		GenerateWorld();
	}

	public void HandlePlayerArrival(Location location) { 
		if (location == finalLocation) {
			PlayerAtFinalDest?.Invoke();
			Reset();
		}
		else if (location.HasEnemy()) {
			EnterCombat?.Invoke();
			ChangeState(State.Combat);
			combatSystem = new CombatSystem(player, location.Enemy);
			combatSystem.CombatCompleteEvent += HandleCombatComplete;
		}
	}

	public void HandleCombatComplete() {
		combatSystem = new CombatSystemBase();
		ChangeState(State.Travel);
		ExitCombat?.Invoke();
	}

	public void HandleLocationSelect(Location location) { 
		if (!player.isMoving && player.current.IsAdjacentsTo(location)) {
			player.SetTarget(location);
		}
	}

	private Stack<int> RandomSequence(int start, int end) {
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		int size = end - start;
		List<int> sequence = new List<int>(size);
		for (int i = 0; i < size; ++i) {
			sequence.Add(i + start);
		}
		for (int i = 0; i < size - 2; ++i) {
			int r = rng.RandiRange(i, size - 1);
			int temp = sequence[i];
			sequence[i] = sequence[r];
			sequence[r] = temp;
		}
		return new Stack<int>(sequence);
	}

	private void GenerateWorld() {
		int numberOfLocations = 3;
		int mapSize = 2;
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		var sequenceX = RandomSequence(-mapSize, mapSize);
		var sequenceY = RandomSequence(-mapSize, mapSize);

		locations = new List<Location>();

		for (int i = 0; i < numberOfLocations; ++i) {
			int randX = sequenceX.Pop() * Globals.PixelsPerUnit;
			int randY = sequenceY.Pop() * Globals.PixelsPerUnit;
			Vector2 position = new Vector2(randX, randY);
			var location = (Location)InitializeNode("Location");
			locations.Add(location);
			location.GlobalPosition = position;
			location.Initialize(this);
		}
		finalLocation = locations[2];

		//Party enemy = (Party)InitializeNode("PartyBase");
		//enemy.Initialize(this, 30);
		//locations[1].AddEnemy(enemy);

		AddEdge(locations[0], locations[1]);
		AddEdge(locations[1], locations[2]);

		player.Reset();
		player.SetLocation(locations[0]);
	}

	private Node InitializeNode(string name) {
		var node = SceneDB.Instance.GetScene(name).Instance();
		AddChild(node);
		return node;
	}

	private Edge AddEdge(Location a, Location b) {
		var e = new Edge(a, b);
		RenderEdge(e);
		a.AddAdjacent(b);
		b.AddAdjacent(a);
		return e;
	}

	private void RenderEdge(Edge edge) { 
		var line = (Line2D)SceneDB.Instance.GetScene("Line").Instance();
		AddChild(line);
		line.ClearPoints();
		line.AddPoint(edge.Start.GlobalPosition);
		line.AddPoint(edge.End.GlobalPosition);
	}

	private void FindNearestTravelPoint() {
		var pos = GetGlobalMousePosition();
	}

	public void HandleInput() { 
		if (Input.IsActionJustPressed("Select")) {
			if (Location.CurrentSelected != null) {
				HandleLocationSelect(Location.CurrentSelected);
			}
		}
	}
}

public class LocationData {
	public Vector2 position;
}

public class Edge {
	public Location Start { private set; get; }
	public Location End { private set; get; }

	public Edge(Location start, Location end) {
		Start = start; End = end;
	}

	public override int GetHashCode() {
		return Start.GetHashCode() + End.GetHashCode();
	}

	public override bool Equals(object obj) {
		var inEdge = (Edge)obj;
		return (Start == inEdge.Start && End == inEdge.End) ||
			   (Start == inEdge.End && End == inEdge.Start);
	}
}

public class CombatSystemBase { 

	float animationLength;
	float animationTimer;

	public event CombatSystemEvent CombatCompleteEvent;
	public CombatSystemBase() { }
	public virtual void Attack() { }
	public virtual void AttackPlayer() {}
	public virtual void AttackEnemy() {}
	public virtual void Simulate(float delta) {}
	protected virtual bool Advance(float delta) {
		if (animationTimer >= animationLength) {
			return true;
		}
		animationTimer += delta;
		return false;
	}
	protected void EndCombat() { 
		CombatCompleteEvent?.Invoke();
	}
}

public delegate void CombatSystemEvent();
public class CombatSystem : CombatSystemBase {

	Party playerParty;
	Party enemyParty;
	int currentTurn;
	RandomNumberGenerator rng;
	TimeTracker timer;
	bool combatComplete;

	public CombatSystem(Party playerParty, Party enemyParty) {
		this.playerParty = playerParty;
		this.enemyParty = enemyParty;
		timer = new TimeTracker(1f, true);

		//playerParty.ArrangPartySprites(Globals.PixelsPerUnit * Vector2.Right);
		//enemyParty.ArrangPartySprites(Globals.PixelsPerUnit * Vector2.Left);
		rng = new RandomNumberGenerator();
		rng.Randomize();
	}

	public override void Attack() {
		enemyParty.ModifyHealth(-50);
		CheckStats();
	}

	public override void AttackPlayer() {
		int roll = rng.RandiRange(10, 50);
		PrintAttack("Enemy", "Player", roll);
		playerParty.ModifyHealth(-roll);
	}
	public override void AttackEnemy() {
		int roll = rng.RandiRange(10, 50);
		PrintAttack("Player", "Enemy", roll);
		enemyParty.ModifyHealth(-roll);
	}

	private void PrintAttack(string source, string target, int damageAmount) {
		GD.Print(String.Format("{0} attacks {1} for {2} damage.", source, target, damageAmount));
	}

	public override void Simulate(float delta) {
		if (combatComplete) return;
		if (!timer.Advance(delta)) {
			return;
		}
		if (currentTurn % 2 == 0) {
			AttackEnemy();
		}
		else {
			AttackPlayer();
		}
		currentTurn += 1;
		CheckStats();
	}

	private void CheckStats() { 
		if (enemyParty.Health == 0) {
			enemyParty.Destroy();
			//playerParty.ArrangPartySprites(Vector2.Zero);
			combatComplete = true;
			EndCombat();
		}
	}
}

public class TimeTracker {
	float length;
	float now;
	bool startAtEnd;

	public TimeTracker(float length) {
		this.length = length;
	}

	public TimeTracker(float length, bool startAtEnd) {
		this.length = length;
		Reset();
	}

	public bool Advance(float delta) {
		if (now >= length) {
			now = 0;
			return true;
		}
		now += delta;
		return false;
	}

	public void Reset() {
		if (startAtEnd) now = length;
		else now = 0;
	}
}
