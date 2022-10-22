using Godot;
using System;
using System.Collections.Generic;

public class Interactor : Area2D {
    Inventory _inventory;
    Interactable _current;
    List<Interactable> _interactables = new List<Interactable>();

    public void Initialize(Inventory inventory) {
        _inventory = inventory;
    }

    public override void _Ready() {
        MakeConnections();
    }

    public void UpdateNearest() {
        HandleNewClosest();
    }

    public void Interact() {
        if (_inventory != null && _current != null) _current.OnInteract(_inventory);
    }

    private void MakeConnections() {
        Connect("area_entered", this, nameof(HandleAreaEntered));
        Connect("area_exited", this, nameof(HandleAreaExited));
    }

    private void HandleNewClosest() {
        var last = _current;
        _current = GetClosest();
        if (last == _current) return;
        if (last != null) last.OnExit();
        if (_current != null) _current.OnEnter();
    }

    private void ProcessNewInteractable(Interactable interactable) {
        if (interactable == null) return;
        _interactables.Add(interactable);
        HandleNewClosest();
    }

    private void ProcessOutInteractable(Interactable interactable) {
        if (interactable == null) return;
        _interactables.Remove(interactable);
        HandleNewClosest();
    }

    private float GetDistanceTo(Interactable interactable) {
        if (interactable == null) return 0;
        return (interactable.GlobalPosition - GlobalPosition).Length();
    }

    private Interactable GetClosest() {
        if (_interactables.Count == 0) return null;
        var toRemove = new List<Interactable>();
        // Setup tracking for closest interactable.
        Interactable closest = _interactables[0];
        float minDist = GetDistanceTo(closest);
        // Check each interactable to determine which is nearest.
        for (int i = 1; i < _interactables.Count; ++i) {
            if (_interactables[i] == null) toRemove.Add(_interactables[i]);
            float currDist = GetDistanceTo(_interactables[i]);
            if (currDist < minDist) {
                minDist = currDist;
                closest = _interactables[i];
            }
        }
        // Clean up null references
        foreach (var r in toRemove) {
            _interactables.Remove(r);
        }
        return closest;
    }

    private void HandleAreaEntered(Area2D area) {
        try {
            ProcessNewInteractable((Interactable)area);
        }
        catch {
            PrintErrorMessage();
        }
    }

    private void HandleAreaExited(Area2D area) {
        try {
            ProcessOutInteractable((Interactable)area);
        }
        catch {
            PrintErrorMessage();
        }
    }

    private void PrintErrorMessage() {
        GD.PrintErr("A non-interactable is in interactable layer.");
    }
}
