using Godot;
using System;
using System.Collections.Generic;

public class Hotbar : GridContainer {

    List<ItemUI> _itemSlots = new List<ItemUI>();

    public override void _Ready() {
        base._Ready();
        SetupItemUI();
    }

    private void SetupItemUI() {
        // Give slots a reference to main UI node for previewing drag.
        foreach (var node in GetChildren()) {
            var slot = (ItemUI)node;
            _itemSlots.Add(slot);
        }
    }

    public void Set(Inventory inventory) {
        for (int i = 0; i < _itemSlots.Count; ++i) {
            _itemSlots[i].Initialize(i, inventory);
        }
    }
}
