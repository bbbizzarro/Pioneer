using Godot;
using System;

public class Main : Node2D {
	WorldMap worldMap;
	GameUI ui;
	bool gameStarted;
	CanvasLayer titleScreen;

	public override void _Ready() {
		worldMap = (WorldMap)GetNode("WorldMap");
		//ui = (GameUI)GetNode("UI");
		//ui.Initialize(worldMap);
		titleScreen = (CanvasLayer)GetNode("Title");
		worldMap.PlayerAtFinalDest += HandleWinCondition;
	}

	public override void _Process(float delta) {
		if (!gameStarted) {
			HandleInput();
		}
	}

	public void HandleInput() {
		if (Input.IsActionJustPressed("Select")) {
			titleScreen.Hide();
			gameStarted = true;
			worldMap.Initialize();
		}
	}

	public void HandleWinCondition() {
		//ui.DisplayWinScreen();
	}
}
