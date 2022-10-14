using Godot;
using System;
using System.Collections.Generic;

public class Hotbar : GridContainer {

    List<ItemUI> _itemSlots = new List<ItemUI>();

    public override void _Ready() {
        base._Ready();
        Connect("mouse_entered", this, nameof(HandleMouseEnter));
        Connect("mouse_exited", this, nameof(HandleMouseExit));
        foreach (var node in GetChildren()) {
            _itemSlots.Add((ItemUI)node);
        }
    }

    public void Set(IEnumerable<Inventory.ItemStack> items) {
        if (_itemSlots.Count == 0) return;
        var slots = new List<Inventory.ItemStack>(items);
        for (int i = 0; i < _itemSlots.Count; ++i) {
            if (i >= slots.Count) return;
            if (!slots[i].IsEmpty()) {
                _itemSlots[i].Set(slots[i].ID, slots[i].Count);
            }
            else {
                _itemSlots[i].UnSet();
            }
        }
    }

    private void HandleMouseEnter() {
    }

    private void HandleMouseExit() {
    }
}
