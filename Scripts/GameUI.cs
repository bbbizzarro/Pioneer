using Godot;
using System;

public class GameUI : CanvasLayer {
    [Export] NodePath HealthNode;

    Label winScreen;
    ProgressBar health;
    WorldMap worldMap;
    BaseButton attackButton;

    public override void _Ready() {
        health = (ProgressBar)GetNode("VBoxContainer/Health");
        attackButton = (BaseButton)GetNode("AttackButton");
        attackButton.Connect("pressed", this, nameof(HandleAttack));
        attackButton.Hide();
        winScreen = (Label)GetNode("WinScreen");
        winScreen.Hide();
    }

    public void HandleAttack() {
        worldMap.GetCombatSystem().Attack();
	}

    public void Initialize(WorldMap worldMap) {
        this.worldMap = worldMap;
        worldMap.GetPlayer().PartyModifiedEvent += UpdatePartyUI;
        worldMap.EnterCombat += ShowCombatUI;
        worldMap.ExitCombat += HideCombatUI;
	}

    public void ShowCombatUI() {
        attackButton.Show();
	}

    public void HideCombatUI() {
        attackButton.Hide();
	}

    public void UpdatePartyUI(Party party) {
        health.Value = party.Health;
	}

    public void DisplayWinScreen() {
        winScreen.Show();
	}
}
