using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
	Cursor cursor;

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
		cursor = (Cursor)GetNode("Cursor");
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
		location.RevealNearbyLocations();

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

	private Stack<Vector2> RandomSequence(int xLow, int xHigh, int yLow, int yHigh) {
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		int size = (xHigh + 1 - xLow) * (yHigh + 1 - yLow);
		List<Vector2> sequence = new List<Vector2>(size);
		for (int x = xLow; x <= xHigh; ++x) {
			for (int y = yLow; y <= yHigh; ++y) {
				sequence.Add(new Vector2(x, y));
			}
		}
		for (int i = 0; i < sequence.Count - 2; ++i) {
			int r = rng.RandiRange(0, size - 1);
			Vector2 temp = sequence[i];
			sequence[i] = sequence[r];
			sequence[r] = temp;
		}
		return new Stack<Vector2>(sequence);
	}

	private Stack<int> RandomSequence(int from, int to, int step) {
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		List<int> sequence = new List<int>();
		for (int x = from; x <= to; x+= step) {
			sequence.Add(x);
		}
		for (int i = 0; i < sequence.Count - 2; ++i) {
			int r = rng.RandiRange(0, sequence.Count - 1);
			int temp = sequence[i];
			sequence[i] = sequence[r];
			sequence[r] = temp;
		}
		return new Stack<int>(sequence);
	}

	private Stack<Edge> RandomEdgeSequence(IEnumerable<Edge> edges) {
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		List<Edge> sequence = new List<Edge>(edges);
		for (int i = 0; i < sequence.Count - 2; ++i) {
			int r = rng.RandiRange(0, sequence.Count - 1);
			Edge temp = sequence[i];
			sequence[i] = sequence[r];
			sequence[r] = temp;
		}
		return new Stack<Edge>(sequence);

	}

	private void GenerateWorld() {
		int mapHeight = 8;
		int mapWidth = 16;
		int numberOfLocations = 20;
		var rng = new RandomNumberGenerator();
		rng.Randomize();

		locations = new List<Location>();
		// Create start location.
		locations.Add(CreateLocation(new Vector2(-(mapWidth + 2)/2 * Globals.PixelsPerUnit, 0)));
		// Create end location.
		locations.Add(CreateLocation(new Vector2((mapWidth + 2)/2 * Globals.PixelsPerUnit, 0)));
		finalLocation = locations[1];

		int randomOffsetX = Mathf.RoundToInt(0.2f * Globals.PixelsPerUnit);
		int randomOffsetY = Mathf.RoundToInt(0.1f * Globals.PixelsPerUnit);

		var columns = new List<List<Location>>();
		int step = 2;
		int alternate = 0;
		float ySquish = 3f/4f;
		//float ySquish = 4f/5f;
		int prev = 0;
		int randCount;
		for (int i = 1; i < mapWidth; i+=step) {
			columns.Add(new List<Location>());
			var randSequence = RandomSequence(-(mapHeight + alternate % 2)/ 2, (mapHeight - alternate % 2)/2, 2);
			alternate += 1;
			if (prev == 3) {
				randCount = rng.RandiRange(1, 2);
			}
			else {
				randCount = rng.RandiRange(1, 3);
			}
			prev = randCount;
			for (int c = 0; c < randCount; ++c) {
				var l = CreateLocation(new Vector2(
					Globals.PixelsPerUnit * (i - mapWidth / 2) + rng.RandiRange(-randomOffsetX, randomOffsetX), 
					Globals.PixelsPerUnit * ySquish * randSequence.Pop() + rng.RandiRange(-randomOffsetY, randomOffsetY)));
				locations.Add(l);
				columns[i/step].Add(l);
			}
		}

		for (int i = 0; i < columns.Count; ++i) {
			var orderedColumn = new List<Location>(columns[i].OrderBy(loc => loc.GlobalPosition.y));
			columns[i] = orderedColumn;
		}

		for (int c = 0; c < columns.Count - 1; ++c) {
			int lastRight = 0;
			for (int l = 0; l < columns[c].Count; ++l) {
				if (l == columns[c].Count - 1) {
					for (int r = lastRight; r < columns[c+1].Count; ++r) {
						//if ((columns[c+1][r].GlobalPosition - columns[c][l].GlobalPosition).Length() <= 5 * Globals.PixelsPerUnit)
							//AddEdge(columns[c][l], columns[c+1][r]);
							AddPath(columns[c][l], columns[c+1][r]);
					}
				}
				else {
					for (int n = 0; n < rng.RandiRange(1, 2); ++n) {
						if (!columns[c][l].IsAdjacentsTo(columns[c+1][lastRight])) {
								//&& (columns[c+1][lastRight].GlobalPosition - columns[c][l].GlobalPosition).Length() <= 5 * Globals.PixelsPerUnit) {
							//AddEdge(columns[c][l], columns[c+1][lastRight]);
							AddPath(columns[c][l], columns[c+1][lastRight]);
							if (n > 0) {
								lastRight = Mathf.Clamp(lastRight + 1, 0, columns[c+1].Count-1);
							}
						}
					}
				}
			}
		}	

		foreach (var loc in columns[0]) {
			AddPath(locations[0], loc);
		}
		foreach (var loc in columns[columns.Count - 1]) {
			AddPath(loc, locations[1]);
		}

		//Party enemy = (Party)InitializeNode("PartyBase");
		//enemy.Initialize(this, 30);
		//locations[1].AddEnemy(enemy);

		//AddEdge(locations[0], locations[1]);

		player.Reset();
		player.SetLocation(locations[0]);
		locations[0].RevealNearbyLocations();
	}

	private Location CreateLocation(Vector2 position) {
		var location = (Location)InitializeNode("Location");
		location.GlobalPosition = position;
		location.Initialize(this);
		location.LocationSelected += cursor.SetCursor;
		location.LocationDeselected += cursor.UnsetCursor;
		return location;
	}

	private Node InitializeNode(string name) {
		var node = SceneDB.Instance.GetScene(name).Instance();
		AddChild(node);
		return node;
	}

	private void AddPath(Location a, Location b) {
		var path = (Path)SceneDB.Instance.Create("Line", this);
		path.Connect(a, b);
	}

	private Edge AddEdge(Location a, Location b) {
		var e = new Edge(a, b);
		RenderEdge(e);
		a.AddAdjacent(b);
		b.AddAdjacent(a);
		return e;
	}

	private void AddEdge(Edge edge) {
		RenderEdge(edge);
		edge.Start.AddAdjacent(edge.End);
		edge.End.AddAdjacent(edge.Start);
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
