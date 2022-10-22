using Godot;
using System;

public delegate void InteractableEventHandler();
public delegate void StringIDEventHandler(string id);
public delegate void InventoryHandler(Inventory inventory);

public class Interactable : Area2D {
    public event InteractableEventHandler EnterEvent;
    public event InteractableEventHandler ExitEvent;
    public event InventoryHandler InteractEvent;

    public void OnInteract(Inventory inventory) {
        InteractEvent?.Invoke(inventory);
    }

    public void OnEnter() {
        EnterEvent?.Invoke();
    }

    public void OnExit() {
        ExitEvent?.Invoke();
    }
}
