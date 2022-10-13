using Godot;
using System;

public class UI : CanvasLayer {
    [Export] NodePath HotbarPath;

    Hotbar _hotbar;

    public override void _Ready() {
        LoadHotbar();
    }

    public void UpdateInventoryUI() {
        GD.Print("Updating inventory.");
    }

    private void LoadHotbar() {
        if (HotbarPath != null) _hotbar = (Hotbar)GetNode(HotbarPath);
        if (_hotbar != null)
            _hotbar.Connect("mouse_entered", this, nameof(HandleMouseEnterUI));
    }

    public void HandleMouseEnterUI() {
    }

    public void HandleMouseExitUI() {
    }
}
