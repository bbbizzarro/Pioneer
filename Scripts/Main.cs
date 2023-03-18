using Godot;
using System;

public class Main : Node2D {
	WorldMap worldMap;
	GameUI ui;

	public override void _Ready() {
		worldMap = (WorldMap)GetNode("WorldMap");
		ui = (GameUI)GetNode("UI");
		ui.Initialize(worldMap);
		worldMap.PlayerAtFinalDest += HandleWinCondition;
	}

	public void HandleWinCondition() {
		//ui.DisplayWinScreen();
	}
}
