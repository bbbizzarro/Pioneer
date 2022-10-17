using Godot;
using System;

public class UI : CanvasLayer {
    [Export] NodePath HotbarPath;

    Hotbar _hotbar;
    Inventory _inventory;

    public override void _Ready() {
        LoadHotbar();
    }

    public void SetInventory(Inventory inventory) {
        _inventory = inventory;
        _inventory.UpdatedEvent += UpdateInventoryUI;
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI() {
        GD.Print("Updating inventory.");
        _hotbar.Set(_inventory);
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
