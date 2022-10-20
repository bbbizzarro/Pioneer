using Godot;
using System;

public class Game : Node {

    PlayerController _player;
    UI _ui;

    public override void _Ready() {
        _player = (PlayerController)GetNode("World/Player");
        _player.Inventory.Add("Axe", 1);
        _ui = (UI)GetNode("UI");
        _ui.SetInventory(_player.Inventory);
    }

    public override void _Process(float delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("PauseMenu")) {
            GetTree().Paused = !GetTree().Paused;
        }
    }

}
