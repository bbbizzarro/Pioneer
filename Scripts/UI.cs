using Godot;
using System;

public class UI : CanvasLayer {
	public static UI Instance;

	[Export] NodePath HotbarPath;

	Hotbar _hotbar;
	Control _inventoryUI;
	Inventory _inventory;

	public override void _Ready() {
		if (Instance != null) {
			QueueFree();
			return;
		}
		Instance = this;
		LoadHotbar();
	}

	public void SetInventory(Inventory inventory) {
		_inventory = inventory;
		_inventory.UpdatedEvent += UpdateInventoryUI;
		UpdateInventoryUI();
	}

	public void UpdateInventoryUI() {
		_hotbar.Set(_inventory);
	}

	private void LoadInventoryUI() {
		try {
			_inventoryUI = (Control)GetNode("InventoryContainer");
		}
		catch {
			GD.PrintErr("Could not find Inventory UI");
		}
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
